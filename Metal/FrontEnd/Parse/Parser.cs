using Metal.FrontEnd.Grammar;
using Metal.FrontEnd.Scan;
using Metal.FrontEnd.Exceptions;
using System.Collections.Generic;
using System;
namespace Metal.FrontEnd.Parse {
  public class Parser {
    
    private List<Token> tokens;
    private int position = 0;
    private Scanner scanner;
    private int loopDepth = 0;
    private bool allowExpression = true;
    private bool foundExpression = false;
    private bool enforceGrammarSemiColon = false;

    public bool IsAtEnd { get { return Current().Type == TokenType.EOF; } }

    public bool AllowExpression { get => allowExpression; set => allowExpression = value; }
    public bool EnforceGrammarSemiColon { get => enforceGrammarSemiColon; set => enforceGrammarSemiColon = value; }
    public Parser(Scanner scanner) {
      this.tokens = new List<Token>();
      this.scanner = scanner;
    }
    public Parser(List<Token> tokens) {
      this.tokens = tokens;
    }

    internal List<Statement> Parse() {
      if (scanner != null) {
        scanner.ScanSafeToken();
        tokens = scanner.Tokens;
      }
      List<Statement> statements = new List<Statement>();
      while (!IsAtEnd) {
        statements.Add(ParseDeclaration());
      }
      return statements;
    }

    internal object ParseREPL() {
      AllowExpression = true;
      List<Statement> statements = new List<Statement>();
      while (!IsAtEnd) {
        statements.Add(ParseDeclaration());
        if (foundExpression) {
          Statement last = statements[statements.Count - 1];
          if (last is Statement.Expr) {
            return ((Statement.Expr)last).Expression;
          }
        }
        AllowExpression = false;
      }
      return statements;
    }


    private Token Current() {
      return tokens[position];
    }
    private Token Next() {
      if (scanner != null) {
        scanner.ScanSafeToken();
        tokens = scanner.Tokens;
      }
      if (!IsAtEnd) position++;
      return PeekBack();
    }

    private Token Peek() {
      return tokens[position + 1];
    }

    private Token Peek(int to) {
      return to + position >= tokens.Count ? tokens[tokens.Count - 1] : tokens[to + position];
    }

    private Token PeekBack() {
      return tokens[position - 1];
    }

    private Token PeekBack(int to) {
      return to - position < 0 ? tokens[0] : tokens[to - position];
    }

    private void Synchronize() {
      Next();
      while (!IsAtEnd) {
        var type = Current().Type;
        var lexeme = Current().Lexeme;
        var types = new List<(TokenType, string)> {
          (TokenType.Reserved, "class"), (TokenType.Reserved, "func"),
          (TokenType.Reserved, "var"), (TokenType.Reserved, "let"),
          (TokenType.Reserved, "for"), (TokenType.Reserved, "if"),
          (TokenType.Reserved, "while"), (TokenType.Reserved, "return"),
          (TokenType.Reserved, "repeat"),
        };
        if (types.Contains((type, lexeme))) return;
        Next();
      }
    }

    private bool Match(params (TokenType, string)[] types) {
      foreach (var type in types) {
        if (Check(type)) {
          Next();
          return true;
        }
      }
      return false;
    }
    
    private bool Match(params TokenType[] types) {
      foreach (var type in types) {
        if (Check((type))) {
          Next();
          return true;
        }
      }
      return false;
    }

    private bool Check(params TokenType[] types) {
      foreach (var type in types) {
        if (Check((type, null))) return true;
      }
      return false;
    }

    private bool CheckNext(TokenType type) {
      if (IsAtEnd) return false;
      if (Peek().Equals(TokenType.EOF)) return false;
      return Peek().Equals(type);
    }

    private bool CheckNext((TokenType, string) type) {
      return type.Item2 == null ? CheckNext(type.Item1) : Peek().Equals(type.Item1, type.Item2);
    }

    private bool Check((TokenType, string) type) {
      if (IsAtEnd) return false;
      return type.Item2 == null ? Current().Equals(type.Item1) : Current().Equals(type.Item1, type.Item2);
    }

    private Token Consume((TokenType, string) type, string message) {
      if (Check(type)) return Next();
      throw Error(Current(), message);
    }
    private Token Consume(TokenType type, string message) {
      if (Check((type, null))) return Next();
      throw Error(Current(), message);
    }

    private MetalException.Parse Error(Token token, string message) {
      Metal.Error(token, message);
      return new MetalException.Parse();
    }

    /* Statements */

    private Statement ParseStatement() {

      // Parse for statement
      if (Match((TokenType.Reserved, "for"))) return ParseForStatement();

      // Parse if statement
      if (Match((TokenType.Reserved, "if"))) return ParseIfStatement();

      //// Parse print statement
      //if (Match((TokenType.Reserved, "print"))) {
        //Consume(TokenType.LeftParenthesisPunctuation, "Expect '(' after print.");
        //var print = ParsePrintStatement();
        //Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after print.");
        //if (Match(TokenType.SemiColonPunctuation)) {};
        //if (EnforceGrammarSemiColon)
        //  Consume(TokenType.SemiColonPunctuation, "Expect ';' after expression.");
        //return print;
      //}

      // Parse return statement
      if (Match((TokenType.Reserved, "return"))) return ParseReturnStatement();

      // Parse break statement
      if (Match((TokenType.Reserved, "break"))) return ParseBreakStatement();

      // Parse while statement
      if (Match((TokenType.Reserved, "while"))) return ParseWhileStatement();

      // Parse repeat-while statement
      if (Match((TokenType.Reserved, "repeat"))) return ParseRepeatWhileStatement();

      // Parse block statement
      if (Match(TokenType.LeftBracePunctuation)) {
        return new Statement.Block(ParseBlockStatement());
      }

      // Parse expression statement
      return ParseExpressionStatement();
    }


    private Statement ParseExpressionStatement() {
      Expression expr = ParseExpression();
      if (Match(TokenType.SemiColonPunctuation)) {}
      foundExpression = true;
      return new Statement.Expr(expr);
    }
    //private Statement ParsePrintStatement() {
    //  Expression value = ParseExpression();
    //  return new Statement.Print(value);
    //}

    private Statement ParseReturnStatement() {
      Token keyword = PeekBack();
      Expression value = null;
      if (!Check(TokenType.SemiColonPunctuation)) {
        value = ParseExpression();
      }
      if (Match(TokenType.SemiColonPunctuation)) {}
      return new Statement.Return(keyword, value);
    }

    private Statement ParseBreakStatement() {
      if (loopDepth == 0) {
        Error(PeekBack(), "Must be inside a loop to use 'break'.");
      }
      return new Statement.Break();
    }

    private List<Statement> ParseBlockStatement() {
      List<Statement> statements = new List<Statement>();
      while (!Check(TokenType.RightBracePunctuation) && !IsAtEnd) {
        statements.Add(ParseDeclaration());
      }
      Consume(TokenType.RightBracePunctuation, "Expect '}' after block.");
      return statements;
    }

    private Statement ParseDeclaration() {
      try {
        if (Check((TokenType.Reserved, "func")) && CheckNext(TokenType.Identifier)) {
          Consume((TokenType.Reserved, "func"), null);
          return ParseFuncDeclaration("function");
        }
        if (Match((TokenType.Reserved, "var"))) return ParseVarDeclaration();
        else return ParseStatement();
      } catch (MetalException.Parse) {
        Synchronize();
        return null;
      }
    }

    private Statement ParseVarDeclaration() {
      Token name = Consume(TokenType.Identifier, "Expect variable name.");
      Expression initializer = null;
      if (Match((TokenType.Operator, "="))) {
        initializer = ParseExpression();
      }
      if (Match(TokenType.SemiColonPunctuation)) {}
      return new Statement.Var(name, initializer);
    }


    public Statement.Function ParseFuncDeclaration(string kind) {
      Token name = Consume(TokenType.Identifier, string.Format("Expect {0} name.", kind));
      return new Statement.Function(name, ParseFuncExpression(kind));
    }

    private Statement ParseIfStatement() {
      var ifParenthesisEnabled = false;
      if (Check(TokenType.LeftParenthesisPunctuation)) {
        ifParenthesisEnabled = true;
        Next();
      }

      Expression condition = ParseExpression();

      if (ifParenthesisEnabled) {
        Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after 'if'.");
      }

      Statement thenBranch = ParseStatement();
      Statement elseBranch = null;
      if (Match((TokenType.Reserved, "else"))) {
        elseBranch = ParseStatement();
      }
      return new Statement.If(condition, thenBranch, elseBranch);
    }

    private Statement ParseForStatement() {
      try {
        loopDepth++;
        bool forParenthesisEnabled = false;
        if (Check(TokenType.LeftParenthesisPunctuation)) {
          forParenthesisEnabled = true;
          Next();
        }
        var rhs = forParenthesisEnabled ? "'('" : "'for'";
        Token name = Consume(TokenType.Identifier, "Expect variable name after " + rhs + ".");
        Consume((TokenType.Operator, "in"), "Expect 'in' after variable name.");
        Expression expression = ParseExpression();
        if (forParenthesisEnabled) {
          Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after for clauses.");
        }

        Statement body = ParseStatement();
        return new Statement.For(name, expression, body);
      } finally {
        loopDepth--;
      }
    }

    private Statement ParseWhileStatement() {
      try {
        loopDepth++;
        var whileParenthesisEnabled = false;
        if (Check(TokenType.LeftParenthesisPunctuation)) {
          whileParenthesisEnabled = true;
          Next();
        }
        Expression condition = ParseExpression();
        if (whileParenthesisEnabled) {
          Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after condition.");
        }
        Statement body = ParseStatement();
        return new Statement.While(condition, body);
      } finally {
        loopDepth--;
      }
    }

    private Statement ParseRepeatWhileStatement() {
      try {
        loopDepth++;
        var repeatWhileParenthesisEnabled = false;
        Statement body = ParseStatement();

        Consume((TokenType.Reserved, "while"), "Expect 'while' after statement.");

        if (Check(TokenType.LeftParenthesisPunctuation)) {
          repeatWhileParenthesisEnabled = true;
          Next();
        }
        Expression condition = ParseExpression();
        if (repeatWhileParenthesisEnabled) {
          Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after condition.");
        }
        var end = repeatWhileParenthesisEnabled ? "')'" : "condition";
        if (Match(TokenType.SemiColonPunctuation)) {}
        return new Statement.RepeatWhile(condition, body);
      } finally {
        loopDepth--;
      }
    }

    /* Expressions */

    private Expression ParseExpression() {
      return ParseAssignment();
    }

    private Expression ParseAssignment() {
      Expression expression = ParseConditional();
        if (Match((TokenType.Operator, "="))) {
        Token equals = PeekBack();
        Expression value = ParseAssignment();
        if (expression is Expression.Variable) {
          Token name = ((Expression.Variable)expression).Name;
          return new Expression.Assign(name, value);
        }
        throw Error(equals, "Invalid assignment target.");
      }
      return expression;
    }

    private Expression ParseConditional() {
      Expression expression = ParseOr();
      if (Match((TokenType.Operator, "?"))) {
        Expression thenBranch = ParseExpression();
        Consume(TokenType.ColonPunctuation, "Expect ':' after then branch of conditional expression.");
        Expression elseBranch = ParseConditional();
        expression = new Expression.Conditional(expression, thenBranch, elseBranch);
      }
      return expression;
    }

    private Expression ParseOr() {
      Expression expression = ParseAnd();
      while (Match((TokenType.Operator, "or"))) {
        Token @operator = PeekBack();
        Expression right = ParseAnd();
        expression = new Expression.Logical(expression, @operator, right);
      }
      return expression;
    }

    private Expression ParseAnd() {
      Expression expression = ParseEquality();
      while (Match((TokenType.Operator, "and"))) {
        Token @operator = PeekBack();
        Expression right = ParseEquality();
        expression = new Expression.Logical(expression, @operator, right);
      }
      return expression;
    }

    private Expression ParseEquality() {
      Expression expression = ParseComparison();
      while (Match((TokenType.Operator, "!="), (TokenType.Operator, "=="))) {
        Token @operator = PeekBack();
        Expression right = ParseComparison();
        expression = new Expression.Binary(expression, @operator, right);
      }
      return expression;
    }

    private Expression ParseComparison() {
      Expression expression = ParseTerm();
      while (Match(
        (TokenType.Operator, ">"), (TokenType.Operator, ">="),
        (TokenType.Operator, "<"), (TokenType.Operator, "<=")
      )) {
        Token @operator = PeekBack();
        Expression right = ParseTerm();
        expression = new Expression.Binary(expression, @operator, right);
      }
      return expression;
    }

    private Expression ParseTerm() {
      Expression expression = ParseFactor();
      while (Match(
        (TokenType.Operator, "+"), (TokenType.Operator, "-"),
        (TokenType.Operator, "..")
      )) {
        Token @operator = PeekBack();
        Expression right = ParseFactor();
        expression = new Expression.Binary(expression, @operator, right);
      }
      return expression;
    }

    private Expression ParseFactor() {
      Expression expression = ParseUnary();
      while (Match(
        (TokenType.Operator, "/"), (TokenType.Operator, "*")
      )) {
        Token @operator = PeekBack();
        Expression right = ParseUnary();
        return new Expression.Binary(expression, @operator, right);
      }
      return expression;
    }

    private Expression ParseUnary() {
      if (Match(
        (TokenType.Operator, "!"), (TokenType.Operator, "-"))) {
        Token @operator = PeekBack();
        Expression right = ParseUnary();
        return new Expression.Unary(@operator, right);
      }
      return ParseCall();
    }

    private Expression ParseCall () {
      Expression expression = ParsePrimary();
      while (true) {
        if (Match(TokenType.LeftParenthesisPunctuation)) {
          expression = FinishCall(expression);
        } else break;
      }
      return expression;
    }

    private Expression.Function ParseFuncExpression(string kind) {
      Consume(TokenType.LeftParenthesisPunctuation, string.Format("Expect '(' after {0} name.", kind));
      List<Token> parameters = new List<Token>();
      if (!Check(TokenType.RightParenthesisPunctuation)) {
        do {
          if (parameters.Count >= 10) {
            Error(Peek(), "Cannot have more than 10 parameters.");
          }
          parameters.Add(Consume(TokenType.Identifier, 
                                 string.Format("Expect parameter name but found {0}", Current())));
        } while (Match(TokenType.CommaPunctuation));
      }

      Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after parameters.");
      Consume(TokenType.LeftBracePunctuation, string.Format("Expect '{0}' before {1} body.", "{", kind));

      return new Expression.Function(parameters, ParseBlockStatement());
    }

    private Expression FinishCall(Expression callee) {
      List<Expression> arguments = new List<Expression>();
      if (!Check(TokenType.RightParenthesisPunctuation)) {
        do {
          if (arguments.Capacity >= 10) {
            Error(Peek(), "Cannot have more than 10 arguments.");
          }
          arguments.Add(ParseExpression());
        } while (Match(TokenType.CommaPunctuation));
      }
      Token parenthesis = Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after arguments.");
      return new Expression.Call(callee, parenthesis, arguments);

    }


    private Expression ParsePrimary() {
      if (Match((TokenType.BooleanLiteral, "false"))) return new Expression.Literal(false);
      if (Match((TokenType.BooleanLiteral, "true"))) return new Expression.Literal(true);
      if (Match(TokenType.NullLiteral)) return new Expression.Literal(null);
      if (Match((TokenType.Reserved, "func"))) return ParseFuncExpression("lambda function");
      if (Match(
        TokenType.IntegerLiteral, TokenType.FloatingPointLiteral,
        TokenType.StringLiteral, TokenType.CharacterLiteral
      )) {
        return new Expression.Literal(PeekBack().Literal);
      }

      if (Match(TokenType.Identifier)) {
        if (PeekBack().Equals(TokenType.IntegerLiteral) || PeekBack().Equals(TokenType.FloatingPointLiteral)) {
          throw Error(Current(), "Unexpected identifier after number literal.");
        }
        return new Expression.Variable(PeekBack());
      }

      if (Match(TokenType.LeftParenthesisPunctuation)) {
        Expression expression = ParseExpression();
        Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after expression.");
        return new Expression.Parenthesized(expression);
      }
      throw Error(Current(), "Expect expression.");
    }
  }
}


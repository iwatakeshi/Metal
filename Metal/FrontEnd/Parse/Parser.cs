using Metal.FrontEnd.Parse.Grammar;
using Metal.FrontEnd.Scan;
using System;
using System.Collections.Generic;

namespace Metal.FrontEnd.Parse {
  public class Parser {
    private class ParseError : Exception { }
    private List<Token> tokens;
    private int position = 0;
    private Scanner scanner;

    public bool IsAtEnd { get { return Current().Type == TokenType.EOF; } }

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


    private Token Current() {
      return tokens[position];
    }
    private Token Next() {
      if (scanner != null) {
        scanner.ScanSafeToken();
        tokens = scanner.Tokens;
      }
      if (!IsAtEnd) position++;
      return Previous();
    }
    private Token Previous() {
      return tokens[position - 1];
    }

    private void Synchronize() {
      Next();
      while (!IsAtEnd) {
        var type = Current().Type;
        var lexeme = Current().Lexeme;
        var types = new List<(TokenType, string)> {
          (TokenType.Reserved, "class"), (TokenType.Reserved, "fn"),
          (TokenType.Reserved, "var"), (TokenType.Reserved, "let"),
          (TokenType.Reserved, "for"), (TokenType.Reserved, "if"),
          (TokenType.Reserved, "while"), (TokenType.Reserved, "return"),
          (TokenType.Reserved, "print")
        };
        if (types.Contains((type, lexeme))) return;
        Next();
      }
    }

    private Statement ParseStatement() {

      // Parse for statement
      if (Match((TokenType.Reserved, "for"))) return ParseForStatement();

      // Parse if statement
      if (Match((TokenType.Reserved, "if"))) return ParseIfStatement();

      // Parse print statement
      if (Match((TokenType.Reserved, "print"))) {
        Consume(TokenType.LeftParenthesisPunctuation, "Expect '(' after print.");
        var print = ParsePrintStatement();
        Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after print.");
        Consume(TokenType.SemiColonPunctuation, "Expect ';' after expression.");
        return print;
      }

      // Parse while statement
      if (Match((TokenType.Reserved, "while"))) return ParseWhileStatement();

      // Parse block statement
      if (Match(TokenType.LeftBracePunctuation)) {
        return new Statement.Block(ParseBlockStatement());
      }

      // Parse expression statement
      return ParseExpressionStatement();
    }
    private Statement ParseExpressionStatement() {
      Expression expr = ParseExpression();
      Consume(TokenType.SemiColonPunctuation, "Expect ';' after expression.");
      return new Statement.Expr(expr);
    }
    private Statement ParsePrintStatement() {
      Expression value = ParseExpression();
      return new Statement.Print(value);
    }

    private List<Statement> ParseBlockStatement() {
      List<Statement> statements = new List<Statement>();
      while(!Check(TokenType.RightBracePunctuation) && !IsAtEnd) {
        statements.Add(ParseDeclaration());
      }
      Consume(TokenType.RightBracePunctuation, "Expect '}' after block.");
      return statements;
    }

    private Statement ParseDeclaration() {
      try {
        if (Match((TokenType.Reserved, "var"))) return ParseVarDeclaration();
        else return ParseStatement();
      } catch(ParseError error) {
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

      Consume(TokenType.SemiColonPunctuation, "Expect ';' after variable declaration.");
      return new Statement.Var(name, initializer);
    }

    private Statement ParseForStatement() {
      Consume(TokenType.LeftParenthesisPunctuation, "Expect '(' after 'for'.");
      Statement initializer;
      if (Match(TokenType.SemiColonPunctuation)) {
        initializer = null;
      } else if (Match((TokenType.Reserved, "var"))) {
        initializer = ParseVarDeclaration();
      } else {
        initializer = ParseExpressionStatement();
      }
      Expression condition = null;
      if(!Check(TokenType.SemiColonPunctuation)) {
        condition = ParseExpression();
      }
      Consume(TokenType.SemiColonPunctuation, "Expect ';' after loop condition.");

      Expression increment = null;
      if (!Check(TokenType.RightParenthesisPunctuation)) {
        increment = ParseExpression();
      }
      Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after for clauses.");

      Statement body = ParseStatement();

      if(increment != null) {
        body = new Statement.Block(new List<Statement> {
          body,
          new Statement.Expr(increment)
        });
      }

      if (condition == null) condition = new Expression.Literal(true);
      body = new Statement.While(condition, body);

      if(initializer != null) {
        body = new Statement.Block(new List<Statement> {
          initializer,
          body
        });
      }

      return body;
    }

    private Statement ParseIfStatement() {
      Consume(TokenType.LeftParenthesisPunctuation, "Expect '(' after 'if'.");
      Expression condition = ParseExpression();
      Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after 'if'.");
      Statement thenBranch = ParseStatement();
      Statement elseBranch = null;
      if(Match((TokenType.Reserved, "else"))) {
        elseBranch = ParseStatement();
      }
      return new Statement.If(condition, thenBranch, elseBranch);
    }

    private Statement ParseWhileStatement() {
      Consume(TokenType.LeftParenthesisPunctuation, "Expect '(' after 'while'.");
      Expression condition = ParseExpression();
      Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after condition.");
      Statement body = ParseStatement();
      return new Statement.While(condition, body);
    }

    private Expression ParseExpression() {
      return ParseAssignment();
    }

    private Expression ParseAssignment() {
      Expression expression = ParseOr();
      if (Match((TokenType.Operator, "="))) {
        Token equals = Previous();
        Expression value = ParseAssignment();
        if (expression is Expression.Variable) {
          Token name = ((Expression.Variable)expression).Name;
          return new Expression.Assign(name, value);
        }
        throw Error(equals, "Invalid assignment target.");
      }
      return expression;
    }

    private Expression ParseOr() {
      Expression expression = ParseAnd();
      while (Match((TokenType.Operator, "or"))) {
        Token @operator = Previous();
        Expression right = ParseAnd();
        expression = new Expression.Logical(expression, @operator, right);
      }
      return expression;
    }

    private Expression ParseAnd() {
      Expression expression = ParseEquality();
      while (Match((TokenType.Operator, "and"))) {
        Token @operator = Previous();
        Expression right = ParseEquality();
        expression = new Expression.Logical(expression, @operator, right);
      }
      return expression;
    }

    private Expression ParseEquality() {
      Expression expression = ParseComparison();
      while (Match((TokenType.Operator, "!="), (TokenType.Operator, "=="))) {
        Token @operator = Previous();
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
        Token @operator = Previous();
        Expression right = ParseTerm();
        expression = new Expression.Binary(expression, @operator, right);
      }
      return expression;
    }

    private Expression ParseTerm() {
      Expression expression = ParseFactor();
      while (Match(
        (TokenType.Operator, "+"), (TokenType.Operator, "-")
      )) {
        Token @operator = Previous();
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
        Token @operator = Previous();
        Expression right = ParseUnary();
        return new Expression.Binary(expression, @operator, right);
      }
      return expression;
    }

    private Expression ParseUnary() {
      if (Match((TokenType.Operator, "!"), (TokenType.Operator, "-"))) {
        Token @operator = Previous();
        Expression right = ParseUnary();
        return new Expression.Unary(@operator, right);
      }
      return ParsePrimary();
    }

    private Expression ParsePrimary() {
      if (Match((TokenType.BooleanLiteral, "false"))) return new Expression.Literal(false);
      if (Match((TokenType.BooleanLiteral, "true"))) return new Expression.Literal(true);
      if (Match(TokenType.NullLiteral)) return new Expression.Literal(null);

      if (Match(
        TokenType.IntegerLiteral, TokenType.FloatingPointLiteral,
        TokenType.StringLiteral, TokenType.CharacterLiteral
      )) {
        return new Expression.Literal(Previous().Literal);
      }

      if(Match(TokenType.Identifier)) {
        return new Expression.Variable(Previous());
      }

      if (Match(TokenType.LeftParenthesisPunctuation)) {
        Expression expression = ParseExpression();
        Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after expression.");
        return new Expression.Parenthesized(expression);
      }
      Console.WriteLine(Current());
      throw Error(Current(), "Expect expression.");
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
        if (Check((type, null))) {
          return true;
        }
      }
      return false;
    }

    private bool Check((TokenType, string) type) {
      if (IsAtEnd) return false;
      if (type.Item2 == null) {
        return type.Item1 == Current().Type;
      }

      return type.Item1 == Current().Type && type.Item2 == Current().Lexeme;
    }

    private Token Consume((TokenType, string) type, string message) {
      if (Check(type)) return Next();
      throw Error(Current(), message);

    }
    private Token Consume(TokenType type, string message) {
      if (Check((type, null))) return Next();
       throw Error(Current(), message);
    }

    private ParseError Error(Token token, string message) {
      Metal.Error(token, message);
      return new ParseError();
    }
  }
}


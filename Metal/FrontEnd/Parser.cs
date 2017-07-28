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

    /// <summary>
    /// Get the current token.
    /// </summary>
    /// <returns>The current token.</returns>
    private Token Current() {
      return tokens[position];
    }

    /// <summary>
    /// Advance to the next token.
    /// </summary>
    /// <returns>The current token.</returns>
    private Token Next() {
      if (scanner != null) {
        scanner.ScanSafeToken();
        tokens = scanner.Tokens;
      }
      if (!IsAtEnd) position++;
      return PeekBack();
    }

    /// <summary>
    /// Revert to the previous token.
    /// </summary>
    /// <returns>The current token.</returns>
    private Token Previous() {
      if (position <= 0) return tokens[0];
      if (position > 0) position--;
      return Peek();
    }

    /// <summary>
    /// Get the next token without advancing.
    /// </summary>
    /// <returns>The next token.</returns>
    private Token Peek() {
      return tokens[position + 1];
    }

    /// <summary>
    /// Get the next token without advancing.
    /// </summary>
    /// <returns>The next token.</returns>
    /// <param name="to">To.</param>
    private Token Peek(int to) {
      return (position + to) > tokens.Count ? Current() : tokens[position + to];
    }

    /// <summary>
    /// Get the previous token without reverting.
    /// </summary>
    /// <returns>The previous token.</returns>
    private Token PeekBack() {
      return tokens[position - 1];
    }

    /// <summary>
    /// Get the previous token without reverting.
    /// </summary>
    /// <returns>The previous token.</returns>
    /// <param name="to">To.</param>
    private Token PeekBack(int to) {
      return (position - to) > 0 ? tokens[position - to] : Current();
    }

    /// <summary>
    /// Match the specified types.
    /// </summary>
    /// <returns>The match.</returns>
    /// <param name="types">Types.</param>
    private bool Match(params (TokenType, string)[] types) {
      foreach (var type in types) {
        if (Check(type)) {
          Next();
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Match the specified types.
    /// </summary>
    /// <returns>The match.</returns>
    /// <param name="types">Types.</param>
    private bool Match(params TokenType[] types) {
      foreach (var type in types) {
        if (Check((type))) {
          Next();
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Check the specified types.
    /// </summary>
    /// <returns>The check.</returns>
    /// <param name="types">Types.</param>
    private bool Check(params TokenType[] types) {
      foreach (var type in types) {
        if (Check((type, null))) {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Check the specified type.
    /// </summary>
    /// <returns>The check.</returns>
    /// <param name="type">Type.</param>
    private bool Check((TokenType, string) type) {
      if (IsAtEnd) return false;
      if (type.Item2 == null) {
        return type.Item1 == Current().Type;
      }

      return type.Item1 == Current().Type && type.Item2 == Current().Lexeme;
    }

    /// <summary>
    /// Consume the specified type and message.
    /// </summary>
    /// <returns>The consume.</returns>
    /// <param name="type">Type.</param>
    /// <param name="message">Message.</param>
    private Token Consume((TokenType, string) type, string message) {
      if (Check(type)) return Next();
      throw Error(Current(), message);
    }
    /// <summary>
    /// Consume the specified type and message.
    /// </summary>
    /// <returns>The consume.</returns>
    /// <param name="type">Type.</param>
    /// <param name="message">Message.</param>
    private Token Consume(TokenType type, string message) {
      if (Check((type, null))) return Next();
      throw Error(Current(), message);
    }

    private ParseError Error(Token token, string message) {
      Metal.Error(token, message);
      return new ParseError();
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
          (TokenType.Reserved, "print"), (TokenType.Reserved, "repeat"),
        };
        if (types.Contains((type, lexeme))) return;
        Next();
      }
    }

    /// <summary>
    /// Parses the statement.
    /// </summary>
    /// <returns>The statement.</returns>
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

      // Parse repeat-while statement
      if (Match((TokenType.Reserved, "repeat"))) return ParseRepeatWhileStatement();

      // Parse block statement
      if (Match(TokenType.LeftBracePunctuation)) {
        return new Statement.Block(ParseBlockStatement());
      }

      // Parse expression statement
      return ParseExpressionStatement();
    }

    /// <summary>
    /// Parses the expression statement.
    /// </summary>
    /// <returns>The expression statement.</returns>
    private Statement ParseExpressionStatement() {
      Expression expr = ParseExpression();
      Consume(TokenType.SemiColonPunctuation, "Expect ';' after expression.");
      return new Statement.Expr(expr);
    }

    /// <summary>
    /// Parses the print statement.
    /// </summary>
    /// <returns>The print statement.</returns>
    private Statement ParsePrintStatement() {
      Expression value = ParseExpression();
      return new Statement.Print(value);
    }

    /// <summary>
    /// Parses the block statement.
    /// </summary>
    /// <returns>The block statement.</returns>
    private List<Statement> ParseBlockStatement() {
      List<Statement> statements = new List<Statement>();
      while (!Check(TokenType.RightBracePunctuation) && !IsAtEnd) {
        statements.Add(ParseDeclaration());
      }
      Consume(TokenType.RightBracePunctuation, "Expect '}' after block.");
      return statements;
    }

    /// <summary>
    /// Parses the declaration.
    /// </summary>
    /// <returns>The declaration.</returns>
    private Statement ParseDeclaration() {
      try {
        if (Match((TokenType.Reserved, "var"))) return ParseVarDeclaration();
        else return ParseStatement();
      } catch (ParseError) {
        Synchronize();
        return null;
      }

    }

    /// <summary>
    /// Parses the variable declaration.
    /// </summary>
    /// <returns>The variable declaration.</returns>
    private Statement ParseVarDeclaration() {
      Token name = Consume(TokenType.Identifier, "Expect variable name.");
      Expression initializer = null;
      if (Match((TokenType.Operator, "="))) {
        initializer = ParseExpression();
      }

      Consume(TokenType.SemiColonPunctuation, "Expect ';' after variable declaration.");
      return new Statement.Var(name, initializer);
    }

    /// <summary>
    /// Parses if statement.
    /// </summary>
    /// <returns>The if statement.</returns>
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

    /// <summary>
    /// Parses for statement.
    /// </summary>
    /// <returns>The for statement.</returns>
    private Statement ParseForStatement() {
      bool forParenthesisEnabled = false;

      if (Check(TokenType.LeftParenthesisPunctuation)) {
        forParenthesisEnabled = true;
        Next();
      }

      var end = forParenthesisEnabled ? "'('" : "'for'";

      Consume((TokenType.Reserved, "var"), "Expect 'var' after " + end + ".");

      Token name = Consume(TokenType.Identifier, "Expect variable name.");

      Consume((TokenType.Operator, "in"), "Expect 'in' after variable name.");

      Expression range = ParseExpression();

      if (forParenthesisEnabled) {
        Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after for clauses.");
      }

      return new Statement.For(name, range, ParseStatement());
    }

    /// <summary>
    /// Parses the while statement.
    /// </summary>
    /// <returns>The while statement.</returns>
    private Statement ParseWhileStatement() {
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
    }

    /// <summary>
    /// Parses the repeat while statement.
    /// </summary>
    /// <returns>The repeat while statement.</returns>
    private Statement ParseRepeatWhileStatement() {
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
      Consume(TokenType.SemiColonPunctuation, "Expect ';' after " + end + ".");
      return new Statement.RepeatWhile(condition, body);
    }

    /// <summary>
    /// Parses the expression.
    /// </summary>
    /// <returns>The expression.</returns>
    private Expression ParseExpression() {
      return ParseAssignmentExpression();
    }

    /// <summary>
    /// Parses the assignment.
    /// </summary>
    /// <returns>The assignment expression.</returns>
    private Expression ParseAssignmentExpression() {
      Expression expression = ParseOrExpression();
      if (Match((TokenType.Operator, "="))) {
        Token equals = PeekBack();
        Expression value = ParseAssignmentExpression();
        if (expression is Expression.Variable) {
          Token name = ((Expression.Variable)expression).Name;
          return new Expression.Assign(name, value);
        }
        throw Error(equals, "Invalid assignment target.");
      }
      return expression;
    }

    /// <summary>
    /// Parses the or.
    /// </summary>
    /// <returns>The 'or' expression.</returns>
    private Expression ParseOrExpression() {
      Expression expression = ParseAnd();
      while (Match((TokenType.Operator, "or"))) {
        Token @operator = PeekBack();
        Expression right = ParseAnd();
        expression = new Expression.Logical(expression, @operator, right);
      }
      return expression;
    }

    /// <summary>
    /// Parses the and.
    /// </summary>
    /// <returns>The 'and' expression.</returns>
    private Expression ParseAnd() {
      Expression expression = ParseEqualityExpression();
      while (Match((TokenType.Operator, "and"))) {
        Token @operator = PeekBack();
        Expression right = ParseEqualityExpression();
        expression = new Expression.Logical(expression, @operator, right);
      }
      return expression;
    }

    /// <summary>
    /// Parses the equality.
    /// </summary>
    /// <returns>The equality expression.</returns>
    private Expression ParseEqualityExpression() {
      Expression expression = ParseComparisonExpression();
      while (Match((TokenType.Operator, "!="), (TokenType.Operator, "=="))) {
        Token @operator = PeekBack();
        Expression right = ParseComparisonExpression();
        expression = new Expression.Binary(expression, @operator, right);
      }
      return expression;
    }

    /// <summary>
    /// Parses the comparison.
    /// </summary>
    /// <returns>The comparison expression.</returns>
    private Expression ParseComparisonExpression() {
      Expression expression = ParseTermExpression();
      while (Match(
        (TokenType.Operator, ">"), (TokenType.Operator, ">="),
        (TokenType.Operator, "<"), (TokenType.Operator, "<=")
      )) {
        Token @operator = PeekBack();
        Expression right = ParseTermExpression();
        expression = new Expression.Binary(expression, @operator, right);
      }
      return expression;
    }

    /// <summary>
    /// Parses the term expression.
    /// </summary>
    /// <returns>The term expression.</returns>
    private Expression ParseTermExpression() {
      Expression expression = ParseFactorExpression();
      while (Match(
        (TokenType.Operator, "+"), (TokenType.Operator, "-"),
        (TokenType.Operator, "..")
      )) {
        Token @operator = PeekBack();
        Expression right = ParseFactorExpression();
        expression = new Expression.Binary(expression, @operator, right);
      }
      return expression;
    }

    /// <summary>
    /// Parses the factor expression.
    /// </summary>
    /// <returns>The factor expression.</returns>
    private Expression ParseFactorExpression() {
      Expression expression = ParseUnaryExpression();
      while (Match(
        (TokenType.Operator, "/"), (TokenType.Operator, "*")
      )) {
        Token @operator = PeekBack();
        Expression right = ParseUnaryExpression();
        return new Expression.Binary(expression, @operator, right);
      }
      return expression;
    }

    /// <summary>
    /// Parses the unary expression.
    /// </summary>
    /// <returns>The unary expression.</returns>
    private Expression ParseUnaryExpression() {
      if (Match(
        (TokenType.Operator, "!"), (TokenType.Operator, "-"),
        (TokenType.Operator, ".."))) {
        Token @operator = PeekBack();
        Expression right = ParseUnaryExpression();
        return new Expression.Unary(@operator, right);
      }
      return ParsePrimaryExpression();
    }

    /// <summary>
    /// Parses the primary expression.
    /// </summary>
    /// <returns>The primary expression.</returns>
    private Expression ParsePrimaryExpression() {
      if (Match((TokenType.BooleanLiteral, "false"))) return new Expression.Literal(false);
      if (Match((TokenType.BooleanLiteral, "true"))) return new Expression.Literal(true);
      if (Match(TokenType.NullLiteral)) return new Expression.Literal(null);

      if (Match(
        TokenType.IntegerLiteral, TokenType.FloatingPointLiteral,
        TokenType.StringLiteral, TokenType.CharacterLiteral
      )) {
        return new Expression.Literal(PeekBack().Literal);
      }

      if (Match(TokenType.Identifier)) {
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


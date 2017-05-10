using Metal.FrontEnd.Parse.Grammar;
using Metal.FrontEnd.Scan;
using System;
using System.Collections.Generic;
#if WINDOWS
using System.ValueType;
#endif
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

    public Expression Parse() {
      if (scanner != null) {
        scanner.ScanSafeToken();
        tokens = scanner.Tokens;
      }
      try {
        return ParseExpression();
      } catch (ParseError error) {
        return null;
      }
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
      return tokens[position];
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
          (TokenType.Reserved, "while"), (TokenType.Reserved, "return")
        };
        if (types.Contains((type, lexeme))) return;
        Next();
      }
    }

    private Expression ParseExpression() {
      return ParseEquality();
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
      if (Match((TokenType.Operator, "?"), (TokenType.Operator, "-"))) {
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
      if (Match(TokenType.LeftParenthesisPunctuation)) {
        Expression expression = ParseExpression();
        Consume(TokenType.RightParenthesisPunctuation, "Expect ')' after expression.");
        return new Expression.Parenthesized(expression);
      }
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
        if (Check((type, null))) {
          Next();
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


using Metal.FrontEnd.Parse.Grammar;
using Metal.FrontEnd.Scan;
using System;
using System.Collections.Generic;
using System.ValueType;

namespace Metal.FrontEnd.Parse {
  public class Parser {
    private class ParseError : Exception { }
    private List<Token> tokens;
    private int position = 0;
    private Scanner scanner;
    private Token currentToken;

    public Parser (Scanner scanner) {
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
      } catch(ParseError error) {
        return null;
      }
    }

    private Token Current() {
      return tokens[position];
    }
    private Token Next() {
      if(scanner != null) {
        scanner.ScanSafeToken();
        tokens = scanner.Tokens;
      }
      position++;
      return tokens [position];
    }

    private Expression ParseExpression() {
      return ParseEquality();
    }

    private Expression ParseEquality() {
      Expression expression = ParseComparison();
      while (Match((TokenType.Operator, "!="))) {

      }
      return null;
    }

    private Expression ParseComparison() {
      throw new NotImplementedException();
    }

    private bool Match(params (TokenType, string)[] types) {
      foreach(var type in types) {
        if(Check(type)) {
          Next();
          return true;
        }
      }
      return false;
    }
  }
}


package metal.frontend.scanner;

import java.text.*;
import java.util.HashMap;
import java.util.Map;

import static metal.frontend.scanner.TokenType.*;
import metal.frontend.scanner.Token;

public class Token {
  public final TokenType type;
  public final String lexeme;
  public final Object literal;
  public final int line;
  public final int column;

  private static final Map<String, TokenType> reserved;
  private static final Map<String, TokenType> operators;
  private static final Map<String, TokenType> punctuations;

  static {
    reserved = new HashMap<>();
    reserved.put("and", Reserved);
    reserved.put("class", Reserved);
    reserved.put("else", Reserved);
    reserved.put("false", Reserved);
    reserved.put("for", Reserved);
    reserved.put("fun", Reserved);
    reserved.put("if", Reserved);
    reserved.put("nil", Reserved);
    reserved.put("or", Reserved);
    reserved.put("print", Reserved);
    reserved.put("return", Reserved);
    reserved.put("super", Reserved);
    reserved.put("this", Reserved);
    reserved.put("true", Reserved);
    reserved.put("var", Reserved);
    reserved.put("while", Reserved);

    operators = new HashMap<>();
    operators.put("+", Operator);
    operators.put("-", Operator);
    operators.put("*", Operator);
    operators.put("/", Operator);
    operators.put("!", Operator);
    operators.put("=", Operator);
    operators.put("<", Operator);
    operators.put(">", Operator);
    operators.put("+=", Operator);
    operators.put("-=", Operator);
    operators.put("*=", Operator);
    operators.put("/=", Operator);
    operators.put("!=", Operator);
    operators.put("==", Operator);
    operators.put("<=", Operator);
    operators.put(">=", Operator);
    operators.put(".", Operator);

    punctuations = new HashMap<>();
    punctuations.put("(", Punctuation);
    punctuations.put(")", Punctuation);
    punctuations.put("{", Punctuation);
    punctuations.put("}", Punctuation);
    punctuations.put(",", Punctuation);
    punctuations.put(";", Punctuation);
  }

  public Token(TokenType type, String lexeme, Object literal, int line, int column) {
    this.type = type;
    this.lexeme = lexeme;
    this.literal = literal;
    this.line = line;
    this.column = column;
  }

  public String toString() {
    return "Type: " + type + ", Lexeme: " + lexeme + ", Value: " + literal + ", Line: " + line + ", Column: " + column;
  }

  public Boolean hasType(TokenType type) {
    return this.type == type;
  }

  public Boolean hasType(TokenType type, String lexeme) {
    return this.type == type && this.lexeme == lexeme;
  }

  public Boolean hasTypes(TokenType... types) {
    for(var type : types) {
      if (this.type != type) {
        return false;
      }
    }
    return true;
  }

  @SafeVarargs
  public final Boolean hasTypes(Map<TokenType, String>... types) {
    for(var type : types) {
      if (type.get(this.type) != this.lexeme) {
        return false;
      }
    }
    return true;
  }

  public static Boolean isReserved(String lexeme) {
    return reserved.get(lexeme) != null;
  }

  public static Boolean isOperator(String lexeme) {
    return operators.get(lexeme) != null;
  }

  public static Boolean isPunctuation(String lexeme) {
    return punctuations.get(lexeme) != null;
  }

  public static Boolean isBooleanLiteral(String lexeme) {
    return lexeme == "true" || lexeme == "false";
  }

  public static Boolean isNullLiteral(String lexeme) {
    return lexeme == "null";
  }

  public static Boolean isNumberLiteral(String lexeme) {
    DecimalFormat decimalFormat = new DecimalFormat("#");
    try {
      decimalFormat.parse(lexeme).doubleValue();
      return true;
   } catch (ParseException e) {
      return false;
   }
  }

  public static Boolean isStringLiteral(String literal) {
    return (literal.charAt(0) == '"' && literal.charAt(literal.length() - 1) == '"') ||
    (literal.charAt(0) == '\'' && literal.charAt(literal.length() - 1) == '\'');
  }

  public static TokenType getTokenType(String lexeme) {
    if (isReserved(lexeme)) {
      return TokenType.Reserved;
    }

    if (isOperator(lexeme)) {
      return TokenType.Operator;
    }

    if(isPunctuation(lexeme)) {
      return TokenType.Punctuation;
    }

    return TokenType.OtherType;
  }
}
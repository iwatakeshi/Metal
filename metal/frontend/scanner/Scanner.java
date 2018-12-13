package metal.frontend.scanner;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import static metal.frontend.scanner.TokenType.*;
import metal.frontend.scanner.Token;
import metal.Metal;

public class Scanner {
  private final String source;
  private final List<Token> tokens = new ArrayList<>();

  private int start = 0;
  private int position = 0;
  private int line = 1;
  private int column = 1;

  public Scanner(String source) {
    this.source = source;
  }

  private static final Map<String, TokenType> keywords;

  static {                                             
    keywords = new HashMap<>();                        
    keywords.put("and",    Reserved);                       
    keywords.put("class",  Reserved);                     
    keywords.put("else",   Reserved);                      
    keywords.put("false",  Reserved);                     
    keywords.put("for",    Reserved);                       
    keywords.put("fun",    Reserved);                       
    keywords.put("if",     Reserved);                        
    keywords.put("nil",    Reserved);                       
    keywords.put("or",     Reserved);                        
    keywords.put("print",  Reserved);                     
    keywords.put("return", Reserved);                    
    keywords.put("super",  Reserved);                     
    keywords.put("this",   Reserved);                      
    keywords.put("true",   Reserved);                      
    keywords.put("var",    Reserved);                       
    keywords.put("while",  Reserved);                     
  } 


  public List<Token> scanTokens() {
    while (!isAtEnd()) {
      // We are at the beginning of the next lexeme.
      start = position;
      scanToken();
    }

    tokens.add(new Token(EOF, "", null, line));
    return tokens;
  }

  private boolean isAtEnd() {
    return position >= source.length();
  }

  private void scanToken() {
    char c = next();
    switch (c) {
    case '(':
    case ')':
    case '{':
    case '}':
    case ',':
    case '.':
    case ';':
      addToken(Punctuation);
      break;
    case '-':
    case '+':
    case '*':
    case '!':
      if (match('='))
        addToken(Operator);
      var list = new ArrayList<TokenType>();
      list.add(Operator);
      list.add(Punctuation);
      addToken(list);
      break;
    case '=':
    case '<':
    case '>':
      if (match('='))
        addToken(Operator);
    case '/':
      if (match('/')) {
        // A comment goes until the end of the line.
        while (current() != '\n' && !isAtEnd())
          next();
      } else {
        addToken(Operator);
      }
      break;
    case '"':
      string();
      break;
      case 'o':
      if (peek() == 'r') {
        addToken(Reserved);
      }
      break;
    default:
      if (isDigit(c)) {
        number();
      } else if (isAlpha(c)) {                   
        identifier(); 
      } else {
        Metal.error(line, "Unexpected character.");
      }
      break;
    }
  }

  private void string() {
    while (current() != '"' && !isAtEnd()) {
      if (current() == '\n')
        line++;
      next();
    }

    // Unterminated string.
    if (isAtEnd()) {
      Metal.error(line, "Unterminated string.");
      return;
    }

    // The closing ".
    next();

    // Trim the surrounding quotes.
    String value = source.substring(start + 1, position - 1);
    addToken(StringLiteral, value);
  }

  private void number() {                                     
    while (isDigit(current())) next();

    // Look for a fractional part.                            
    if (current() == '.' && isDigit(peek())) {               
      // Consume the "."                                      
      next();                                              

      while (isDigit(current())) next();                      
    }                                                         

    addToken(NumberLiteral,                                          
        Double.parseDouble(source.substring(start, position)));
  }

  private void identifier() {                
    while (isAlphaNumeric(peek())) next();

    // See if the identifier is a reserved word.   
    String text = source.substring(start, position);

    TokenType type = keywords.get(text);           
    if (type == null) type = Identifier;           
    addToken(type);                  
  }  

  private boolean match(char expected) {
    if (isAtEnd())
      return false;
    if (source.charAt(position) != expected)
      return false;

    position++;
    return true;
  }

  private char current() {
    if (isAtEnd())
      return '\0';
    return source.charAt(position);
  }

  private char next() {
    position++;
    return source.charAt(position - 1);
  }

  private char peek() {                         
    if (position + 1 >= source.length()) return '\0';
    return source.charAt(position + 1);              
  } 


  private boolean isDigit(char c) {
    return c >= '0' && c <= '9';   
  }

  private boolean isAlpha(char c) {       
    return (c >= 'a' && c <= 'z') ||      
           (c >= 'A' && c <= 'Z') ||      
            c == '_';                     
  }

  private boolean isAlphaNumeric(char c) {
    return isAlpha(c) || isDigit(c);      
  }

  private void addToken(TokenType type) {
    addToken(type, null);
  }

  private void addToken(ArrayList<TokenType> types) {
    addToken(types, null);
  }

  private void addToken(TokenType type, Object literal) {
    String lexeme = source.substring(start, position);
    tokens.add(new Token(type, lexeme, literal, line));
  }

  private void addToken(ArrayList<TokenType> types, Object literal) {
    String lexeme = source.substring(start, position);
    tokens.add(new Token(types, lexeme, literal, line));
  }

}
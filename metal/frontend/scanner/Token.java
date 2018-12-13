package metal.frontend.scanner;

import java.util.ArrayList;

import metal.frontend.scanner.Token;

public class Token {                                                     
  public final TokenType type;
  public final ArrayList<TokenType> types;                                           
  public final String lexeme;                                           
  public final Object literal;                                           
  public final int line; 
  public final int column;

  public Token(TokenType type, String lexeme, Object literal, int line, int column) {
    this.type = type;                                             
    this.lexeme = lexeme;                                         
    this.literal = literal;                                       
    this.line = line;
    this.column = column;
    this.types = new ArrayList<>();                                            
  }

  public String toString() {                                      
    return "Type: " + type + ", Lexeme: " + lexeme + ", Value: " + literal + ", Line: " + line + ", Column: " + column;                   
  }
}                                                                 
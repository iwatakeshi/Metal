package metal.frontend.scanner;

import java.util.ArrayList;

import metal.frontend.scanner.Token;

public class Token {                                                     
  final TokenType type;
  final ArrayList<TokenType> types;                                           
  final String lexeme;                                           
  final Object literal;                                           
  final int line; 
  final int column;

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
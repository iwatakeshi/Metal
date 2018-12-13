package metal.frontend.scanner;

import java.util.ArrayList;

import metal.frontend.scanner.Token;

public class Token {                                                     
  final TokenType type;
  final ArrayList<TokenType> types;                                           
  final String lexeme;                                           
  final Object literal;                                           
  final int line; 

  public Token(TokenType type, String lexeme, Object literal, int line) {
    this.type = type;                                             
    this.lexeme = lexeme;                                         
    this.literal = literal;                                       
    this.line = line;
    this.types = new ArrayList<>();                                             
  }
  
  public Token(ArrayList<TokenType> types, String lexeme, Object literal, int line) {
    this.types = types;
    this.lexeme = lexeme;
    this.literal = literal;
    this.line = line;
    this.type = TokenType.None;
  }

  public String toString() {                                      
    return type + " " + lexeme + " " + literal;                   
  }
}                                                                 
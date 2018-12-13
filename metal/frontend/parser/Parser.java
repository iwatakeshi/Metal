package metal.frontend.parser;

import java.util.List;                                 

import static metal.frontend.scanner.TokenType.*;
import metal.frontend.scanner.Token;
import metal.frontend.parser.grammar.*;

class Parser {                                         
  private final List<Token> tokens;                    
  private int current = 0;                             

  Parser(List<Token> tokens) {                         
    this.tokens = tokens;                              
  } 
  private Expression expression() {
    return equality();       
  }
  private Expression equality() {                         
    Expression expression = comparison();

    while (match('!=', '==')) {        
      Token operator = previous();                  
      Expression right = comparison();                    
      expression = new Expression.Binary(expression, operator, right);
    }                                               

    return expression;                                    
  } 

  private Expression comparison() {                                
    Expression expression = addition();

    while (match('>', '>=', '<', '<=')) {
      Token operator = previous();                           
      Expression right = addition();                               
      expression = new Expression.Binary(expression, operator, right);         
    }                                                        

    return expression;                                             
  } 
  
  private Expression addition() {                         
    Expression expression = multiplication();

    while (match('-', '+')) {                    
      Token operator = previous();                  
      Expression right = multiplication();                
      expression = new Expression.Binary(expression, operator, right);
    }                                               

    return expression;                                    
  }                                                 

  private Expression multiplication() {                   
    Expression expression = unary();                            

    while (match('/', '*')) {                    
      Token operator = previous();                  
      Expression right = unary();                         
      expression = new Expression.Binary(expression, operator, right);
    }                                               

    return expression;                                    
  } 

  private Expression unary() {                     
    if (match('!', '-')) {                
      Token operator = previous();           
      Expression right = unary();                  
      return new Expression.Unary(operator, right);
    }

    return primary();                        
  } 

  private Expression primary() {                                 
    if (match('FALSE')) return new Expression.Literal(false);      
    if (match('TRUE')) return new Expression.Literal(true);        
    if (match('NIL')) return new Expression.Literal(null);

    if (match(NumberLiteral, StringLiteral)) {                           
      return new Expression.Literal(previous().literal);         
    }                                                      

    if (match('(') {                               
      Expression expression = expression();                            
      consume(')', "Expect ')' after expression.");
      return new Expression.Group(expression);                      
    }                                                      
  } 

  private boolean match(TokenType... types) {
    for (TokenType type : types) {           
      if (check(type)) {                     
        advance();                           
        return true;                         
      }                                      
    }

    return false;                            
  } 
  private boolean check(TokenType type) {
    if (isAtEnd()) return false;         
    return peek().type == type;          
  } 
  private Token advance() {   
    if (!isAtEnd()) current++;
    return previous();        
  } 
  private boolean isAtEnd() {      
    return peek().type == EOF;     
  }

  private Token peek() {           
    return tokens.get(current);    
  }                                

  private Token previous() {       
    return tokens.get(current - 1);
  }                                                   
} 
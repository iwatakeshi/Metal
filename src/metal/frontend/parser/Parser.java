package metal.frontend.parser;

import java.util.List;

import static metal.frontend.scanner.TokenType.*;
import metal.frontend.scanner.TokenType;
import metal.frontend.scanner.Token;
import metal.frontend.parser.grammar.*;
import metal.Metal;


class Parser {

  private static class ParseError extends RuntimeException {

    private static final long serialVersionUID = 1L;
  }

  private final List<Token> tokens;
  private int current = 0;

  Parser(List<Token> tokens) {
    this.tokens = tokens;
  }

  Expression parse() {                
    try {                       
      return expression();      
    } catch (ParseError error) {
      return null;              
    }                           
  } 

/*
 * expression → equality
 */
  private Expression expression() {
    return equality();
  }

/*
 * equality → comparison ( ( "!=" | "==" ) comparison )* zero or more
 */
  private Expression equality() {
    Expression expression = comparison();

    while (match("!=", "==")) {
      Token operator = previous();
      Expression right = comparison();
      expression = new Expression.Binary(expression, operator, right);
    }

    return expression;
  }

/*
 * comparison → addition ( ( ">" | ">=" | "<" | "<=" ) addition )* zero or more
 */
  private Expression comparison() {
    Expression expression = addition();

    while (match(">", ">=", "<", "<=")) {
      Token operator = previous();
      Expression right = addition();
      expression = new Expression.Binary(expression, operator, right);
    }

    return expression;
  }

/*
 * addition → multiplication ( ( "-" | "+" ) multiplication )* zero or more
 */
  private Expression addition() {
    Expression expression = multiplication();

    while (match("-", "+")) {
      Token operator = previous();
      Expression right = multiplication();
      expression = new Expression.Binary(expression, operator, right);
    }

    return expression;
  }

/*
 * multiplication → unary ( ( "/" | "*" ) unary )* zero or more
 */
  private Expression multiplication() {
    Expression expression = unary();

    while (match("/", "*")) {
      Token operator = previous();
      Expression right = unary();
      expression = new Expression.Binary(expression, operator, right);
    }

    return expression;
  }

/*
 * unary → ( "!" | "-" ) unary | primary ;
 */
  private Expression unary() {
    if (match("!", "-")) {
      Token operator = previous();
      Expression right = unary();
      return new Expression.Unary(operator, right);
    }

    return primary();
  }

/*
 * primary → NUMBER | STRING | "false" | "true" | "nil" | "(" expression ")" ;
 */
  private Expression primary() {
    if (match(BooleanLiteral))
      return new Expression.Literal(Boolean.parseBoolean(previous().lexeme));
    if (match(NullLiteral))
      return new Expression.Literal(null);

    if (match(NumberLiteral, StringLiteral)) {
      return new Expression.Literal(previous().literal);
    }

    if (match("(")) {
      Expression expression = expression();
      consume(")", "Expect ')' after expression.");
      return new Expression.Group(expression);
    }

    throw error(peek(), "Expect expression."); 
  }

  //Needed? match(TokenType... types)
  private boolean match(TokenType... types) {
    for (TokenType type : types) {
      if (check(type)) {
        advance();
        return true;
      }
    }

    return false;
  }

  private boolean match(String... types) {
    for (String type : types) {
      if (check(type)) {
        advance();
        return true;
      }
    }
    return false;
  }

  // private Token consume(TokenType type, String message) {
  //   if (check(type))
  //     return advance();

  //   throw error(peek(), message);
  // }

  private Token consume(String type, String message) {
    if(check(type)) {
      return advance();
    }
    throw error(peek(), message);
  }

  //Needed? check(TokenType type)
  private boolean check(TokenType type) {
    if (isAtEnd())
      return false;
    return peek().type == type;
  }

  private boolean check(String type) {
    if (isAtEnd())
      return false;
    return peek().type == Token.getTokenType(type);
  }

  private Token advance() {
    if (!isAtEnd())
      current++;
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

  private ParseError error(Token token, String message) {
    Metal.error(token, message);                           
    return new ParseError();                             
  }

/*
*  Looking for next statement
*  for,if,return,var,etc...
*/
//don't know where synchronize() is being called from TT  
  private void synchronize() {                 
    advance();

    while (!isAtEnd()) {                       
      if (previous().type == ";") return;

      switch (peek().type) {                   
        case CLASS:                            
        case FUN:                              
        case VAR:                              
        case FOR:                              
        case IF:                               
        case WHILE:                            
        case PRINT:                            
        case RETURN:                           
          return;                              
      }                                        

      advance();                               
    }                                          
  } 
}//end Parser Class

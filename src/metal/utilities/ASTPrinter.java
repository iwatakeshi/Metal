package metal.utilities;

import metal.frontend.scanner.Token;
import metal.frontend.scanner.TokenType;
import metal.frontend.parser.grammar.Expression;
// Creates an unambiguous, if ugly, string representation of AST nodes.
public class ASTPrinter implements Expression.Visitor<String> {                     
  public String print(Expression expression) {                                            
    return expression.accept(this);                                          
  } 
  @Override                                                          
  public String visitBinaryExpression(Expression.Binary expression) {                  
    return parenthesize(expression.operator.lexeme, expression.left, expression.right);
  }

  @Override                                                          
  public String visitGroupExpression(Expression.Group expression) {              
    return parenthesize("group", expression.expression);                   
  }                                                                  

  @Override                                                          
  public String visitLiteralExpression(Expression.Literal expression) {                
    if (expression.value == null) return "nil";                            
    return expression.value.toString();                                    
  }                                                                  

  @Override                                                          
  public String visitUnaryExpression(Expression.Unary expression) {                    
    return parenthesize(expression.operator.lexeme, expression.right);           
  } 
  
  private String parenthesize(String name, Expression... expressions) {
    StringBuilder builder = new StringBuilder();

    builder.append("(").append(name);                      
    for (Expression expression : expressions) {                              
      builder.append(" ");                                 
      builder.append(expression.accept(this));                   
    }                                                      
    builder.append(")");                                   

    return builder.toString();                             
  } 

  public static void main(String[] args) {                 
    Expression expression = new Expression.Binary(                     
        new Expression.Unary(                                    
            new Token(TokenType.Operator, "-", null, 1, 2),      
            new Expression.Literal(123)),                        
        new Token(TokenType.Operator, "*", null, 1, 2),           
        new Expression.Group(                                 
            new Expression.Literal(45.67)));

    System.out.println(new ASTPrinter().print(expression));
  } 
}                                                                    
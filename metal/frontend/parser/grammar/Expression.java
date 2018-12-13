package metal.frontend.parser.grammar;

import java.util.List;
import metal.frontend.scanner.Token;

public abstract class Expression {
 public interface Visitor<R> {
    R visitBinaryExpression(Binary expression);
    R visitGroupExpression(Group expression);
    R visitLiteralExpression(Literal expression);
    R visitUnaryExpression(Unary expression);
  }

 public static class Binary extends Expression {
  public Binary(Expression left, Token operator, Expression right) {
      this.left = left;
      this.operator = operator;
      this.right = right;
    }

  public <R> R accept(Visitor<R> visitor) {
      return visitor.visitBinaryExpression(this);
  }

    public final Expression left;
    public final Token operator;
    public final Expression right;
  }

 public static class Group extends Expression {
  public Group(Expression expression) {
      this.expression = expression;
    }

  public <R> R accept(Visitor<R> visitor) {
      return visitor.visitGroupExpression(this);
  }

    public final Expression expression;
  }

 public static class Literal extends Expression {
  public Literal(Object value) {
      this.value = value;
    }

  public <R> R accept(Visitor<R> visitor) {
      return visitor.visitLiteralExpression(this);
  }

    public final Object value;
  }

 public static class Unary extends Expression {
  public Unary(Token operator, Expression right) {
      this.operator = operator;
      this.right = right;
    }

  public <R> R accept(Visitor<R> visitor) {
      return visitor.visitUnaryExpression(this);
  }

    public final Token operator;
    public final Expression right;
  }

 public abstract <R> R accept(Visitor<R> visitor);
}

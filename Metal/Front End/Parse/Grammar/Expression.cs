using Metal.FrontEnd.Scan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metal.FrontEnd.Parse.Grammar {
  public abstract class Expression {

    public interface Visitor<T> {
      T visitLiteralExpression(LiteralExpression expression);
      T visitUnaryExpression(UnaryExpression expression);
      T visitBinaryExpression(BinaryExpression expression);
      T visitParenthesizedExpression(ParenthesizedExpression expression);
    }
    public abstract T Accept<T>(Visitor<T> visitor);

    public class UnaryExpression : Expression {
      Expression left;
      Token @operator;
      UnaryExpression(Expression left, Token @operator) {
        this.left = left;
        this.@operator = @operator;
      }
      public override T Accept<T>(Visitor<T> visitor) {
        return visitor.visitUnaryExpression(this);
      }
    }
 

    public class BinaryExpression : Expression {
      Expression left;
      Token @operator;
      Expression right;
      BinaryExpression(Expression left, Token @operator, Expression right) {
        this.left = left;
        this.@operator = @operator;
        this.right = right;
      }

      public override T Accept<T>(Visitor<T> visitor) {
        return visitor.visitBinaryExpression(this);
      }
    }
    public class ParenthesizedExpression : Expression {
      Expression expression;
      ParenthesizedExpression(Expression expression) {
        this.expression = expression;
      }
      public override T Accept<T>(Visitor<T> visitor) {
        return visitor.visitParenthesizedExpression(this);
      }
    }

    public class LiteralExpression : Expression {
      object value;
      LiteralExpression(object value) {
        this.value = value;
      }

      public override T Accept<T>(Visitor<T> visitor) {
        return visitor.visitLiteralExpression(this);
      }

    }
  }
}

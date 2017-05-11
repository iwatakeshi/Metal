using Metal.FrontEnd.Scan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metal.FrontEnd.Parse.Grammar {
  public abstract class Expression {

    public interface Visitor<T> {
      T VisitLiteral(Literal expression);
      T VisitUnary(Unary expression);
      T VisitBinary(Binary expression);
      T VisitParenthesized(Parenthesized expression);
    }
    public abstract T Accept<T>(Visitor<T> visitor);
    public abstract Expression Right { get; }
    public abstract Expression Left { get; }
    public abstract Token Operator { get; }
    public class Unary : Expression {
      Token @operator;
      Expression right;
      public Unary(Token @operator, Expression right) {
        this.@operator = @operator;
        this.right = right;
      }
      public override T Accept<T>(Visitor<T> visitor) {
        return visitor.VisitUnary(this);
      }
      public override Expression Left { get { return null; } }
      public override Expression Right { get { return right; } }
      public override Token Operator { get { return @operator; } }
    }


    public class Binary : Expression {
      private Expression left;
      private Token @operator;
      private Expression right;
      public Binary(Expression left, Token @operator, Expression right) {
        this.left = left;
        this.@operator = @operator;
        this.right = right;
      }

      public override T Accept<T>(Visitor<T> visitor) {
        return visitor.VisitBinary(this);
      }

      public override Expression Left { get { return left; } }
      public override Expression Right { get { return right; } }
      public override Token Operator { get { return @operator; } }
    }
    public class Parenthesized : Expression {
      private Expression expression;
      public Parenthesized(Expression expression) {
        this.expression = expression;
      }
      public override T Accept<T>(Visitor<T> visitor) {
        return visitor.VisitParenthesized(this);
      }
      
      public override Expression Left { get { return null; } }
      public Expression Center { get { return expression; } }
      public override Expression Right { get { return null; } }
      public override Token Operator { get { return null; } }
    }

    public class Literal : Expression {
      private object value;
      public Literal(object value) {
        this.value = value;
      }

      public override T Accept<T>(Visitor<T> visitor) {
        return visitor.VisitLiteral(this);
      }
      public override Expression Left { get { return null; } }
      public override Expression Right { get { return null; } }
      public override Token Operator { get { return null; } }

      public object Value { get { return value; } }

    }
  }
}

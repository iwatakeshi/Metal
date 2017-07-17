using Metal.FrontEnd.Scan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metal.FrontEnd.Parse.Grammar {
  abstract class Expression : AST<Expression> {

    internal interface IVisitor<T> {
      T Visit(Assign expression);
      T Visit(Literal expression);
      T Visit(Unary expression);
      T Visit(Binary expression);
      T Visit(Parenthesized expression);
      T Visit(Variable expression);
    }
    internal abstract T Accept<T>(IVisitor<T> visitor);

    internal class Assign : Expression {
      private Token name;
      private Expression value;

      internal override Expression Left => null;
      internal override Expression Right => null;

      internal override Token Operator => null;
      internal Token Name => name;
      internal Expression Value => value;

      internal Assign(Token name, Expression value) {
        this.name = name;
        this.value = value;
      }

      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    internal class Unary : Expression {
      Token @operator;
      Expression right;
      internal Unary(Token @operator, Expression right) {
        this.@operator = @operator;
        this.right = right;
      }

      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }

      internal override Expression Left => null;
      internal override Expression Right => right;
      internal override Token Operator => @operator;
    }


    internal class Binary : Expression {
      private Expression left;
      private Token @operator;
      private Expression right;
      internal Binary(Expression left, Token @operator, Expression right) {
        this.left = left;
        this.@operator = @operator;
        this.right = right;
      }

      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }

      internal override Expression Left => null;
      internal override Expression Right => right;
      internal override Token Operator => @operator;
    }
    internal class Parenthesized : Expression {
      private Expression expression;
      internal Parenthesized(Expression expression) {
        this.expression = expression;
      }
      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }

      internal override Expression Left => null;
      internal Expression Center => expression;
      internal override Expression Right => null;
      internal override Token Operator => null;
    }

    internal class Literal : Expression {
      private object value;
      internal Literal(object value) {
        this.value = value;
      }

      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
      internal override Expression Left => null;
      internal override Expression Right => null;
      internal override Token Operator => null;

      internal object Value => value;
    }

    internal class Variable : Expression {
      private Token name;
      internal Token Name => name;
      internal Variable(Token name) {
        this.name = name;
      }
      internal override Expression Left => null;

      internal override Expression Right => null;

      internal override Token Operator => null;

      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }
  }
}

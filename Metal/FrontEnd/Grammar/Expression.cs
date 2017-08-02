using Metal.FrontEnd.Scan;
using System.Collections.Generic;
using System;

namespace Metal.FrontEnd.Grammar {
  public abstract class Expression : AST<Expression> {

    public interface IVisitor<T> {
      T Visit(Assign expression);
      T Visit(Literal expression);
      T Visit(Unary expression);
      T Visit(Binary expression);
      T Visit(Parenthesized expression);
      T Visit(Variable expression);
      T Visit(Logical expression);
      T Visit(Call expression);
      T Visit(Function expression);
    }
    public abstract T Accept<T>(IVisitor<T> visitor);

    public class Assign : Expression {
      private Token name;
      private Expression value;

      public override Expression Left => null;
      public override Expression Right => null;

      public override Token Operator => null;
      public Token Name => name;
      public Expression Value => value;

      public Assign(Token name, Expression value) {
        this.name = name;
        this.value = value;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Unary : Expression {
      Token @operator;
      Expression right;
      public Unary(Token @operator, Expression right) {
        this.@operator = @operator;
        this.right = right;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }

      public override Expression Left => null;
      public override Expression Right => right;
      public override Token Operator => @operator;
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

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }

      public override Expression Left => left;
      public override Expression Right => right;
      public override Token Operator => @operator;
    }
    public class Parenthesized : Expression {
      private Expression expression;
      public Parenthesized(Expression expression) {
        this.expression = expression;
      }
      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }

      public override Expression Left => null;
      public Expression Center => expression;
      public override Expression Right => null;
      public override Token Operator => null;
    }

    public class Literal : Expression {
      private object value;
      public Literal(object value) {
        this.value = value;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
      public override Expression Left => null;
      public override Expression Right => null;
      public override Token Operator => null;

      public object Value => value;
    }


    public class Variable : Expression {
      private Token name;
      public Token Name => name;
      public Variable(Token name) {
        this.name = name;
      }
      public override Expression Left => null;

      public override Expression Right => null;

      public override Token Operator => null;

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Logical : Expression {
      private Expression left;
      private Expression right;
      private Token @operator;

      public override Expression Left => left;

      public override Expression Right => right;

      public override Token Operator => @operator;

      public Logical(Expression left, Token @operator, Expression right) {
        this.left = left;
        this.@operator = @operator;
        this.right = right;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Call : Expression {

      private Expression callee;
      private Token parenthesis;
      private List<Expression> arguments;

      public override Expression Left => null;

      public override Expression Right => null;

      public override Token Operator => null;

      public Expression Callee => callee;
      public Token Parenthesis => parenthesis;
      public List<Expression> Arguments => arguments;

      public Call(Expression callee, Token parenthesis, List<Expression> arguments) {
        this.callee = callee;
        this.parenthesis = parenthesis;
        this.arguments = arguments;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }
    public class Function : Expression {
      private List<Token> parameters;
      private List<Statement> body;

      public override Expression Left => null;
      public override Expression Right => null;
      public override Token Operator => null;
      public List<Token> Parameters => parameters;
      public List<Statement> Body => body;

      public Function(List<Token> parameters, List<Statement> body) {
        this.parameters = parameters;
        this.body = body;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }
  }
}

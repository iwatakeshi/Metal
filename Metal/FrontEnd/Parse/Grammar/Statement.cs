using Metal.FrontEnd.Scan;
using System.Collections.Generic;

namespace Metal.FrontEnd.Parse.Grammar {
  abstract class Statement {
    internal interface IVisitor<T> {
      T Visit(Block statement);
      T Visit(Expr statement);
      T Visit(Print statement);
      T Visit(Var statement);
    }
    internal abstract T Accept<T>(IVisitor<T> visitor);

    internal class Block : Statement {
      List<Statement> statements;

      internal List<Statement> Statements => statements;
      internal Block(List<Statement> statements) {
        this.statements = statements;
      }
      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    internal class Expr : Statement {
      Expression expression;
      internal Expression Expression => expression;
      internal Expr(Expression expression) {
        this.expression = expression;
      }
      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    internal class Print : Statement {
      Expression expression;
      internal Expression Expression => expression;
      internal Print(Expression expression) {
        this.expression = expression;
      }
      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    internal class Var : Statement {
      private Token name;
      private Expression initializer;
      internal Token Name => name;
      internal Expression Initializer => initializer;
      internal Var(Token name, Expression initializer) {
        this.name = name;
        this.initializer = initializer;
      }
      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }
  }
  


}

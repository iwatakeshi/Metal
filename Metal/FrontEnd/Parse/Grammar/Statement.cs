using Metal.FrontEnd.Scan;
using System.Collections.Generic;
using System;

namespace Metal.FrontEnd.Parse.Grammar {
  abstract class Statement {
    internal interface IVisitor<T> {
      T Visit(Block statement);
      T Visit(Expr statement);
      T Visit(Print statement);
      T Visit(Var statement);
      T Visit(If statement);
      T Visit(For statement);
      T Visit(While statement);
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

    internal class If : Statement {
      private Expression condition;
      private Statement thenBranch;
      private Statement elseBranch;
      internal Expression Condition => condition;
      internal Statement ThenBranch => thenBranch;
      internal Statement ElseBranch => elseBranch;

      internal If(Expression condition, Statement thenBranch, Statement elseBranch) {
        this.condition = condition;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;
      }
      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    internal class For : Statement {
      private Token name;
      private Expression range;
      private Statement body;

      internal Token Name => name;
      internal Expression Range => range;
      internal Statement Body => body;


      internal For(Token name, Expression range, Statement body) {
        this.name = name;
        this.range = range;
        this.body = body;
      }

      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    internal class While : Statement {
      private Expression condition;
      private Statement body;

      internal Expression Condition => condition;
      internal Statement Body => body;

      internal While(Expression condition, Statement body) {
        this.condition = condition;
        this.body = body;
      }

      internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    
  }
}

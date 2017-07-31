﻿using Metal.FrontEnd.Scan;
using System.Collections.Generic;
using System;

namespace Metal.FrontEnd.Grammar {
  public abstract class Statement {
    public interface IVisitor<T> {
      T Visit(Block statement);
      T Visit(Expr statement);
      T Visit(Print statement);
      T Visit(Var statement);
      T Visit(Function statement);
      T Visit(If statement);
      T Visit(For statement);
      T Visit(While statement);
      T Visit(RepeatWhile statement);
    }
    public abstract T Accept<T>(IVisitor<T> visitor);

    public class Block : Statement {
      List<Statement> statements;

      public List<Statement> Statements => statements;
      public Block(List<Statement> statements) {
        this.statements = statements;
      }
      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Expr : Statement {
      Expression expression;
      public Expression Expression => expression;
      public Expr(Expression expression) {
        this.expression = expression;
      }
      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Print : Statement {
      Expression expression;
      public Expression Expression => expression;
      public Print(Expression expression) {
        this.expression = expression;
      }
      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Var : Statement {
      private Token name;
      private Expression initializer;
      public Token Name => name;
      public Expression Initializer => initializer;
      public Var(Token name, Expression initializer) {
        this.name = name;
        this.initializer = initializer;
      }
      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Function : Statement {

      private Token name;
      private List<Token> parameters;
      private List<Statement> body;
      public Token Name => name;
      public List<Token> Parameters => parameters;
      public List<Statement> Body => body;

      public Function(Token name, List<Token> parameters, List<Statement> body) {
        this.name = name;
        this.parameters = parameters;
        this.body = body;
      }
      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }

    }

    public class If : Statement {
      private Expression condition;
      private Statement thenBranch;
      private Statement elseBranch;
      public Expression Condition => condition;
      public Statement ThenBranch => thenBranch;
      public Statement ElseBranch => elseBranch;

      public If(Expression condition, Statement thenBranch, Statement elseBranch) {
        this.condition = condition;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;
      }
      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class For : Statement {
      private Token name;
      private Expression range;
      private Statement body;

      public Token Name => name;
      public Expression Range => range;
      public Statement Body => body;


      public For(Token name, Expression range, Statement body) {
        this.name = name;
        this.range = range;
        this.body = body;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class While : Statement {
      private Expression condition;
      private Statement body;

      public Expression Condition => condition;
      public Statement Body => body;

      public While(Expression condition, Statement body) {
        this.condition = condition;
        this.body = body;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }
    public class RepeatWhile : Statement {
      private Expression condition;
      private Statement body;

      public Expression Condition => condition;
      public Statement Body => body;

      public RepeatWhile(Expression condition, Statement body) {
        this.condition = condition;
        this.body = body;
      }

      public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }
    
  }
}
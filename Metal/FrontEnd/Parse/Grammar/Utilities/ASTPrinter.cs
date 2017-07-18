﻿using System;
using System.Text;
using Metal.FrontEnd.Parse.Grammar;

namespace Metal {
  class ASTPrinter : Expression.IVisitor<string> {
    public string Print(Expression expression) {
      return expression.Accept(this);
    }

    public string Visit(Expression.Literal expression) {
      return expression.Value.ToString();
    }

    public string Visit(Expression.Unary expression) {
      return Parenthesize(expression.Operator.Lexeme, expression.Right);
    }

    public string Visit(Expression.Binary expression) {
      return Parenthesize(expression.Operator.Lexeme, expression.Left, expression.Right);
    }

    public string Visit(Expression.Parenthesized expression) {
      return Parenthesize("parenthesized", expression.Center);
    }

    public string Visit(Expression.Assign expression) {
      throw new NotImplementedException();
    }

    public string Visit(Expression.Variable expression) {
      throw new NotImplementedException();
    }

    public string Visit(Expression.Logical expression) {
      throw new NotImplementedException();
    }

    private String Parenthesize(String name, params Expression[] expressions) {
      StringBuilder builder = new System.Text.StringBuilder();

      builder.Append("(").Append(name);
      foreach (var expression in expressions) {
        builder.Append(" ");
        builder.Append(expression.Accept(this));
      }
      builder.Append(")");

      return builder.ToString();
    }
  }
}

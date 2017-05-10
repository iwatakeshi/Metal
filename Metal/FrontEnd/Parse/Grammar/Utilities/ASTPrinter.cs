﻿using System;
using System.Text;
using Metal.FrontEnd.Parse.Grammar;

namespace Metal {
  public class ASTPrinter : Expression.Visitor<string> {
    public string Print(Expression expression) {
      return expression.Accept(this);
    }

    public string VisitLiteral(Expression.Literal expression) {
      return expression.Value.ToString();
    }

    public string VisitUnary(Expression.Unary expression) {
      return Parenthesize(expression.Operator.Lexeme, expression.Right);
    }

    public string VisitBinary(Expression.Binary expression) {
      return Parenthesize(expression.Operator.Lexeme, expression.Left, expression.Right);
    }

    public string VisitParenthesized(Expression.Parenthesized expression) {
      return Parenthesize("parenthesized", expression.Center);
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

using Metal.Diagnostics.Runtime;
using Metal.FrontEnd.Parse.Grammar;
using Metal.FrontEnd.Scan;
using System;

namespace Metal.FrontEnd.Interpret {
  public class Interpreter : Expression.Visitor<Object> {
    public object VisitBinary(Expression.Binary expression) {
      object left = Evaluate(expression.Left);
      object right = Evaluate(expression.Right);

      // '+' operator
      if (expression.Operator.IsOperator("+")) {

        if (left is int && right is int) {
          var result1 = ConvertObjectToInt(left, right);
          return result1.Item1 + result1.Item2;
        }
        if (left is double && right is double) {
          var result2 = ConvertObjectToDouble(left, right);
          return result2.Item1 + result2.Item2;
        }

        if ((left is int || left is double) && (right is int || right is double)) {
          var result3 = ConvertObjectToDouble(left, right);
          return result3.Item1 + result3.Item2;
        }

        if (left is string || right is string) {
          Console.WriteLine(string.Format("left {0}, right {1}", left.ToString(), right.ToString()));
          return left.ToString() + right.ToString();
        }
      }

      // '-' operator
      if (expression.Operator.IsOperator("-")) {
        if ((left is int || left is double) && (right is int || right is double)) {
          var result4 = ConvertObjectToDouble(left, right);
          return result4.Item1 - result4.Item2;
        }
        var result5 = ConvertObjectToInt(left, right);
        return result5.Item1 - result5.Item2;
      }

      // '/' operator
      if (expression.Operator.IsOperator("/")) {
        if ((left is int || left is double) && (right is int || right is double)) {
          var result6 = ConvertObjectToDouble(left, right);
          return result6.Item1 / result6.Item2;
        }

        var result7 = ConvertObjectToInt(left, right);
        return result7.Item1 / result7.Item2;
      }

      // '*' operator
      if (expression.Operator.IsOperator("*")) {
        if ((left is int || left is double) && (right is int || right is double)) {
          var result8 = ConvertObjectToDouble(left, right);
          return result8.Item1 * result8.Item2;
        }
        var result9 = ConvertObjectToInt(left, right);
        return result9.Item1 * result9.Item2;
      }

      // '>', '>=', '<', '<=' operators
      if (expression.Operator.IsOperator(">")) {
        CheckNumberOperand(expression.Operator, left, right);
        var result10 = ConvertObjectToDouble(left, right);
        return result10.Item1 > result10.Item2;
      }
      if (expression.Operator.IsOperator(">=")) {
        CheckNumberOperand(expression.Operator, left, right);
        var result11 = ConvertObjectToDouble(left, right);
        return result11.Item1 >= result11.Item2;
      }
      if (expression.Operator.IsOperator("<")) {
        CheckNumberOperand(expression.Operator, left, right);
        var result12 = ConvertObjectToDouble(left, right);
        return result12.Item1 < result12.Item2;
      }
      if (expression.Operator.IsOperator("<=")) {
        CheckNumberOperand(expression.Operator, left, right);
        var result13 = ConvertObjectToDouble(left, right);
        return result13.Item1 <= result13.Item2;
      }

      // Check equality
      if (expression.Operator.IsOperator("!=")) return !isEqual(left, right);
      if (expression.Operator.IsOperator("==")) return isEqual(left, right);

      return null;
    }

    public object VisitLiteral(Expression.Literal expression) {
      return expression.Value;
    }

    public object VisitParenthesized(Expression.Parenthesized expression) {
      return Evaluate(expression.Center);
    }

    public object VisitUnary(Expression.Unary expression) {
      object right = Evaluate(expression.Right);
      if(expression.Operator.IsOperator("-")) {
        CheckNumberOperand(expression.Operator, right);
        if (right is double) return -(double)right;
        else return -(int)right;
      }
      if(expression.Operator.IsOperator("!")) {
        return !isTrue(right);
      }
      return null;
    }

    private (int, int) ConvertObjectToInt(object left, object right) {
      int.TryParse(left.ToString(), out int a);
      int.TryParse(right.ToString(), out int b);
      Console.WriteLine(string.Format("a {0}, b {1}", a, b));
      return (a, b);
    }

    private (double, double) ConvertObjectToDouble(object left, object right) {
      double.TryParse(left.ToString(), out double a);
      double.TryParse(right.ToString(), out double b);
      Console.WriteLine(string.Format("a {0}, b {1}", a, b));
      return (a, b);
    }

    private void CheckNumberOperand(Token @operator, object operand) {
      if (operand is int || operand is double) return;
      throw new RuntimeError(@operator, "Operand must be a number.");
    }

    private void CheckNumberOperand(Token @operator, object left, object right) {
      if ((left is int || left is double) && (right is int || right is double)) return;
      throw new RuntimeError(@operator, "Operands must be numbers.");
    }

    private bool isTrue(object @object) {
      if (@object == null) return false;
      if (@object is Boolean) return (bool)@object;
      return true;
    }

    private bool isEqual(object a, object b) {
      if (a == null && b == null) return true;
      if (a == null) return false;
      return a.Equals(b);
    }

    private object Evaluate(Expression expression) {
      return expression.Accept(this);
    }
    
    public void Interpret(Expression expression) {
      try {
        object value = Evaluate(expression);
        Console.WriteLine(value == null ? "null" : value.ToString());
      } catch(RuntimeError error) {
        Metal.RuntimeError(error);
      }
    }
  }
}

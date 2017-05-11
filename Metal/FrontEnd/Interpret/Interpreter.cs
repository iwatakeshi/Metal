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

        CheckNullOperand(expression.Operator, left, right);

        if (left is string || right is string) {
          //Console.WriteLine(string.Format("left {0}, right {1}", left.ToString(), right.ToString()));
          return left.ToString() + right.ToString();
        }

        if (left is int && right is int) {
          return ApplyIntOperatorToOperand(ConvertObjectToInt(left, right), "+");
        }

        if ((left is int || left is double) && (right is int || right is double)) {
          return ApplyDoubleOperatorToOperand(ConvertObjectToDouble(left, right), "+");
        }
      }

      // '-' operator
      if (expression.Operator.IsOperator("-")) {
        if (left is int && right is int) {
          return ApplyIntOperatorToOperand(ConvertObjectToInt(left, right), "-");
        }
        return ApplyDoubleOperatorToOperand(ConvertObjectToInt(left, right), "-");
      }

      // '*' operator
      if (expression.Operator.IsOperator("*")) {

        if(left is int && right is int) {
          return ApplyIntOperatorToOperand(ConvertObjectToInt(left, right), "*");
        }

        if ((left is int || left is double) && (right is int || right is double)) {
          return ApplyDoubleOperatorToOperand(ConvertObjectToInt(left, right), "*");
        }
      }

      // '/' operator
      if (expression.Operator.IsOperator("/")) {
        if (left is int && right is int) {
          return ApplyIntOperatorToOperand(ConvertObjectToInt(left, right), "/");
        }
        if ((left is int || left is double) && (right is int || right is double)) {
          return ApplyDoubleOperatorToOperand(ConvertObjectToDouble(left, right), "/");
        }
      }

      // '>', '>=', '<', '<=' operators
      if (expression.Operator.IsOperator(">")) {
        CheckNumberOperand(expression.Operator, left, right);
        return ApplyBoolOperatorToOperand(ConvertObjectToDouble(left, right), ">");
      }
      if (expression.Operator.IsOperator(">=")) {
        CheckNumberOperand(expression.Operator, left, right);
        return ApplyBoolOperatorToOperand(ConvertObjectToDouble(left, right), ">=");
      }
      if (expression.Operator.IsOperator("<")) {
        CheckNumberOperand(expression.Operator, left, right);
        return ApplyBoolOperatorToOperand(ConvertObjectToDouble(left, right), "<");
      }
      if (expression.Operator.IsOperator("<=")) {
        CheckNumberOperand(expression.Operator, left, right);
        return ApplyBoolOperatorToOperand(ConvertObjectToDouble(left, right), "<=");
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
      //Console.WriteLine(string.Format("a {0}, b {1}", a, b));
      return (a, b);
    }

    private (double, double) ConvertObjectToDouble(object left, object right) {
      double.TryParse(left.ToString(), out double a);
      double.TryParse(right.ToString(), out double b);
      //Console.WriteLine(string.Format("a {0}, b {1}", a, b));
      return (a, b);
    }

    private int ApplyIntOperatorToOperand((int, int) operands, string operand) {
      switch (operand) {
        case "+": return operands.Item1 + operands.Item2;
        case "-": return operands.Item1 - operands.Item2;
        case "*": return operands.Item1 * operands.Item2;
        case "/": return operands.Item1 / operands.Item2;
      }
      return 0;
    }

    private double ApplyDoubleOperatorToOperand((double, double) operands, string operand) {
      switch (operand) {
        case "+": return operands.Item1 + operands.Item2;
        case "-": return operands.Item1 - operands.Item2;
        case "*": return operands.Item1 * operands.Item2;
        case "/": return operands.Item1 / operands.Item2;
      }
      return 0.0f;
    }

    private bool ApplyBoolOperatorToOperand((double, double) operands, string operand) {
      switch (operand) {
        case ">": return operands.Item1 > operands.Item2;
        case ">=": return operands.Item1 >= operands.Item2;
        case "<": return operands.Item1 < operands.Item2;
        case "<=": return operands.Item1 <= operands.Item2;
      }
      return false;
    }

    private void CheckNullOperand(Token @operator, object left, object right) {
      if (left == null || right == null) {
        throw new RuntimeError(@operator, "Operand must not be null.");
      }
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

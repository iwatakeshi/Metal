using Metal.FrontEnd.Exceptions;
using Metal.FrontEnd.Grammar;
using Metal.FrontEnd.Types;
using Metal.FrontEnd.Scan;
using System;
using System.Collections.Generic;
using Metal.Intermediate;
using System.Collections;

namespace Metal.FrontEnd.Interpret {
  public class Interpreter : Expression.IVisitor<object>, Statement.IVisitor<object> {


    MetalEnvironment globals = new MetalEnvironment();
    MetalEnvironment environment;

    public MetalEnvironment Globals => globals;
    public MetalEnvironment Environment => environment;

    public Interpreter() {

      DefineGlobals();
      environment = globals;
    }

    public void ResetEnvironment() {
      globals = new MetalEnvironment();
      DefineGlobals();
      environment = globals;
    }

    private void DefineGlobals() {
      globals.Define("print", new MetalType.Callable(1, (arg1, arg2) => {
        Console.WriteLine(arg2[0]);
        return null;
      }));

      globals.Define("clock", new MetalType.Callable((arg1, arg2) => {
        return (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) / 1000.0;
      }));

      globals.Define("typeof", new MetalType.Callable(1, (arg1, arg2) => {
        return ((MetalType)MetalType.DeduceType(arg2[0])).TypeName;
      }));
    }


    /* Visit Statements */
    public object Visit(Statement.Expr statement) {
      Evaluate(statement.Expression);
      return null;
    }
    //public object Visit(Statement.Print statement) {
    //  Object value = Evaluate(statement.Expression);
    //  Console.WriteLine(value ?? "null");
    //  return null;
    //}

    public object Visit(Statement.Return statement) {
      object value = null;
      if (statement.Value != null) {
        value = Evaluate(statement.Value);
      }
      throw new MetalException.Runtime.Return(value);
    }

    public object Visit(Statement.Break statement) {
      throw new MetalException.Runtime.Break();
    }

    public object Visit(Statement.Block statement) {
      ExecuteBlock(statement.Statements, new MetalEnvironment(environment));
      return null;
    }

    public object Visit(Statement.Var statement) {

      object value = null;
      if (statement.Initializer != null) {
        value = Evaluate(statement.Initializer);
      }
      environment.Define(statement.Name.Lexeme, value);
      return null;
    }

    public object Visit(Statement.Function statement) {
      MetalType.Function function = new MetalType.Function(
        statement.Name.Lexeme, statement.Declaration, environment);
      environment.Define(statement.Name.Lexeme, function);
      return null;
    }

    public object Visit(Statement.If statement) {
      if (IsTruthy(Evaluate(statement.Condition))) {
        Execute(statement.ThenBranch);
      } else if (statement.ElseBranch != null) {
        Execute(statement.ElseBranch);
      }
      return null;
    }

    public object Visit(Statement.While statement) {
      try {
        while (IsTruthy(Evaluate(statement.Condition))) {
          Execute(statement.Body);
        }
      } catch (MetalException.Runtime.Break) { /* Do nothing */ }

      return null;
    }

    public object Visit(Statement.RepeatWhile statement) {
      try {
        do {
          Execute(statement.Body);
        } while (IsTruthy(Evaluate(statement.Condition)));
      } catch (MetalException.Runtime.Break) { /* Do nothing */ }
      return null;
    }

    public object Visit(Statement.For statement) {
      try {
        environment.Define(statement.Name.Lexeme, null);
        foreach (var value in (IEnumerable)Evaluate(statement.Expression)) {
          environment.Assign(statement.Name, value);
          Execute(statement.Body);
        }
      } catch (MetalException.Runtime.Break) { /* Do nothing */ }
      return null;
    }

    /* Visit Expressions */

    public object Visit(Expression.Assign expression) {
      object value = Evaluate(expression.Value);
      environment.Assign(expression.Name, value);
      return value;
    }

    public object Visit(Expression.Logical expression) {
      object left = Evaluate(expression.Left);
      if (expression.Operator.IsOperator("or")) {
        if (IsTruthy(left)) return left;
      } else {
        if (!IsTruthy(left)) return left;
      }
      return Evaluate(expression.Right);
    }

    public object Visit(Expression.Conditional expression) {
      if (IsTruthy(Evaluate(expression.Condition))) {
        return Evaluate(expression.ThenBranch);
      } else {
        return Evaluate(expression.ElseBranch);
      }
    }

    public object Visit(Expression.Literal expression) {
      return expression.Value;
    }

    public object Visit(Expression.Parenthesized expression) {
      return Evaluate(expression.Center);
    }

    public object Visit(Expression.Unary expression) {
      object right = Evaluate(expression.Right);
      if (expression.Operator.IsOperator("-")) {
        CheckNumberOperand(expression.Operator, right);
        if (right is MetalType.Number) return -(MetalType.Number)right;
        if (right is double) return -(double)right;
        else return -(int)right;
      }
      if (expression.Operator.IsOperator("!")) {
        return !IsTruthy(right);
      }
      return null;
    }

    public object Visit(Expression.Binary expression) {
      object left = Evaluate(expression.Left);
      object right = Evaluate(expression.Right);
      // '+' operator
      if (expression.Operator.IsOperator("+")) {

        CheckNullOperand(expression.Operator, left, right);

        if (left is string || right is string) {
          //Console.WriteLine(string.Format("left {0}, right {1}", left.ToString(), right.ToString()));
          return left.ToString() + right.ToString();
        }

        if ((left is int || right is int) || (left is double || right is double)) {
          return new MetalType.Number(left) + new MetalType.Number(right);
        }

        if((left is MetalType.Number) && (right is MetalType.Number)) {
          return ((MetalType.Number)left) + ((MetalType.Number)right);
        }
      }

      // '-' operator
      if (expression.Operator.IsOperator("-")) {
        if ((left is int || right is int) || (left is double || right is double)) {
          return new MetalType.Number(left) - new MetalType.Number(right);
        }
      }

      // '*' operator
      if (expression.Operator.IsOperator("*")) {

        if ((left is int || right is int) || (left is double || right is double)) {
          return new MetalType.Number(left) * new MetalType.Number(right);
        }
      }

      // '/' operator
      if (expression.Operator.IsOperator("/")) {
        if ((left is int || right is int) || (left is double || right is double)) {
          return new MetalType.Number(left) / new MetalType.Number(right);
        }
      }

      // '>', '>=', '<', '<=' operators
      if (expression.Operator.IsOperator(">")) {
        CheckNumberOperand(expression.Operator, left, right);
        return new MetalType.Number(left) > new MetalType.Number(right);
      }
      if (expression.Operator.IsOperator(">=")) {
        CheckNumberOperand(expression.Operator, left, right);
        return new MetalType.Number(left) >= new MetalType.Number(right);
      }
      if (expression.Operator.IsOperator("<")) {
        CheckNumberOperand(expression.Operator, left, right);
        return new MetalType.Number(left) < new MetalType.Number(right);
      }
      if (expression.Operator.IsOperator("<=")) {
        CheckNumberOperand(expression.Operator, left, right);
        return new MetalType.Number(left) <= new MetalType.Number(right);
      }

      // Check equality
      if (expression.Operator.IsOperator("!=")) return !IsEqual(left, right);
      if (expression.Operator.IsOperator("==")) return IsEqual(left, right);

      if (expression.Operator.IsOperator("..")) {
        CheckNumberOperand(expression.Operator, left, right);
        return ApplyRangeOperator(ConvertObjectToInt(left, right));
      }

      return null;
    }

    public object Visit(Expression.Call expression) {
      object callee = Evaluate(expression.Callee);
      List<object> arguments = new List<object>();
      foreach (var argument in expression.Arguments) {
        arguments.Add(Evaluate(argument));
      }
      if (!(callee is MetalType.ICallable)) {
        throw new MetalException.Runtime(expression.Parenthesis,
        "Can only call functions and classes");
      }
      MetalType.ICallable function = (MetalType.ICallable)callee;

      if (arguments.Count != function.Arity) {
        throw new MetalException.Runtime(expression.Parenthesis,
          string.Format("Expected {0} arguments but got {1}.", function.Arity, arguments.Count));
      }

      return function.Call(this, arguments);
    }

    public object Visit(Expression.Function expression) {
      MetalType.Function function = new MetalType.Function(null, expression, environment);
      return function;
    }

    public object Visit(Expression.Variable expression) {
      return environment.Get(expression.Name);
    }

    /* Helpers */

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
      int a = operands.Item1, b = operands.Item2;
      switch (operand) {
        case "+": return a + b;
        case "-": return a - b;
        case "*": return a * b;
        case "/": {
            if (b == 0) throw new MetalException.Runtime("Division by zero.");
            return a / b;
          }
      }
      return 0;
    }

    private double ApplyDoubleOperatorToOperand((double, double) operands, string operand) {
      double a = operands.Item1, b = operands.Item2;
      switch (operand) {
        case "+": return a + b;
        case "-": return a - b;
        case "*": return a * b;
        case "/": {
            if (Equals(b, 0)) Metal.RuntimeError(new MetalException.Runtime("Division by zero."));
            return a / b;
          }
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

    private object ApplyRangeOperator((int, int) operands) {
      var a = operands.Item1;
      var b = operands.Item2;
      return CreateRange(a, b);
    }

    IEnumerable<int> CreateRange(int a, int b) {
      var increment = b > a ? 1 : -1;
      for (var i = a; i != b; i += increment)
        yield return i;
      yield return b;
    }

    private void CheckNullOperand(Token @operator, object left, object right) {
      if (left == null || right == null) {
        throw new MetalException.Runtime(@operator, "Operand must not be null.");
      }
    }

    private void CheckNumberOperand(Token @operator, object operand) {
      if (operand is int || operand is double) return;
      if (operand is MetalType.Number) return;
      throw new MetalException.Runtime(@operator, "Operand must be a number.");
    }

    private void CheckNumberOperand(Token @operator, object left, object right) {
      if (left is MetalType.Number || right is MetalType.Number) return;
      if ((left is int || left is double) && (right is int || right is double)) return;
      throw new MetalException.Runtime(@operator, "Operands must be numbers.");
    }

    private bool IsTruthy(object @object) {
      if (@object == null) return false;
      if (@object is bool) return (bool)@object;
      if (@object is MetalType.Boolean) return ((MetalType.Boolean)@object).Value;
      return true;
    }

    private MetalType.Boolean IsEqual(object a, object b) {
      if (a == null && b == null) return new MetalType.Boolean(true);
      if (a == null) return new MetalType.Boolean(false);
      return new MetalType.Boolean(a.Equals(b));
    }

    private object Evaluate(Expression expression) {
      return expression.Accept(this);
    }

    internal string Interpret(Expression expression) {
      try {
        object value = Evaluate(expression);
        return value?.ToString();
      } catch (MetalException.Runtime error) {
        Metal.RuntimeError(error);
        return null;
      }
    }

    internal void Interpret(List<Statement> statements) {
      try {
        foreach (var statement in statements) {
          Execute(statement);
        }
      } catch (MetalException.Runtime error) {
        Metal.RuntimeError(error);
      }
    }

    public void Execute(Statement statement) {
      if (statement != null) {
        statement.Accept(this);
      }
    }

    public void ExecuteBlock(List<Statement> statements, MetalEnvironment environment) {
      if (environment == null) throw new ArgumentNullException(nameof(environment));
      MetalEnvironment previous = this.environment;
      try {
        this.environment = environment;
        foreach (var statement in statements) {
          Execute(statement);
        }
      } finally {
        this.environment = previous;
      }
    }
  }
}


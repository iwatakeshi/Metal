using System;
using System.Collections.Generic;
using Metal.Intermediate;
using Metal.FrontEnd.Grammar;
using Metal.FrontEnd.Interpret;
using Metal.FrontEnd.Exceptions;
using System.Collections;

namespace Metal.FrontEnd.Types {
  public abstract class MetalType : Object {
    public abstract string TypeName { get; }
    public interface ICallable {
      int Arity { get; }
      Func<Interpreter, List<object>, object> Call { get; }
    }

    public class Callable : MetalType, ICallable {
      Func<Interpreter, List<object>, object> callee;
      private int arity;
      public Callable(Func<Interpreter, List<object>, object> callee) {
        this.arity = 0;
        this.callee = callee;
      }
      public Callable(int arity, Func<Interpreter, List<object>, object> callee) {
        this.callee = callee;
        this.arity = arity;
      }

      public int Arity { get => arity; }
      public Func<Interpreter, List<object>, object> Call { get => callee; }
      public override string TypeName => "callable";
    }

    public class Function : MetalType, ICallable {
      private string name;
      private Expression.Function declaration;
      private MetalEnvironment closure;

      Func<Interpreter, List<object>, object> callee;

      public Function(string name, Expression.Function declaration, MetalEnvironment closure) {
        this.name = name;
        this.declaration = declaration;
        this.closure = closure;
        callee = (interpreter, arguments) => {
          MetalEnvironment environment = new MetalEnvironment(closure);
            if (declaration != null) {
              for (var i = 0; i < declaration.Parameters.Count; i++) {
                environment.Define(declaration.Parameters[i].Lexeme, arguments[i]);
              }
            }

            if (declaration != null) {
            try {
              interpreter.ExecuteBlock(declaration.Body, environment);
            } catch(MetalException.Runtime.Return value) {
              return value.Value;
            }
          }
          return null;
          
        };
      }
      public string Name => name;
      public int Arity { get { return declaration.Parameters.Count; } }
      Func<Interpreter, List<object>, object> ICallable.Call => callee;
      public MetalEnvironment Closure => closure;
      public override string ToString() {
        return name == null ? "<func>": string.Format("<func {0}>", name);
      }

      public override string TypeName => "function";
    }


    public class Number : MetalType {
      private object value;
      public object Value => value;

      public Number(object value) {
        this.value = (value is MetalType) ? ((MetalType.Number)value).value : value;
      }

      /* Negation */
      public static Number operator -(Number self) {
        if (self.Value is int) {
          int.TryParse(self.ToString(), out int a);
          return new Number(-a);
        }
        double.TryParse(self.ToString(), out double b);
        return new Number(-b);
      }

      /* Addition */
      public static Number operator +(Number left, Number right) {
        if (left.Value is int && right.Value is int) {
          int.TryParse(left.Value.ToString(), out int a);
          int.TryParse(right.Value.ToString(), out int b);
          return new Number(a + b);
        }
        double.TryParse(left.Value.ToString(), out double c);
        double.TryParse(right.Value.ToString(), out double d);
        return new Number(c + d);
      }

      public static Number operator +(Number left, int right) {
        return left + new Number(right);
      }

      public static Number operator +(int left, Number right) {
        return new Number(left) + right;
      }

      public static Number operator +(Number left, double right) {
       return left + new Number(right);
      }

      public static Number operator +(double left, Number right) {
        return new Number(left) + right;
      }

      /* Subtraction */
      public static Number operator -(Number left, Number right) {
        if (left.Value is int && right.Value is int) {
          int.TryParse(left.Value.ToString(), out int a);
          int.TryParse(right.Value.ToString(), out int b);
          return new Number(a - b);
        }
        double.TryParse(left.Value.ToString(), out double c);
        double.TryParse(right.Value.ToString(), out double d);
        return new Number(c - d);
      }
      public static Number operator -(Number left, int right) {
        return left - new Number(right);
      }

      public static Number operator -(int left, Number right) {
        return new Number(left) - right;
      }

      public static Number operator -(Number left, double right) {
        return left - new Number(right);
      }

      public static Number operator -(double left, Number right) {
        return new Number(left) - right;
      }

      /* Multiplication */
      public static Number operator *(Number left, Number right) {
        if (left.Value is int && right.Value is int) {
          int.TryParse(left.Value.ToString(), out int a);
          int.TryParse(right.Value.ToString(), out int b);
          return new Number(a * b);
        }
        double.TryParse(left.Value.ToString(), out double c);
        double.TryParse(right.Value.ToString(), out double d);
        return new Number(c * d);
      }

      public static Number operator *(Number left, int right) {
        return left * new Number(right);
      }

      public static Number operator *(int left, Number right) {
        return new Number(left) * right;
      }

      public static Number operator *(Number left, double right) {
        return left * new Number(right);
      }

      public static Number operator *(double left, Number right) {
        return new Number(left) * right;
      }

      /* Division */
      public static Number operator /(Number left, Number right) {
        if (left.Value is int && right.Value is int) {
          int.TryParse(left.Value.ToString(), out int a);
          int.TryParse(right.Value.ToString(), out int b);
          return new Number(a / b);
        }
        double.TryParse(left.Value.ToString(), out double c);
        double.TryParse(left.Value.ToString(), out double d);
        return new Number(c / d);
      }
      public static Number operator /(Number left, int right) {
        return left / new Number(right);
      }

      public static Number operator /(int left, Number right) {
        return new Number(left) / right;
      }

      public static Number operator /(Number left, double right) {
        return left / new Number(right);
      }

      public static Number operator /(double left, Number right) {
        return new Number(left) / right;
      }

      /* Greater than */
      public static Boolean operator >(Number left, Number right) {
        double.TryParse(left.Value.ToString(), out double a);
        double.TryParse(right.Value.ToString(), out double b);
        return new Boolean(a > b);
      }
      public static Boolean operator >(Number left, int right) {
        return left > new Number(right);
      }

      public static Boolean operator >(int left, Number right) {
        return new Number(left) > right;
      }

      public static Boolean operator >(Number left, double right) {
        return left > new Number(right);
      }

      public static Boolean operator >(double left, Number right) {
        return new Number(left) > right;
      }

      /* Less than */
      public static Boolean operator <(Number left, Number right) {
        double.TryParse(left.Value.ToString(), out double a);
        double.TryParse(right.Value.ToString(), out double b);
        return new Boolean(a < b);
      }
      public static Boolean operator <(Number left, int right) {
        return left < new Number(right);
      }

      public static Boolean operator <(int left, Number right) {
        return new Number(left) < right;
      }

      public static Boolean operator <(Number left, double right) {
        return left < new Number(right);
      }

      public static Boolean operator <(double left, Number right) {
        return new Number(left) < right;
      }

      /* Greater than, equal to*/
      public static Boolean operator >=(Number left, Number right) {
        double.TryParse(left.Value.ToString(), out double a);
        double.TryParse(right.Value.ToString(), out double b);
        return new Boolean(a >= b);
      }

      public static Boolean operator >=(Number left, int right) {
        return left >= new Number(right);
      }

      public static Boolean operator >=(int left, Number right) {
        return new Number(left) >= right;
      }

      public static Boolean operator >=(Number left, double right) {
        return left >= new Number(right);
      }

      public static Boolean operator >=(double left, Number right) {
        return new Number(left) >= right;
      }

      /* Less than, equal to */
      public static Boolean operator <=(Number left, Number right) {
        double.TryParse(left.Value.ToString(), out double a);
        double.TryParse(right.Value.ToString(), out double b);
        return new Boolean(a <= b);
      }

      public static Boolean operator <=(Number left, int right) {
        return left <= new Number(right);
      }

      public static Boolean operator <=(int left, Number right) {
        return new Number(left) <= right;
      }

      public static Boolean operator <=(Number left, double right) {
        return left <= new Number(right);
      }

      public static Boolean operator <=(double left, Number right) {
        return new Number(left) <= right;
      }

      /* Equal to */
      public static Boolean operator ==(Number left, Number right) {
        double.TryParse(left.Value.ToString(), out double a);
        double.TryParse(right.Value.ToString(), out double b);
        return new Boolean(a == b);
      }

      public static Boolean operator ==(Number left, int right) {
        return left == new Number(right);
      }

      public static Boolean operator ==(int left, Number right) {
        return new Number(left) == right;
      }

      public static Boolean operator ==(Number left, double right) {
        return left == new Number(right);
      }

      public static Boolean operator ==(double left, Number right) {
        return new Number(left) == right;
      }

      /* Not equal to */
      public static Boolean operator !=(Number left, Number right) {
        double.TryParse(left.Value.ToString(), out double a);
        double.TryParse(right.Value.ToString(), out double b);
        return new Boolean(a != b);
      }

      public static Boolean operator !=(Number left, int right) {
        return left != new Number(right);
      }

      public static Boolean operator !=(int left, Number right) {
        return new Number(left) != right;
      }

      public static Boolean operator !=(Number left, double right) {
        return left != new Number(right);
      }

      public static Boolean operator !=(double left, Number right) {
        return new Number(left) != right;
      }

      public override bool Equals(object obj) {
        if (obj is int) return (int)value == (int) obj;
        if (obj is double) return (double)value == (double)obj;
        if (obj is Number) return value == ((Number)obj).value;
        return false;
      }

      public override int GetHashCode() {
        return ((int)value ^ (int)((int)value >> 32));
      }

      /* Type conversions */
      public override string ToString() {
        return value.ToString();
      }

      public int ToInteger() {
        return (int)value;
      }

      public double ToFloatingPoint() {
        return (double)value;
      }

      public Number ToIntegerNumber() {
        return new Number(this.ToInteger());
      }

      public Number ToFloatingPointNumber() {
        return new Number(this.ToFloatingPoint());
      }

      public override string TypeName => "number";

      public bool IsFloatingPoint() {
        return value.ToString().Contains(".");
      }

      public static bool IsNumber(params object[] numbers) {
        foreach (var number in numbers) {
          if (!((number is int) || (number is double) || (number is Number))) {
            return false;
          }
        }
        return true;
      }
    }

    public class String : MetalType {
      private string value;
      public string Value => value;

      public String(string value) {
        this.value = value;
      }

      public String(object value) {
        this.value = value.ToString();
      }

      public static String operator +(String left, String right) {
        return new String(left.value + right.value);
      }

      public static String operator +(String left, string right) {
        return new String(left.value + right);
      }

      public static String operator +(string left, String right) {
        return new String(left + right.value);
      }

      public static String operator +(String left, Number right) {
        return new String(left + right); 
      }

      public static String operator +(Number left, String right) {
        return new String(left + right);
      }


      public override string ToString() {
        return value;
      }

      public override string TypeName => "string";
    }

    public class Boolean: MetalType {
      private bool value;
      public bool Value => value;

      public Boolean(bool value) {
        this.value = value;
      }
      public static Boolean operator &(Boolean left, Boolean right) {
        System.Boolean.TryParse(left.Value.ToString(), out bool a);
        System.Boolean.TryParse(right.Value.ToString(), out bool b);
        return new Boolean(a && b);
      }

      public static Boolean operator |(Boolean left, Boolean right) {
        System.Boolean.TryParse(left.Value.ToString(), out bool a);
        System.Boolean.TryParse(right.Value.ToString(), out bool b);
        return new Boolean(a || b);
      }

      public static Boolean operator !(Boolean self) {
        System.Boolean.TryParse(self.Value.ToString(), out bool a);
        return new Boolean(!self.Value);
      }

      public override string ToString() {
        return value.ToString().ToLower();
      }

      public override string TypeName => "boolean";
    }

    public class Any : MetalType {
      private object value;
      public object Value;

      public Any(object value) {
        this.value = value;
      }

      public override string ToString() {
        return value.ToString();
      }

      public override string TypeName => "any";

    }

    public class Null : MetalType {
      public object Value => null;
      public override string TypeName => "null";
    }

    public class Range : MetalType, IEnumerable {
      public object start;
      public object end;
      public Range(object start, object end) {
        this.start = start;
        this.end = end;
      }

      public override string TypeName => "range";

      public Number Start => new Number(start);
      public Number End => new Number(end);

      public IEnumerator GetEnumerator() {
        var increment = (int)end > (int)start ? 1 : -1;
        for (var i = (int)start; i != (int)end; i += increment)
          yield return new Number(i);
        yield return new Number(end);
      }

      public List<Number> ToList() {
        List<Number> range = new List<Number>();
        foreach(var value in this) {
          range.Add((Number)value);
        }
        return range;
      }

      public override string ToString() {
        return "range";
      }
    }

    public static object DeduceType(object value) {
      if (value is string) return new String(((string)value).ToString());
      if (value is int) return new Number(value);
      if (value is double) return new Number(value);
      if (value is bool) return new Boolean((bool)value);
      if (value is null) return new Null();
      if (value is MetalType) return (MetalType)value;
      return new Any(value);
    }
  }
}

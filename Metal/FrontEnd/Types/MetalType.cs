using System;
using System.Collections.Generic;
using Metal.Intermediate;
using Metal.FrontEnd.Grammar;
using Metal.FrontEnd.Interpret;
using Metal.FrontEnd.Exceptions;
using System.Collections;
using System.Linq;
using System.Text;

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
            } catch (MetalException.Runtime.Return value) {
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
        return name == null ? "<func>" : string.Format("<func {0}>", name);
      }

      public override string TypeName => "function";
    }

    public class Array : MetalType, IEnumerable {
      private List<object> values;
      public List<object> Values => values;
      public int Count => values.Count;
      public Array(List<object> values) {
        this.values = values;
      }
      public IEnumerator GetEnumerator() {
        return this.values.GetEnumerator();
      }

      public object this[int index] {
        get => values[index];
        set => values[index] = value;
      }

      public object this[Number index] {
        get { return values[(int)index.Value]; }
        set { values[(int)index.Value] = value; }
      }

      public Array this[Range index] {
        get {
          List<object> slice = new List<object>();
          foreach (var i in index.ToList()) {
            slice.Add(values[(int)i.Value]);
          }
          return new Array(slice);
        }
      }

      public override string ToString() {
        return string.Format("[{0}]", string.Join(", ", values));
      }

      public override string TypeName => "array";
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
        return new Boolean(a.Equals(b));
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
        return new Boolean(!(a.Equals(b)));
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
        if (obj is int) return (int)value == (int)obj;
        if (obj is double) return ((double)value).Equals((double)obj);
        if (obj is Number) return value == ((Number)obj).value;
        return false;
      }

      public override int GetHashCode() {
        return ((int)Value ^ (int)((int)Value >> 32));
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
    }

    public class String : MetalType {
      private string value;
      public string Value => value;

      public String(string value) {
        this.value = value;
      }

      public String(object value) {
        this.value = (string)value;
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

    public class Boolean : MetalType {
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
      public override string ToString() {
        return "null";
      }
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
        foreach (var value in this) {
          range.Add((Number)value);
        }
        return range;
      }

      public override string ToString() {
        return string.Format("{0}..{1}", start, end);
      }
    }

    public class Tuple : MetalType {
      List<object> values;
      public List<object> Values => values;
      public Tuple(List<object> values) {
        this.values = values;
      }
      public override string TypeName => "tuple";
      public override string ToString() {
        StringBuilder tuple = new StringBuilder();
        tuple.Append("(");
        var count = 0;
        foreach (var value in values) {
          if (value is System.Tuple<object, object> t) tuple.AppendFormat("{0}: {1}", t.Item1, t.Item2);
          else tuple.Append(value);
          if (count++ < (values.Count - 1)) tuple.Append(", ");
        }
        tuple.Append(")");
        return tuple.ToString();
      }
      public override bool Equals(object obj) {
        if (obj is null) return false;
        if (obj is Tuple tuple) {
          return ToString() == tuple.ToString();
        }
        return false;
      }

      public override int GetHashCode() {
        unchecked {
          int hash = 17;
          hash = hash * 23 + Values.GetHashCode();
          return hash;
        }
      }

      //private class TupleComparer : IComparer<List<object>> {
      //  public int Compare(List<object> x, List<object> y) {
      //    if (object)
      //  }
      //}

      private static bool ScrambledEquals<T>(IEnumerable<T> a, IEnumerable<T> b) {
        var cnt = new Dictionary<T, int>();
        foreach (T s in a) {
          if (cnt.ContainsKey(s)) {
            cnt[s]++;
          } else {
            cnt.Add(s, 1);
          }
        }
        foreach (T s in b) {
          if (cnt.ContainsKey(s)) {
            cnt[s]--;
          } else {
            return false;
          }
        }
        return cnt.Values.All(c => c == 0);
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

    public static bool IsNumber(params object[] numbers) {
      foreach (var number in numbers) {
        if (!((number is int) || (number is double) || (number is Number))) {
          return false;
        }
      }
      return true;
    }

    public static bool IsString(params object[] strings) {
      foreach (var @string in strings) {
        if (!((@string is string) || (@string is String))) {
          return false;
        }
      }
      return true;
    }
  }
}

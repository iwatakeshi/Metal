using System;
using System.Collections.Generic;
using Metal.Intermediate;
using Metal.FrontEnd.Grammar;
using Metal.FrontEnd.Interpret;
using Metal.FrontEnd.Exceptions;

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
        this.value = value;
      }

      public static Number operator +(Number left, Number right) {
        if (left.Value is int && right.Value is int) {
          int.TryParse(left.ToString(), out int a);
          int.TryParse(right.ToString(), out int b);
          return new Number(a + b);
        }
        double.TryParse(left.ToString(), out double c);
        double.TryParse(left.ToString(), out double d);
        return new Number(c + d);
      }

      public static Number operator -(Number left, Number right) {
        if (left.Value is int && right.Value is int) {
          int.TryParse(left.ToString(), out int a);
          int.TryParse(right.ToString(), out int b);
          return new Number(a - b);
        }
        double.TryParse(left.ToString(), out double c);
        double.TryParse(left.ToString(), out double d);
        return new Number(c - d);
      }

      public static Number operator *(Number left, Number right) {
        if (left.Value is int && right.Value is int) {
          int.TryParse(left.ToString(), out int a);
          int.TryParse(right.ToString(), out int b);
          return new Number(a * b);
        }
        double.TryParse(left.ToString(), out double c);
        double.TryParse(left.ToString(), out double d);
        return new Number(c * d);
      }

      public static Number operator /(Number left, Number right) {
        if (left.Value is int && right.Value is int) {
          int.TryParse(left.ToString(), out int a);
          int.TryParse(right.ToString(), out int b);
          return new Number(a / b);
        }
        double.TryParse(left.ToString(), out double c);
        double.TryParse(left.ToString(), out double d);
        return new Number(c / d);
      }

      public static Boolean operator >(Number left, Number right) {
        double.TryParse(left.ToString(), out double a);
        double.TryParse(left.ToString(), out double b);
        return new Boolean(a > b);
      }

      public static Boolean operator <(Number left, Number right) {
        double.TryParse(left.ToString(), out double a);
        double.TryParse(left.ToString(), out double b);
        return new Boolean(a < b);
      }

      public static Boolean operator >=(Number left, Number right) {
        double.TryParse(left.ToString(), out double a);
        double.TryParse(left.ToString(), out double b);
        return new Boolean(a >= b);
      }

      public static Boolean operator <=(Number left, Number right) {
        double.TryParse(left.ToString(), out double a);
        double.TryParse(left.ToString(), out double b);
        return new Boolean(a <= b);
      }

      public static Boolean operator ==(Number left, Number right) {
        double.TryParse(left.ToString(), out double a);
        double.TryParse(left.ToString(), out double b);
        return new Boolean(a == b);
      }

      public static Boolean operator !=(Number left, Number right) {
        double.TryParse(left.ToString(), out double a);
        double.TryParse(left.ToString(), out double b);
        return new Boolean(a != b);
      }


      public override string ToString() {
        return value.ToString();
      }
      public override string TypeName => "number";

      public Boolean IsFloatingPoint() {
        return new Boolean(this.ToString().Contains("."));
      }
    }

    public class String : MetalType {
      private string value;
      public string Value => value;

      public String(string value) {
        this.value = value;
      }
      public static String operator +(String left, String right) {
        return new String(left.Value + right.Value);
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

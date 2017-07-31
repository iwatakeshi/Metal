using System;
using System.Collections.Generic;
using Metal.Intermediate;
using Metal.FrontEnd.Grammar;
using Metal.FrontEnd.Interpret;

namespace Metal.FrontEnd.Analyze {
  public abstract class MetalType : Object {
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
    }

    public class Function : MetalType, ICallable {
      private Statement.Function declaration;

      Func<Interpreter, List<object>, object> callee;
      internal Function(Statement.Function declaration) {
        this.declaration = declaration;
        callee = (interpreter, arguments) => {
            MetalEnvironment environment = new MetalEnvironment();
            if (declaration != null) {
              for (var i = 0; i < declaration.Parameters.Count; i++) {
                environment.Define(declaration.Parameters[i].Lexeme, arguments[i]);
              }
            }

            if (declaration != null) {
              interpreter.ExecuteBlock(declaration.Body, environment);
            }
            return null;
          
        };
      }

      public int Arity { get { return declaration.Parameters.Count; } }

      Func<Interpreter, List<object>, object> ICallable.Call => callee;

      public override string ToString() {
        return string.Format("<func {0}>", declaration.Name.Lexeme);
      }
    }
  }
}

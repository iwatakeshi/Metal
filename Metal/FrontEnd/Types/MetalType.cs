﻿using System;
using System.Collections.Generic;
using Metal.Intermediate;
using Metal.FrontEnd.Grammar;
using Metal.FrontEnd.Interpret;
using Metal.FrontEnd.Exceptions;

namespace Metal.FrontEnd.Types {
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
    }
  }
}
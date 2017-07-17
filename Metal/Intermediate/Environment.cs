using Metal.Diagnostics.Runtime;
using Metal.FrontEnd.Scan;
using System;
using System.Collections.Generic;

namespace Metal.Intermediate {
  internal class MetalEnvironment {
    private readonly Dictionary<string, object> values = new Dictionary<string, object>();
    private readonly MetalEnvironment enclosing;

    internal MetalEnvironment() {
      this.enclosing = null;
    }
    internal MetalEnvironment(MetalEnvironment enclosing) {
      this.enclosing = enclosing;
    }
    internal void Define(string name, object value) {
      values[name] = value;
    }

    internal object Get(Token name) {
      if(values.ContainsKey(name.Lexeme)) {
        return values[name.Lexeme];
      }

      if (enclosing != null) return enclosing.Get(name);

      throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }

    internal void Assign(Token name, object value) {
      if (values.ContainsKey(name.Lexeme)) {
        values[name.Lexeme] = value;
        return;
      }
      if (enclosing != null) {
        enclosing.Assign(name, value);
        return;
      }
      throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }
  }
}

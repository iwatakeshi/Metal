using Metal.FrontEnd.Exceptions;
using Metal.FrontEnd.Scan;
using System;
using System.Collections.Generic;

namespace Metal.Intermediate {
  public class MetalEnvironment {
    private Dictionary<string, object> values = new Dictionary<string, object>();
    private MetalEnvironment enclosing;

    public MetalEnvironment() {
      this.enclosing = null;
    }
    public MetalEnvironment(MetalEnvironment enclosing) {
      this.enclosing = enclosing;
    }
    public void Define(string name, object value) {
      values[name] = value;
    }

    public object Get(Token name) {
      if(values.ContainsKey(name.Lexeme)) {
        return values[name.Lexeme];
      }

      if (enclosing != null) return enclosing.Get(name);

      throw new MetalException.Runtime(name, "Undefined variable '" + name.Lexeme + "'.");
    }

    public void Assign(Token name, object value) {
      if (values.ContainsKey(name.Lexeme)) {
        values[name.Lexeme] = value;
        return;
      }
      if (enclosing != null) {
        enclosing.Assign(name, value);
        return;
      }
      throw new MetalException.Runtime(name, "Undefined variable '" + name.Lexeme + "'.");
    }
  }
}

using Metal.FrontEnd.Scan;
using System;

namespace Metal.Diagnostics.Runtime {

  public abstract class MetalException : Exception {
    public MetalException() : base() { }
    public MetalException(string message) : base(message) { }
  }
  
  public class RuntimeError: MetalException {
    Token token;
    public RuntimeError() : base() { }
    public RuntimeError(string message) : base(message) {}

    public RuntimeError(Token token, string message) : base(message) {
      this.token = token;
    }

    public Token Token => token;
  }
}

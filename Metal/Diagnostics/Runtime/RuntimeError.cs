using Metal.FrontEnd.Scan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metal.Diagnostics.Runtime {
  public class RuntimeError: Exception {
    Token token;
    public RuntimeError(Token token, string message) : base(message) {
      this.token = token;
    }

    public Token Token => token;
  }
}

using System;
using Metal.FrontEnd.Scan;

namespace Metal.FrontEnd.Exceptions {
  public class MetalException : Exception {
    public MetalException() { }
    public MetalException(string message) : base(message) { }

    /* General Exceptions */

    public class Parse : MetalException {
      public Parse() { }
      public Parse(string message) : base(message) { }
    }

    public class Runtime : MetalException {
      Token token;
      public Runtime() { }
      public Runtime(string message) : base(message) { }
      public Runtime(Token token, string message) : base(message) {
        this.token = token;
      }
      public Token Token => token;


      /* Run-time exceptions */
      public class Return : Runtime {
        private object value;
        public object Value => value;
        public Return(object value): base(null, null) {
          this.value = value;
        }

        public Return(Token token, string message) : base(token, message) { }
      }
    }
  }


}

using Metal.FrontEnd.Scan;

namespace Metal.FrontEnd.Parse.Grammar {
  internal abstract class AST<S> {
    internal abstract S Left { get; }
    internal abstract S Right { get;  }
    internal abstract Token Operator { get;  }
  }
}

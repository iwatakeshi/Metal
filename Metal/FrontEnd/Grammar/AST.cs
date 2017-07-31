using Metal.FrontEnd.Scan;

namespace Metal.FrontEnd.Grammar {
  public abstract class AST<S> {
    public abstract S Left { get; }
    public abstract S Right { get;  }
    public abstract Token Operator { get;  }
  }
}

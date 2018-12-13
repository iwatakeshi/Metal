package metal.frontend.scanner;

public enum TokenType {
  /* Literals */
  
  // "Hello, World!" or 'Hello, World!'
  StringLiteral,
  // 12
  NumberLiteral,
  // true
  BooleanLiteral,
  // null
  NullLiteral,

  /* Identifier */
  Identifier,

  /* Reserved */
  Reserved,

  /* Operator */
  Operator,

  /* Punctuations */
  Punctuation,

  /* Others */
  Invalid,
  EOF,
  None
}
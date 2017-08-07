using System;

namespace Metal.FrontEnd.Scan {
  public enum TokenType {

    /* Literals */
    // "Hello, World!" or 'Hello, World!'
    StringLiteral,
    // 12
    NumberLiteral,
    // 3.14
    FloatingPointLiteral,
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
    // '(':
    LeftParenthesisPunctuation,
    // ')':
    RightParenthesisPunctuation,
    // '{':
    LeftBracePunctuation,
    // '}':
    RightBracePunctuation,
    // '[':
    LeftBracketPunctuation,
    // ']':
    RightBracketPunctuation,
    // ',':
    CommaPunctuation,
    // ':':
    ColonPunctuation,
    // ';':
    SemiColonPunctuation,
    // '@':
    AtPunctuation,

    /* Others */
    Invalid,
    EOF,
  }

  public class Token {
    private TokenType type;
    private string lexeme;
    private object literal;
    public Token(TokenType type, String lexeme, Object literal, int line, int column) {
      this.type = type;
      this.lexeme = lexeme;
      this.literal = literal;
      Line = line;
      Column = column;
    }

    public override String ToString() {
      return string.Format("{0} '{1}' {2}", type, lexeme, literal);
    }

    /// <summary>
    /// Gets the type of the current token.
    /// </summary>
    /// <value>The type.</value>
    public TokenType Type { get { return type; } }
    /// <summary>
    /// Gets the lexeme of the current token.
    /// </summary>
    public string Lexeme { get { return lexeme; } }
    /// <summary>
    /// Gets the literal value of the current token.
    /// </summary>
    public object Literal { get { return literal; } }
    /// <summary>
    /// Gets the line number of the current token.
    /// </summary>
    public int Line { get; private set; }
    /// <summary>
    /// Gets the column number of the current token.
    /// </summary>
    public int Column { get; private set; }


    /// <summary>
    /// Determines whether this instance is a keyword.
    /// </summary>
    /// <returns><c>true</c> if this instance is a keyword; otherwise, <c>false</c>.</returns>
    public bool IsReserved() {
      return Token.IsReservedWord(lexeme);
    }
    public bool IsReserved(string lexeme) {
      return Token.IsReservedWord(this.lexeme) && this.lexeme == lexeme;
    }

    /// <summary>
    /// Determines if the specified key is a keyword.
    /// </summary>
    /// <returns><c>true</c> if the specified key is a keyword; otherwise, <c>false</c>.</returns>
    /// <param name="key">Key.</param>
    public static bool IsReservedWord(string key) {
      switch (key) {
        /* Reserved words used in declarations */
        case "class":
        case "enum":
        case "extends":
        case "func":
        case "import":
        case "export":
        case "init":
        case "let":
        case "var":
        case "private":
        case "public":
        /* Reserved words used in statements for control-flow and loops */
        case "break":
        case "case":
        case "continue":
        case "default":
        case "do":
        case "else":
        case "for":
        case "if":
        case "in":
        case "return":
        case "switch":
        case "while":
        case "repeat":
        case "yield":
        /* Reserved words used in expressions and types */
        case "catch":
        case "false":
        case "null":
        case "super":
        case "this":
        case "throw":
        case "true":
        case "try":
        case "string":
        case "number":
        case "char":
        case "int":
        case "double":
        case "bool":
        /* Reserved words in particular contexts */
        case "get":
        case "set":
        /* Reserved words in operators */
        case "and":
        case "or":
        case "xor":
          return true;
        default:
          return false;
      }
    }

    public bool IsLiteral() {
      return IsLiteralString(lexeme);
    }

    public bool IsLiteral(string lexeme) {
      return IsLiteralString(this.lexeme) && this.lexeme == lexeme;
    }

    public static bool IsLiteralString(string key) {
      switch (key) {
        case "null":
        case "true":
        case "false":
          return true;
        default:
          return false;
      }
    }
    /// <summary>
    /// Determines whether this instance is a punctuation.
    /// </summary>
    /// <returns><c>true</c> if this instance is a punctuation; otherwise, <c>false</c>.</returns>
    public bool IsPunctuation() {
      return IsPunctuationChar(lexeme);
    }
    public bool IsPunctuation(string lexeme) {
      return IsPunctuationChar(this.lexeme) && this.lexeme == lexeme;
    }
    /// <summary>
    /// Determines if the specified key is a punctuation .
    /// </summary>
    /// <returns><c>true</c> if the specified key is a punctuation; otherwise, <c>false</c>.</returns>
    /// <param name="key">Key.</param>
    public static bool IsPunctuationChar(string key) {
      switch (key) {
        case "(":
        case ")":
        case "{":
        case "}":
        case "[":
        case "]":
        case ",":
        case ":":
        case ";":
        case "@":
        case "#":
        case "&":
          return true;
        default:
          return false;
      }
    }

    /// <summary>
    /// Determines whether this instance is an operator.
    /// </summary>
    /// <returns><c>true</c> if this instance is an operator; otherwise, <c>false</c>.</returns>
    public bool IsOperator() {
      return IsOperatorString(lexeme);
    }

    public bool IsOperator(string lexeme) {
      return IsOperatorString(this.lexeme) && this.lexeme == lexeme;
    }

    /// <summary>
    /// Determines if the specified key is an operator.
    /// </summary>
    /// <returns><c>true</c> if the specified key is an operator; otherwise, <c>false</c>.</returns>
    /// <param name="key">Key.</param>
    public static bool IsOperatorString(string key) {
      switch (key) {
        /* Assignment Operator */
        case "=":

        /* Arithmentic Operators */
        // Addition / Unary Plus Operator
        case "+":
        // Subtraction / Unary Minus Operator
        case "-":
        // Multiplication
        case "*":
        // Division
        case "/":

        /* Remainder Operator */
        case "%":

        /* Increment and Decrement Operators */
        // Increment
        case "++":
        // Decrement
        case "--":

        /* Compound Assignment Operators */
        // Addition assignment
        case "+=":
        // Subtraction assignment
        case "-=":
        // Multiplication assignment
        case "*=":
        // Division assignment
        case "/=":
        // Modulus assignment
        case "%=":
        // Logical AND assignment
        case "&&=":
        // Logical OR assignment
        case "||=":
        // Bitwise AND assignment
        case "&=":
        // BItwise OR assignment
        case "|=":

        /* Comparison Operators */
        // Equal to
        case "==":
        // Not equal to
        case "!=":
        // Greater than
        case ">":
        // Less than
        case "<":
        // Greater than or equal to
        case ">=":
        // Less than or equal to
        case "<=":

        /* Identity Operators */
        // Strict equal to
        case "===":
        // Strict not equal to
        case "!==":

        /* Ternary Operator */
        // Ternary if
        case "?":

        /* Logical Operator */
        // AND
        case "and":
        //// OR
        case "or":
        //// NOT 
        case "!":

        /* Lambda Operator */
        case "=>":

        /* Dot Operator */
        case ".":

        /* Bitwise Operators */
        // NOT
        //case "~":
        //// AND
        //case "&":
        //// OR
        //case "|":
        //// XOR 
        //case "^":
        //// Left Shift
        //case "<<":
        //// Right Shift
        //case ">>":
        /* Range Operator */
        case "..":
        /* Spread Operator */
        case "...":
        /* As Operator */
        case "as":
        /* Is Operator */
        case "is":
        /* In Operator */
        case "in":
          return true;
        default:
          return false;
      }
    }
    public bool Equals(Token token) {
      return this.type == token.type && this.lexeme == token.lexeme;
    }
    public bool Equals(TokenType type) {
      return this.type == type;
    }
    public bool Equals(TokenType type, string lexeme) {
      return this.type == type && this.lexeme == lexeme;
    }
  }
}


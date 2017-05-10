using System;
using Metal.IO;
using Newtonsoft.Json.Linq;

namespace Metal.FrontEnd.Scan {
  public enum TokenType {
        
    /* Literals */
    // "Hello, World!" || 'Hello, World!'
    StringLiteral,
    // 12
    IntegerLiteral,
    // 3.14
    FloatingPointLiteral,
    // true
    BooleanLiteral,
        
    /* Identifier */
    Identifier,
    
    /* Keyword */
    Keyword,
    
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
    TokenType type;
    String lexeme;
    Object literal;
    public Token () {
    }

    public Token (TokenType type) {
      this.type = type;
    }

    public Token (TokenType type, String lexeme) {
      this.type = type;
      this.lexeme = lexeme;
    }

    public Token (TokenType type, String lexeme, Object literal) {
      this.type = type;
      this.lexeme = lexeme;
      this.literal = literal;
    }

    /// <summary>
    /// Gets the type of the current token.
    /// </summary>
    /// <value>The type.</value>
    public TokenType Type { get { return type; } }

    /// <summary>
    /// Determines whether this instance is a keyword.
    /// </summary>
    /// <returns><c>true</c> if this instance is a keyword; otherwise, <c>false</c>.</returns>
    public bool IsKeyword () {
      return IsKeyword (lexeme);
    }

    /// <summary>
    /// Determines if the specified key is a keyword.
    /// </summary>
    /// <returns><c>true</c> if the specified key is a keyword; otherwise, <c>false</c>.</returns>
    /// <param name="key">Key.</param>
    public static bool IsKeyword (string key) {
      switch (key) {
      /* Keywords used in declarations */
      case "class":
      case "enum":
      case "extends":
      case "fn":
      case "import":
      case "export":
      case "init":
      case "let":
      case "var":
      case "private":
      case "public":
      /* Keywords used in statements */
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
      case "yield":
      /* Keywords used in expressions and types */
      case "catch":
      case "false":
      case "null":
      case "super":
      case "this":
      case "throw":
      case "true":
      case "try":
      /* Keywords reserved in particular contexts */
      case "get":
      case "set":
        return true;
      default:
        return false;
      }
    }

    /// <summary>
    /// Determines whether this instance is a punctuation.
    /// </summary>
    /// <returns><c>true</c> if this instance is a punctuation; otherwise, <c>false</c>.</returns>
    public bool IsPunctuation () {
      return IsPunctuation (lexeme);
    }

    /// <summary>
    /// Determines if the specified key is a punctuation .
    /// </summary>
    /// <returns><c>true</c> if the specified key is a punctuation; otherwise, <c>false</c>.</returns>
    /// <param name="key">Key.</param>
    public static bool IsPunctuation (string key) {
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
      case "=":
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
    public bool IsOperator () {
      return IsOperator (lexeme);
    }

    /// <summary>
    /// Determines if the specified key is an operator.
    /// </summary>
    /// <returns><c>true</c> if the specified key is an operator; otherwise, <c>false</c>.</returns>
    /// <param name="key">Key.</param>
    public static bool IsOperator (string key) {
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
      case "&&":
      // OR
      case "||":
      // NOT 
      case "!":
      
      /* Lambda Operator */
      case "=>":
        
      /* Dot Operator */
      case ".":
        
      /* Bitwise Operators */
      // NOT
      case "~":
      // AND
      case "&":
      // OR
      case "|":
      // XOR 
      case "^":
      // Left Shift
      case "<<":
      // Right Shift
      case ">>":
      /* Spread Operator */
      case "...":
      /* As Operator */
      case "as":
      /* Is Operator */
      case "is":
        return true;
      default:
        return false;
      }
    }

    /// <summary>
    /// Determines whether this instance is reserved.
    /// </summary>
    /// <returns><c>true</c> if this instance is reserved; otherwise, <c>false</c>.</returns>
    public bool IsReserved () {
      return IsReserved (lexeme);
    }

    /// <summary>
    /// Determines if the specified key is a reserved word.
    /// </summary>
    /// <returns><c>true</c> if the specified key is reserved; otherwise, <c>false</c>.</returns>
    /// <param name="key">Key.</param>
    public static bool IsReserved (string key) {
      return IsOperator (key) || IsPunctuation (key) || IsKeyword (key);
    }
  }
}


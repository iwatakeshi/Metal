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
    Source source;
    TokenType type;
    String value;

    public Token () {
    }

    public Token (TokenType token, Source source) {
      this.type = token;
      this.source = source;
    }

    public Token (TokenType type, String value, Source source) {
      this.type = type;
      this.value = value;
      this.source = source;
    }

    /// <summary>
    /// Gets the source.
    /// </summary>
    /// <value>The source.</value>
    Source Source { get { return source; } }

    /// <summary>
    /// Gets the token name.
    /// </summary>
    /// <value>The name.</value>
    public string Name {
      get { 
        return GetName (type);
      }
    }

    /// <summary>
    /// Gets the token name.
    /// </summary>
    /// <returns>The name.</returns>
    /// <param name="key">Key.</param>
    public static string GetName (TokenType key) {
      switch (key) {
      /* Literals */
      case TokenType.StringLiteral:
        return "String Literal";
      case TokenType.IntegerLiteral:
        return "Integer Literal";
      case TokenType.FloatingPointLiteral:
        return "Floating Point Literal";
      case TokenType.BooleanLiteral:
        return "Boolean Literal";
      /* Operator */
      case TokenType.Operator:
        return "Operator";
      /* Punctuations */
      case TokenType.LeftParenthesisPunctuation:
        return "Left Parenthesis Punctuation";
      case TokenType.RightParenthesisPunctuation:
        return "Right Parenthesis Punctuation";
      case TokenType.LeftBracePunctuation:
        return "Left Brace Punctuation";
      case TokenType.RightBracePunctuation:
        return "Right Brace Punctuation";
      case TokenType.LeftBracketPunctuation:
        return "Left Bracket Punctuation";
      case TokenType.RightBracketPunctuation:
        return "Right Bracket Punctuation";
      case TokenType.CommaPunctuation:
        return "Comma Punctuation";
      case TokenType.ColonPunctuation:
        return "Colon Punctuation";
      case TokenType.SemiColonPunctuation:
        return "Semi-Colon Punctuation";
      case TokenType.AtPunctuation:
        return "At Punctuation";
      case TokenType.Identifier:
        return "Identifier";
      case TokenType.Keyword:
        return "Keyword";
      case TokenType.Invalid:
        return "Invalid Token";
      case TokenType.EOF:
        return "End of File";
      default:
        return "";
      }
    }

    /// <summary>
    /// Gets the type of the current token.
    /// </summary>
    /// <value>The type.</value>
    public TokenType Type { get { return type; } }

    /// <summary>
    /// Gets the value of the current token.
    /// </summary>
    /// <value>The value.</value>
    public string Value { get { return value; } }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="Metal.FrontEnd.Lex.Token"/>.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="Metal.FrontEnd.Lex.Token"/>.</returns>
    public override string ToString () {
      return string.Format ("Type: {0}, Value: {1}, Is Reserved: {2}, {3}", Name, value, IsReserved (value), source.ToString ());
    }

    /// <summary>
    /// Returns a JSON object that represents the current <see cref="Metal.FrontEnd.Lex.Token"/>.
    /// </summary>
    /// <returns>The JSON object.</returns>
    public object ToJson () {
      String val;
      switch (type) {
      case TokenType.StringLiteral:
      case TokenType.Identifier:
      case TokenType.Keyword:
      case TokenType.Operator:
      case TokenType.AtPunctuation:
      case TokenType.ColonPunctuation:
      case TokenType.CommaPunctuation:
      case TokenType.LeftBracePunctuation:
      case TokenType.LeftBracketPunctuation:
      case TokenType.LeftParenthesisPunctuation:
      case TokenType.RightBracePunctuation:
      case TokenType.RightBracketPunctuation:
      case TokenType.RightParenthesisPunctuation:
      case TokenType.SemiColonPunctuation:
        val = "\"" + value + "\"";
        break;
      case TokenType.IntegerLiteral:
      case TokenType.FloatingPointLiteral:
      case TokenType.BooleanLiteral:
        val = value;
        break;
      default:
        val = "";
        break;
      }
      var str = string.Format ("{{ \"token\": {{ \"type\": \"{0}\", \"value\": {1}, \"isReserved\": {2} }} }}", 
        Name, val, IsReserved (val).ToString ().ToLower ());
      var mergeSettings = new JsonMergeSettings {
        MergeArrayHandling = MergeArrayHandling.Union
      };

      var json = JObject.Parse (str);
      json.Merge (source.ToJson (), mergeSettings);
      return json;
    }

    /// <summary>
    /// Determines whether this instance is a keyword.
    /// </summary>
    /// <returns><c>true</c> if this instance is a keyword; otherwise, <c>false</c>.</returns>
    public bool IsKeyword () {
      return IsKeyword (value);
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
      return IsPunctuation (value);
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
      return IsOperator (value);
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
      return IsReserved (value);
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


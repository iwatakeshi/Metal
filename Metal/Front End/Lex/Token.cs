using System;
using Metal.IO;
using Newtonsoft.Json.Linq;

namespace Metal.FrontEnd.Lex {
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
    
    /* Operators */
    // '='
    AssignmentOperator,
    // '+'
    AdditionOperator,
    UnaryPlusOperator,
    // '-'
    SubtractionOperator,
    UnaryMinusOperator,
    // '*'
    MuliplicationOperator,
    // '/'
    DivisionOperator,
    // '%'
    RemainderOperator,
    // '++'
    IncrementOperator,
    // '--'
    DecrementOperator,
    // '+='
    AdditionAssignmentOperator,
    // '-='
    SubtractionAssignmentOperator,
    // '*='
    MultiplicationAssignmentOperator,
    // '/='
    DivisionAssignmentOperator,
    // '=='
    EqualToComparisonOperator,
    // '!='
    NotEqualToComparisonOperator,
    // '>'
    GreaterThanComparisonOperator,
    // '<'
    LessThanComparisonOperator,
    // '>='
    GreaterThanEqualToComparisonOperator,
    // '<='
    LessThanEqualToComparisonOperator,
    // '==='
    StrictEqualToIdentityOperator,
    // '!=='
    StrictNotEqualToIdentityOperator,
    // '?'
    IfTernaryOperator,
    // '&&'
    AndLogicalOperator,
    // 'Or'
    OrLogicalOperator,
    // '!'
    NotLogicalOperator,
    // '->':
    LambdaOperator,
    // '.':
    DotOperator,
    
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

    Source Source { get { return source; } }

    /// <summary>
    /// Gets the name of the current token.
    /// </summary>
    /// <value>The name.</value>
    public string Name {
      get { 
        return GetName(type);
      }
    }
    public static string GetName (TokenType key){
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
        /* Operators */
        case TokenType.AssignmentOperator:
          return "Assignment Operator";
        case TokenType.AdditionOperator:
          return "Addition Operator";
        case TokenType.SubtractionOperator:
          return "Subtraction Operator";
        case TokenType.MuliplicationOperator:
          return "Multiplication Operator";
        case TokenType.DivisionOperator:
          return "Division Operator";
        case TokenType.AdditionAssignmentOperator:
          return "Addition Assignment Operator";
        case TokenType.SubtractionAssignmentOperator:
          return "Subtraction Assignment Operator";
        case TokenType.MultiplicationAssignmentOperator:
          return "Multiplication Assignment Operator";
        case TokenType.DivisionAssignmentOperator:
          return "Division Assignment Operator";
        case TokenType.EqualToComparisonOperator:
          return "Equal-To Comparison Operator";
        case TokenType.NotEqualToComparisonOperator:
          return "Not Equal-To Comparison Operator";
        case TokenType.GreaterThanComparisonOperator:
          return "Greater-Than Comparison Operator";
        case TokenType.LessThanComparisonOperator:
          return "Less-Than Comparison Operator";
        case TokenType.GreaterThanEqualToComparisonOperator:
          return "Greater-Than Or Equal-To Comparision Operator";
        case TokenType.LessThanEqualToComparisonOperator:
          return "Less-Than Or Equal-To Comparison Operator";
        case TokenType.StrictEqualToIdentityOperator:
          return "Strict Equal-To Identity Operator";
        case TokenType.StrictNotEqualToIdentityOperator:
          return "Strict Not Equal-To Identity Operator";
        case TokenType.IfTernaryOperator:
          return "If Ternary Operator";
        case TokenType.AndLogicalOperator:
          return "AND Logical Operator";
        case TokenType.OrLogicalOperator:
          return "OR Logical Operator";
        case TokenType.NotLogicalOperator:
          return "Not Logical Operator";
        case TokenType.LambdaOperator:
          return "Lambda Operator";
        case TokenType.DotOperator:
          return "Dot Operator";
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
      return string.Format ("Type: {0}, Value: {1}, Is Reserved: {2}, {3}", Name, value, IsReserved(value), source.ToString ());
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
      var str = string.Format ("{{ \"token\": {{ \"type\": \"{0}\", \"value\": {1}, \"isReserved\": {2} }} }}", Name, val, IsReserved (val));
      var mergeSettings = new JsonMergeSettings {
        MergeArrayHandling = MergeArrayHandling.Union
      };

      var json = JObject.Parse (str);
      json.Merge (source.ToJson (), mergeSettings);
      return json;
    }

    /// <summary>
    /// Determines if the key is the specified key of keywords.
    /// </summary>
    /// <returns><c>true</c> if the key is the specified key of keywords; otherwise, <c>false</c>.</returns>
    /// <param name="key">Key.</param>
    public static bool IsKeyword (string key) {
      switch (key) {
      /* Keywords used in declarations */
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
      case "as":
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

    public static bool IsPunctuation (string key) {
      switch (key) {
      case "(":
      case ")":
      case "{":
      case "}":
      case "[":
      case "]": 
      case ",":
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
        return true;
      default:
        return false;
      }
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


using System;
using Metal.IO;
using System.Text;
using log4net;

namespace Metal.FrontEnd.Scan {
  /// <summary>
  /// Transforms a stream of characters into a stream of words.
  /// </summary>
  public class Scanner {

    /* Private variables */
    StringBuilder buffer;
    Source source;

    public Scanner (string fileName) {
      buffer = new StringBuilder ();
      source = new Source (fileName);
      log4net.Config.BasicConfigurator.Configure ();
      Log = log4net.LogManager.GetLogger (typeof(Scanner));
    }

    public Scanner (string path, string fileName) {
      buffer = new StringBuilder ();
      source = new Source (path, fileName);
      log4net.Config.BasicConfigurator.Configure ();
      Log = log4net.LogManager.GetLogger (typeof(Scanner));
    }

    public TokenStream Scan () {
      TokenStream stream = new TokenStream ();
      IgnoreWhiteSpace ();
      while (PeekChar () != Source.EOF) {
        var token = NextToken ();
        if (token != null) stream.AddToken (token);
        IgnoreWhiteSpace ();
      }
      return stream;
    }

    /// <summary>
    /// Gets the next the token.
    /// </summary>
    /// <returns>The token.</returns>
    private Token NextToken () {
      buffer.Clear ();
      var character = PeekChar ();
      switch (character) {
      case '#':
        return ScanComment ();
      case '\'':
      case '"':
        return ScanStringLiteral ();
      case '_':
        return ScanIdentifier ();
      case '$':
        return ScanIdentifier ();
      case '0':
      case '1':
      case '2':
      case '3':
      case '4':
      case '5':
      case '6':
      case '7':
      case '8':
      case '9':
        return ScanNumber ();
      case '+':
      case '-':
      case '*':
      case '/':
      case '.':
      case '=':
      case '<':
      case '>':
      case '~':
      case '!':
      case '&':
      case '^':
      case '|':
      case '%':
      case '@':
      case '?':
        if (PeekChar () == '/' && PeekChar (1) == '*')
          return ScanComment ();
        else return ScanOperator ();
      case ':':
      case '{':
      case '}':
      case '(':
      case ')':
      case '[':
      case ']':
      case ';':
      case ',':
        return ScanPunctuation ();
      default:
        if (char.IsLetter (character)) {
          if(PeekChar () == 'a' && PeekChar (1) == 's' || PeekChar () == 'i' && PeekChar (1) == 's')
            return ScanOperator();
          else return ScanIdentifier ();
        }
        Log.Error (String.Format ("Metal [Error]: Invalid character encountered {0} on line {1}.", character.ToString (), Source.Line));
        return null;
      }
    }

    /// <summary>
    /// Peeks the next character in the source text.
    /// </summary>
    /// <returns>The char.</returns>
    char PeekChar () {
      return PeekChar (0);
    }

    /// <summary>
    /// Peeks the the next character in the source text at a given index.
    /// </summary>
    /// <returns>The char.</returns>
    /// <param name="peek">Peek.</param>
    char PeekChar (int peek) {
      if (source.Position + peek >= source.Length)
        return Source.EOF;
      return source.File[source.Position + peek];
    }

    /// <summary>
    /// Gets the next character in the source text.
    /// </summary>
    /// <returns>The char.</returns>
    char NextChar () {
      if (source.Position >= Source.Length)
        return Source.EOF;
      if (source.File[source.Position] == NewLine) {
        source.Line++;
        source.Column = 0;
      } else source.Column++;
      return source.File[source.Position++];
    }

    /// <summary>
    /// Gets a value indicating whether this instance is EOF.
    /// </summary>
    /// <value><c>true</c> if this instance is EOF; otherwise, <c>false</c>.</value>
    public bool IsEOF { get { return PeekChar () == source.EOF; } }

    public Source Source { get { return source; } }

    bool IsWhiteSpace { get { return Char.IsWhiteSpace (PeekChar ()); } }

    char NewLine { get { return Environment.NewLine.ToCharArray ()[0]; } }

    /// <summary>
    /// Skips all white spaces and comments
    /// </summary>
    void IgnoreWhiteSpace () {
      for (;; NextChar ()) {
        if (IsWhiteSpace)
          continue;
        else break;
      }
    }

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    /// <value>The logger.</value>
    ILog Log { get; set; }

    /// <summary>
    /// Escape the character.
    /// </summary>
    string EscapeChar () {
      switch (PeekChar ()) {
      case 'b':
        NextChar ();
        return "\\b";
      case 't':
        NextChar ();
        return "\\t";
      case 'n':
        NextChar ();
        return "\\n";
      case 'f':
        NextChar ();
        return "\\f";
      case 'r':
        NextChar ();
        return "\\r";
      case '"':
        NextChar ();
        return "\"";
      case '\'':
        NextChar ();
        return "\\'";
      case '\\':
        NextChar ();
        return "\\\\";
      default:
        NextChar ();
        Log.Warn (String.Format ("Metal [Error]: Failed to escape \"{0}\" on line {1}.", PeekChar (), Source.Line));
        return "";
      }
    }

    /// <summary>
    /// Scans the identifier.
    /// </summary>
    /// <returns>The identifier token.</returns>
    private Token ScanIdentifier () {
      var character = PeekChar ();
      do {
        buffer.Append (NextChar ());
        character = PeekChar ();
      } while (Char.IsLetterOrDigit (character) || character == '_' || character == '$');
      
      if (Token.IsKeyword (buffer.ToString ())){
        if(buffer.ToString () == "true" || buffer.ToString () == "false")
          return new Token(TokenType.BooleanLiteral, buffer.ToString (), source);
        else return new Token (TokenType.Keyword, buffer.ToString (), source);
      }

        
      else return new Token (TokenType.Identifier, buffer.ToString (), source);
    }

    /// <summary>
    /// Scans the operator.
    /// </summary>
    /// <returns>The operator token.</returns>
    private Token ScanOperator () {
      var three = PeekChar ().ToString () + PeekChar (1).ToString () + PeekChar (2).ToString ();
      switch (three) {
      case "===":
      case "!==":
      case "...":
      case "&&=":
      case "||=":
        NextChar ();
        NextChar ();
        NextChar ();
        return new Token (TokenType.Operator, three, source);
      }
      var two = PeekChar ().ToString () + PeekChar (1).ToString ();
      switch (two) {
      case "as":
      case "is":
      case "++":
      case "--":
      case "+=":
      case "-=":
      case "*=":
      case "%=":
      case "/=":
      case "==":
      case "!=":
      case ">=":
      case "<=":
      case "&&":
      case "||":
      case "=>":
      case "<<":
      case ">>":
      case "&=":
      case "|=":
        NextChar ();
        NextChar ();
        return new Token (TokenType.Operator, two, source);
      }
      var one = PeekChar ().ToString ();
      switch (one) {
      case "=":
      case "+":
      case "-":
      case "*":
      case "/":
      case "%":
      case ">":
      case "<":
      case "?":
      case "!":
      case ".":
      case "~":
      case "&":
      case "|": 
      case "^":
        NextChar ();
        return new Token (TokenType.Operator, one, source);
      default:
        break;
      }
      return null;
    }

    /// <summary>
    /// Scans the punctuation.
    /// </summary>
    /// <returns>The punctuation token.</returns>
    private Token ScanPunctuation () {
      var character = PeekChar ();
      switch (character) {
      case '{':
        NextChar ();
        return new Token (TokenType.LeftBracePunctuation, character.ToString (), source);
      case '}':
        NextChar ();
        return new Token (TokenType.RightBracePunctuation, character.ToString (), source);
      case '(':
        NextChar ();
        return new Token (TokenType.LeftParenthesisPunctuation, character.ToString (), source);
      case ')':
        NextChar ();
        return new Token (TokenType.RightParenthesisPunctuation, character.ToString (), source);
      case '[':
        NextChar ();
        return new Token (TokenType.LeftBracketPunctuation, character.ToString (), source);
      case ']':
        NextChar ();
        return new Token (TokenType.RightBracketPunctuation, character.ToString (), source);
      case ';':
        NextChar ();
        return new Token (TokenType.SemiColonPunctuation, character.ToString (), source);
      case ':':
        NextChar ();
        return new Token (TokenType.ColonPunctuation, character.ToString (), source);
      case ',':
        NextChar ();
        return new Token (TokenType.CommaPunctuation, character.ToString (), source);
      }
      return null;
    }

    /// <summary>
    /// Scans the comment.
    /// </summary>
    /// <returns>Null.</returns>
    private Token ScanComment () {
      var character = PeekChar ();
      switch (character) {
      case '#':
        NextChar ();
        do {
          character = NextChar ();
        } while (character != Source.EOF && character != NewLine);
          
        break;
      case '/':
        NextChar ();
        if (PeekChar () == '*') {
          NextChar ();
          do {
            character = NextChar ();
          } while (character != Source.EOF &&
                   character != '*' && PeekChar () != '/');
          if (PeekChar () == '/') NextChar ();
        }
        break;
      }
      return null;
    }

    /// <summary>
    /// Scans the string literal.
    /// </summary>
    /// <returns>The string literal token.</returns>
    private Token ScanStringLiteral () {
      var delimiter = NextChar ();
      var ch = PeekChar ();
      while (ch != delimiter && ch != Source.EOF) {
        if (ch == '\\') {
          NextChar ();
          buffer.Append (EscapeChar ());
        } else {
          buffer.Append (NextChar ());
        }
        ch = PeekChar ();
      }
      if (NextChar () == Source.EOF) {
        var error = "Metal [Error]: Invalid string literal encountered on line {0}.";
        Log.Error (string.Format (error, source.Line));
      }
      return new Token (TokenType.StringLiteral, buffer.ToString (), source);
    }

    /// <summary>
    /// Scans the number.
    /// </summary>
    /// <returns>The number token.</returns>
    private Token ScanNumber () {
      StringBuilder number = new StringBuilder ();
      char ch = PeekChar ();
      if (ch == '0' && PeekChar (1) == 'x')
        return ScanHex (number);
      do {
        if (ch == '.')
          return ScanFloatingPoint (number);
        number.Append (NextChar ());
        ch = PeekChar ();
      } while (char.IsDigit (ch) || ch == '.');
      return new Token (TokenType.IntegerLiteral, number.ToString (), source);
    }

    /// <summary>
    /// Scans the hexidecimal number.
    /// </summary>
    /// <returns>The hexidecimal token.</returns>
    /// <param name="number">Number token.</param>
    private Token ScanHex (StringBuilder number) {
      NextChar ();
      NextChar ();
      while (IsHex (PeekChar ())) {
        number.Append (NextChar ());
      }

      return new Token (TokenType.IntegerLiteral, Int32.Parse (number.ToString (),
        System.Globalization.NumberStyles.HexNumber).ToString (), source);
    }

    /// <summary>
    /// Determines if the specified character is a hexidecimal number.
    /// </summary>
    /// <returns><c>true</c> if is hex the specified c; otherwise, <c>false</c>.</returns>
    /// <param name="c">C.</param>
    private static bool IsHex (char character) {
      return "ABCDEF0123456789".Contains (character.ToString ());
    }

    /// <summary>
    /// Scans the floating point number.
    /// </summary>
    /// <returns>The floating point number.</returns>
    /// <param name="number">Number token.</param>
    private Token ScanFloatingPoint (StringBuilder number) {
      NextChar ();
      number.Append (".");
      char ch = PeekChar ();
      do {
        number.Append (NextChar ());
        ch = PeekChar ();
      } while (char.IsDigit (ch));
      return new Token (TokenType.FloatingPointLiteral, number.ToString (), source);
    }
  }
}


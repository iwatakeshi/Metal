using System;
using Metal.IO;
using System.Text;
using log4net;

namespace Metal.FrontEnd.Lex {
  /// <summary>
  /// Transforms a stream of characters into a stream of words.
  /// </summary>
  public class Scanner {

    /* Private variables */
    StringBuilder buffer;
    Source source;

    public Scanner () {
      buffer = new StringBuilder ();

      log4net.Config.BasicConfigurator.Configure ();
      Log = log4net.LogManager.GetLogger (typeof(Scanner));
    }

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
        Console.WriteLine (token);
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
      char ch = PeekChar ();
      switch (ch) {
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
      case ':':
        if(PeekChar() == '/' && PeekChar(1) == '*')
          return ScanComment();
        else return ScanOperator();
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
        if (char.IsLetter (ch)) {
          return ScanIdentifier ();
        }
        Log.Error (String.Format ("Metal [Error]: Unexpected token {0} at line {1}.", NextChar (), Source.Line));
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
        if (PeekChar () == NewLine) {
        } else if (IsWhiteSpace)
          continue;
        else break;
      }
    }

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

    private Token ScanIdentifier () {
      var character = PeekChar ();
      do {
        buffer.Append (NextChar ());
        character = PeekChar();
      } while (Char.IsLetterOrDigit (character) || character == '_' || character == '$');
      Console.WriteLine("Consumed Identifier" + buffer.ToString ());
      if (Token.IsKeyword (buffer.ToString ()))
        return new Token (TokenType.Keyword, buffer.ToString (), source);
      else return new Token (TokenType.Identifier, buffer.ToString (), source);
    }

    private Token ScanOperator () {
      /* One Character Operators */
      string one = NextChar ().ToString ();
      string two = one + PeekChar ().ToString ();
      string three = two + PeekChar (1).ToString ();
      switch (one) {
      case "=":
        return new Token (TokenType.AssignmentOperator, one, source);
      case "+":
        return new Token (TokenType.AdditionOperator, one, source);
      case "-":
        return new Token (TokenType.SubtractionOperator, one, source);
      case "*":
        return new Token (TokenType.MuliplicationOperator, one, source);
      case "/":
        return new Token (TokenType.DivisionOperator, one, source);
      case "%":
        return new Token (TokenType.RemainderOperator, one, source);
      case ">":
        return new Token (TokenType.GreaterThanComparisonOperator, one, source);
      case "<":
        return new Token (TokenType.LessThanComparisonOperator, one, source);
      case "?":
        return new Token (TokenType.IfTernaryOperator, one, source);
      case "!":
        return new Token (TokenType.NotLogicalOperator, one, source);
      case ".":
        return new Token (TokenType.DotOperator, one, source);
      default:
        break;
      }
      switch (two) {     
      case "++":
        NextChar ();
        return new Token (TokenType.IncrementOperator, two, source);
      case "--":
        NextChar ();
        return new Token (TokenType.DecrementOperator, two, source);
      case "+=":
        NextChar ();
        return new Token (TokenType.AdditionAssignmentOperator, two, source);
      case "-=":
        NextChar ();
        return new Token (TokenType.SubtractionAssignmentOperator, two, source);
      case "*=":
        NextChar ();
        return new Token (TokenType.MultiplicationAssignmentOperator, two, source);
      case "/=":
        NextChar ();
        return new Token (TokenType.DivisionAssignmentOperator, two, source);
      case "==":
        NextChar ();
        return new Token (TokenType.EqualToComparisonOperator, two, source);
      case "!=":
        NextChar ();
        return new Token (TokenType.NotEqualToComparisonOperator, two, source);
      case ">=":
        NextChar ();
        return new Token (TokenType.GreaterThanEqualToComparisonOperator, two, source);
      case "<=":
        NextChar ();
        return new Token (TokenType.LessThanEqualToComparisonOperator, two, source);
      case "&&":
        NextChar ();
        return new Token (TokenType.AndLogicalOperator, two, source);
      case "||":
        NextChar ();
        return new Token (TokenType.OrLogicalOperator, two, source);
      case "=>":
        NextChar ();
        return new Token (TokenType.LambdaOperator, two, source);
      }
      switch (three) {
      case "===":
        NextChar ();
        NextChar ();
        return new Token (TokenType.StrictEqualToIdentityOperator, three, source);
      case "!==":
        NextChar ();
        NextChar ();
        return new Token (TokenType.StrictNotEqualToIdentityOperator, three, source);
      }
      return null;
    }

    private Token ScanPunctuation () {
      NextChar ();
      TokenType type = TokenType.Invalid;
      switch (PeekChar ()) {
      case '{':
        type = TokenType.LeftBracePunctuation;
        break;
      case '}':
        type = TokenType.RightBracePunctuation;
        break;
      case '(':
        type = TokenType.LeftParenthesisPunctuation;
        break;
      case ')':
        type = TokenType.RightParenthesisPunctuation;
        break;
      case '[':
        type = TokenType.LeftBracketPunctuation;
        break;
      case ']':
        type = TokenType.RightBracketPunctuation;
        break;
      case ';':
        type = TokenType.SemiColonPunctuation;
        break;
      case ':':
        type = TokenType.ColonPunctuation;
        break;
      case ',':
        type = TokenType.CommaPunctuation;
        break;
      }
      return new Token (type, Char.ToString (PeekChar ()), source);
    }

    private Token ScanComment () {
      var character = PeekChar ();
      switch (character) {
      case '#':
        do {
          character = NextChar ();
        } while (character != Source.EOF && character != NewLine);
          
        break;
      case '/':
        NextChar ();
        if (PeekChar () == '*') {
          NextChar ();
          do {
            character = NextChar();
          } while (character != Source.EOF && character != NewLine &&
                   character != '*' && PeekChar () != '/');
          if(PeekChar() == '/') NextChar();
        }
        break;
      }
      return null;
    }

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
        var error = "Metal [Error]: Invalid string literal on line {0}";
        Log.Error (string.Format (error, source.Line));
      }
      return new Token (TokenType.StringLiteral, buffer.ToString (), source);
    }

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

    private Token ScanHex (StringBuilder number) {
      NextChar ();
      NextChar ();
      while (IsHex (PeekChar ())) {
        number.Append (NextChar ());
      }

      return new Token (TokenType.IntegerLiteral, Int32.Parse (number.ToString (),
        System.Globalization.NumberStyles.HexNumber).ToString (), source);
    }

    private static bool IsHex (char c) {
      return "ABCDEF0123456789".Contains (c.ToString ());
    }

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


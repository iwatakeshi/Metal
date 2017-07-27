using System;
using System.Collections.Generic;
using Metal;
using Metal.IO;
using System.Text.RegularExpressions;

namespace Metal.FrontEnd.Scan {

  /// <summary>
  /// Transforms a stream of characters into a stream of words.
  /// </summary>
  public class Scanner {
    private Source source;
    private List<Token> tokens = new List<Token>();

    public Scanner(string source, bool isFile) {
      this.source = new Source(source, isFile);
    }

    public Scanner(string path, string source) {
      this.source = new Source(path, source);
    }

    public Scanner(string source, int line, int position, int column, bool isFile) {
      this.source = new Source(source, line, position, column, isFile);
    }

    public Scanner(string path, string source, int line, int position, int column) {
      this.source = new Source(path, source, line, position, column);
    }

    /// <summary>
    /// Scans all the tokens
    /// </summary>
    /// <returns></returns>
    internal List<Token> ScanTokens() {
      while (!IsAtEnd) {
        Start = source.Position;
        ScanToken();
      }
      tokens.Add(EOF);
      return tokens;
    }

    /// <summary>
    /// Scans a token while skipping all the tokens with null values.
    /// </summary>
    /// <returns>The current token.</returns>
    public Token ScanSafeToken() {
      Token current = EOF;
      while (!IsAtEnd && (current = ScanToken()) == null) ;
      if (IsAtEnd) tokens.Add(current);
      return current;
    }
    /// <summary>
    /// Scans a token.
    /// </summary>
    /// <returns>The current token.</returns>
    public Token ScanToken() {
      char ch = Next();
      switch (ch) {
        /* Puncutations */
        case '(': return AddToken(TokenType.LeftParenthesisPunctuation, ch);
        case ')': return AddToken(TokenType.RightParenthesisPunctuation, ch);
        case '{': return AddToken(TokenType.LeftBracePunctuation, ch);
        case '}': return AddToken(TokenType.RightBracePunctuation, ch);
        case ',': return AddToken(TokenType.CommaPunctuation, ch);
        case ';': return AddToken(TokenType.SemiColonPunctuation, ch);
        /* Operators */
        case '.': return AddToken(TokenType.Operator, Match('.') ? ".." : ".");
        case '?':
        case '+':
        case '-':
        case '*': return AddToken(TokenType.Operator, ch);
        case '/':
          // Multi-line Comment
          if (Current() == '*') {
            Next();
            while(!IsAtEnd) {
              if (Current() == '*' && Peek() == '/') break;
              Next();
            }
            if (Current() == '*' && Peek() == '/') {
              Next();
              Next();
              break;
            }
            Metal.Error(source.Line, "Unterminated block comment.");
            break;
          } else { return AddToken(TokenType.Operator, ch); }
        case '!': return AddToken(TokenType.Operator, Match('=') ? "!=" : "!");
        case '=': return AddToken(TokenType.Operator, Match('=') ? "==" : "=");
        case '<': return AddToken(TokenType.Operator, Match('=') ? "<=" : "<");
        case '>': return AddToken(TokenType.Operator, Match('=') ? ">=" : ">");
        // Single Comment
        case '#': while (Current() != '\n' && !IsAtEnd) Next(); break;
        /* Whitespaces */
        case ' ':
        case '\r':
        case '\t': break;
        /* String Literals */
        case '"': ScanString(); break;
        case '\'': ScanCharacter(); break;
        default:
          if (Char.IsDigit(ch)) {
            return ScanNumber();
          } else if (Char.IsLetter(ch)) {
            return ScanIdentifier();
          }
          if (IsAtEnd) return EOF;
          Metal.Error(source.Line, "Unexpected character");
          break;
      }
      return null;
    }

    private Token ScanString() {
      while (Current() != '"' && !IsAtEnd) { Next(); }

      // Unterminated string.
      if (IsAtEnd) { Metal.Error(source.Line, "Unterminated string."); return null; }
      // The closing '"'.
      Next();

      string lexeme = source.File.Substring(Start, End);
      string literal = source.File.Substring(Start + 1, End - 2);
      return AddToken(TokenType.StringLiteral, lexeme, literal);
    }

    private Token ScanCharacter() {
      while (Current() != '\'' && !IsAtEnd) { Next(); }

      // Unterminated string.
      if (IsAtEnd) { Metal.Error(source.Line, "Unterminated character."); return null; }
      // The closing '''.
      Next();
      string lexeme = source.File.Substring(Start, End);
      string literal = source.File.Substring(Start + 1, End - 2);
      if (literal.Contains("\\")) {
        return AddToken(TokenType.CharacterLiteral, lexeme, Regex.Escape(literal));
      }
      else if(literal.Length > 1) { Metal.Error(source.Line, "Unrecognized character literal."); }
      return AddToken(TokenType.CharacterLiteral, lexeme, literal);
    }

    private bool Match(char expected) {
      if (IsAtEnd) return false;
      if (Current() != expected) return false;
      Next();
      return true;
    }

    private char Current() {
      if (source.Position >= source.File.Length) return File.EOF;
      return source.File[source.Position];
    }

    private char Next() {
      source.Position++;
      char ch = source.File[source.Position - 1];
      if (ch == '\n') {
        source.Line++;
        source.Column = 1;
      } else source.Column++;
      return ch;
    }

    private char Peek() {
      return Peek(1);
    }

    private char Peek(int to) {
      if (source.Position + to >= source.File.Length) return File.EOF;
      return source.File[source.Position + to];
    }

    private Token ScanNumber() {
      while (Char.IsDigit(Current())) Next();

      // Look for fractional part.
      if (Current() == '.' && Char.IsDigit(Peek())) {
        // Consume '.'
        Next();
        while (Char.IsDigit(Current())) Next();
      }
      string lexeme = source.File.Substring(Start, End);
      TokenType type = lexeme.Contains(".") ? TokenType.FloatingPointLiteral : TokenType.IntegerLiteral;
      object literal = null;
      if (lexeme.Contains(".")) {
        Double.TryParse(lexeme, out var number);
        literal = number;
      } else {
        Int32.TryParse(lexeme, out var number);
        literal = number; 
      }
      return AddToken(type, literal);
    }

    private Token ScanIdentifier() {
      TokenType type = TokenType.Identifier;
      while (IsIdentifer(Current()) && Current() != ' ') { Next(); }

      bool IsIdentifer(char ch) {
        return Char.IsLetterOrDigit(ch) || ch == '_' || ch == '$';
      }
      string lexeme = source.File.Substring(Start, End);
      if (Token.IsReservedWord(lexeme)) type = TokenType.Reserved;
      if (Token.IsOperatorString(lexeme)) type = TokenType.Operator;
      if (Token.IsLiteralString(lexeme)) {
        switch (lexeme) {
          case "null": type = TokenType.NullLiteral; break;
          case "true": case "false": 
            type = TokenType.BooleanLiteral; break;
          default: break;
        }
      }
      return AddToken(type, lexeme);
    }

    private Token AddToken(TokenType type) {
      return AddToken(type, null);
    }

    private Token AddToken(TokenType type, char lexeme) {
      return AddToken(type, lexeme.ToString());
    }

    private Token AddToken(TokenType type, string lexeme) {
      return AddToken(type, lexeme, null);
    }

    private Token AddToken(TokenType type, object literal) {
      return AddToken(type, literal != null ? literal.ToString() : "", literal);
    }

    private Token AddToken(TokenType type, string lexeme, Object literal) {
      string text = lexeme.Length == 1 ? lexeme : source.File.Substring(Start, End);
      Token token = new Token(type, text, literal, source.Line, source.Position);
      tokens.Add(token);
      return token;
    }

    private int Start { get; set; }
    private int End { get { return source.Position - Start; } }
    public Token EOF { get { return new Token(TokenType.EOF, "", null, source.Line, source.Column); } }
    public bool IsAtEnd { get { return source.Position >= source.File.Length; } }
    public List<Token> Tokens { get { return tokens; } }
  }
}


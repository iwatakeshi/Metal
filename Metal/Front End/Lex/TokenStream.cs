using System;
using System.Collections.Generic;
using log4net;
using Metal.IO;

namespace Metal.FrontEnd.Lex {
  public class TokenStream {
    private List<Token> tokens;
    public TokenStream () {
      tokens = new List<Token>();
      Log = log4net.LogManager.GetLogger (typeof(Scanner));
    }
    ILog Log { get; set; }
    public bool EOS { get { return tokens.Count <= Position; } }
    public int Position { get; set; }
    
    public void AddToken(Token token){
      tokens.Add(token);
    }
    public bool Match(TokenType type) {
      return PeekToken () != null && 
        PeekToken().Type == type;
    }
    public bool Match(TokenType type, TokenType type2) {
      return PeekToken () != null && 
        PeekToken().Type == type && 
        PeekToken (1) != null &&
				PeekToken (1).Type == type2;
    }
    
    public bool Match (TokenType type, string value)
		{
			return PeekToken () != null &&
				PeekToken ().Type == type &&
				PeekToken ().Value == value;
		}
    public bool Accept (TokenType type)
		{
			if (PeekToken () != null && PeekToken ().Type == type) {
				ScanToken ();
				return true;
			}
			return false;
		}

		public bool Accept (TokenType type, ref Token token)
		{
			if (PeekToken () != null && PeekToken ().Type == type) {
				token = ScanToken ();
				return true;
			}
			return false;
		}

		public bool Accept (TokenType type, string val)
		{
			if (PeekToken () != null && PeekToken ().Type == type && PeekToken ().Value == val) {
				ScanToken ();
				return true;
			}
			return false;
		}
    
    public Token Expect (TokenType type, string value, Source source)
		{
			var token = PeekToken ();
			if (Accept(type, value)) {
				return token;
			}
			var failed = ScanToken ();
			if (failed != null) {
        var error = "Metal [Error]: Invalid token \"{0}\" at line: {1}, position: {2}. Expected \"{3}\"";
				Log.Error (string.Format(error, value, source.Line, source.Position, Token.GetName(type)));
			} else {
				var error = "Metal [Error]: End of File at line: {0}, position: {1}. Expected \"{2}\"";
				Log.Error (string.Format(error, source.Line, source.Position, Token.GetName(type)));
				throw new Exception ("");
			}
			return new Token (type, "", source);
		}
    
    private Token PeekToken ()
		{
			return PeekToken (0);
		}

		private Token PeekToken (int n)
		{
			if (Position + n < tokens.Count) {
				return tokens [Position + n];
			}
			return null;
		}

		public Token ScanToken ()
		{
			if (Position >= tokens.Count) {
				return null;
			}
			return tokens [Position++];
		}
	}
}


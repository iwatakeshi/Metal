using System;
using Metal.IO;
using Flask.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Metal.FrontEnd.Lex {
	/// <summary>
	/// Transforms a stream of characters into a stream of words.
	/// </summary>
	public class Scanner {

		/* Private variables */
		StringBuilder buffer;
		Source source;
		Comment comments;

		public Scanner () {
			buffer = new StringBuilder ();
			comments = new Comment ();
		}

		public Scanner (string fileName) {
			buffer = new StringBuilder ();
			source = new Source (fileName);
		}

		public Scanner (string path, string fileName) {
			buffer = new StringBuilder ();
			source = new Source (path, fileName);
		}

		/// <summary>
		/// Gets the next the token.
		/// </summary>
		/// <returns>The token.</returns>
		public Token NextToken () {
			if (IsEOF)
				return new Token (Token.EOF, source);
			// Clear the string buffer
			buffer.Clear ();
			// Skip whitespace
			Ignore ();

			// Check state as we gather the pieces to create the tokens
			switch (CurrentChar ()) {
			/* Punctuation */
			case '.':
				NextChar ();
				return new Token (Token.Dot, source);
			case ',':
				NextChar ();
				return new Token (Token.Comma, source);
			case ':':
				NextChar ();
				return new Token (Token.Colon, source);
			case ';':
				NextChar ();
				return new Token (Token.SemiColon, source);
			case '(':
				NextChar ();
				return new Token (Token.LeftParenthesis, source);
			case ')':
				NextChar ();
				return new Token (Token.RightParenthesis, source);
			case '{':
				NextChar ();
				return new Token (Token.LeftBraces, source);
			case '}':
				NextChar ();
				return new Token (Token.RightBraces, source);
			case '[':
				NextChar ();
				return new Token (Token.LeftBracket, source);
			case ']':
				NextChar ();
				return new Token (Token.RightBracket, source);
			case '\n':
				source.Line++;
				NextChar ();
				return NextToken ();
			
			/* Operators */
			
			case '=':
				// Comsume '='
				NextChar ();
				// ==
				if (CurrentChar () == '=') {
					// Consume '='
					NextChar ();
					// ===
					if (CurrentChar () == '=') {
						NextChar ();
						return new Token (Token.StrictEqual, source);
						// ==
					} else {
						// =
						return new Token (Token.Equal, source);
					}
				} else {
					// =
					return new Token (Token.Assign, source);
				}
			case '-':
				// Comsume '-'
				NextChar ();
				// --
				if (CurrentChar () == '-') {
					// Consume the next '-'
					NextChar ();
					return new Token (Token.Decrement, source);
					// -=
				} else if (CurrentChar () == '=') {
					// consume
					NextChar ();
					return new Token (Token.DifferenceAssignment, source);
				} else {
					// -
					return new Token (Token.Minus, source);
				}
			case '+':
				// Consume '+'
				NextChar ();
				// ++
				if (CurrentChar () == '+') {
					NextChar ();
					return new Token (Token.Increment, source);
					// +=
				} else if (CurrentChar () == '=') {
					NextChar ();
					return new Token (Token.SumAssignment, source);
				} else {
					// +
					return new Token (Token.Plus, source);
				}
					
			case '*':
				// Consume '*'
				NextChar ();
				// *=
				if (CurrentChar () == '=') {
					NextChar ();
					return new Token (Token.ProductAssignment, source);
				} else {
					return new Token (Token.Times, source);
				}
					
			case '/':
				// Consume '/'
				NextChar ();
				// /=
				if (CurrentChar () == '=') {
					NextChar ();
					return new Token (Token.RemainderAssignment, source);
				} else {
					// /
					return new Token (Token.Divide, source);
				}
			case '>':
				// Consume '>'
				NextChar ();
				// >=
				if (CurrentChar () == '=') {
					NextChar ();
					return new Token (Token.GreaterThanEqualTo, source);
				} else {
					// >
					return new Token (Token.GreaterThan, source);
				}
			case '<':
				// Consume '<'
				NextChar ();
				// <=
				if (CurrentChar () == '=') {
					NextChar ();
					return new Token (Token.LessThanEqualTo, source);
				} else {
					// <
					return new Token (Token.LessThan, source);
				}
			case '&':
				// Comsume '&'
				NextChar ();
				// && 
				if (CurrentChar () == '&') {
					NextChar ();
					return new Token (Token.And, source);
				} else {
					return new Token (TokenType.Invalid, CurrentChar ().ToString (), source);
				}
			case '|':
				// Consume '|'
				NextChar ();
				// ||
				if (CurrentChar () == '|') {
					NextChar ();
					return new Token (Token.Or, source);
				} else {
					return new Token (TokenType.Invalid, CurrentChar ().ToString (), source);
				}
			case '!':
				// Consume '!'
				NextChar ();
				if (CurrentChar () == '=') {
					NextChar ();
					return new Token (Token.NotEqual, source);
				} else {
					// !
					return new Token (Token.LogicalNot, source);
				}
			case '%':
				NextChar ();
				// %
				return new Token (Token.Modulus, source);
			case '\'':
				buffer.Append ('\'');
				NextChar ();
				if (CurrentChar () == '\\') {
					NextChar ();
					buffer.Append (Escape ());
				} else {
					buffer.Append (CurrentChar ());
					NextChar ();
				}
				if (CurrentChar () == '\'') {
					buffer.Append ('\'');
					NextChar ();
					return new Token (TokenType.CharacterLiteral, buffer.ToString (), source);
				} else {
					// TODO: Report error
					while (CurrentChar () != '\'' && CurrentChar () != ';' && CurrentChar () != '\n') {
						NextChar ();
					}
					return new Token (TokenType.CharacterLiteral, buffer.ToString (), source);
				}
			case '"':
				buffer.Append ("\"");
				NextChar ();
				while (CurrentChar () != '"' && CurrentChar () != '\n') {
					if (CurrentChar () == '\\') {
						NextChar ();
						buffer.Append (Escape ());
					} else {
						buffer.Append (CurrentChar ());
						NextChar ();
					}
				}
				if (CurrentChar () == '\n') {
					// TODO: Report error
				} else {
					// Scan the closing "
					NextChar ();
					buffer.Append ("\"");
				}
				return new Token (TokenType.StringLiteral, buffer.ToString (), source);
			default:
				// If the character is a number
				if (Char.IsDigit (CurrentChar ())) {
					// Consume the number
					while (Char.IsDigit (CurrentChar ())) {
						buffer.Append (CurrentChar ());
						NextChar ();
					}
					// If the number is a decimal
					if (CurrentChar () == '.' && Char.IsDigit (PeekChar ())) {
						// Consume the decimal
						buffer.Append (CurrentChar ());
						NextChar ();
						// Consume the number
						if (Char.IsNumber (CurrentChar ())) {
							while (Char.IsDigit (CurrentChar ())) {
								buffer.Append (CurrentChar ());
								NextChar ();
							}
						}
					} else if ((CurrentChar () == 'E' || CurrentChar () == 'e')) {
						char current = CurrentChar ();
						char next = NextChar ();
						if ((next == '+' || (next == '-')) && Char.IsDigit (PeekChar ())) {
							// Consume 'E' or 'e'
							buffer.Append (current);
							// Consume '+' or '-'
							buffer.Append (next);
							NextChar ();
							// Consume the number
							while (Char.IsDigit (CurrentChar ())) {
								buffer.Append (CurrentChar ());
								NextChar ();
							}
						}
					}
					return new Token (TokenType.Number, buffer.ToString (), source);
				}
				// If the character is a identifier
				if (Char.IsLetter (CurrentChar ()) || CurrentChar () == '$') {
					while ((Char.IsLetterOrDigit (CurrentChar ()) || CurrentChar () == '$')) {
						buffer.Append (CurrentChar ());
						NextChar ();
						if (CurrentChar () == NewLine)
							break;
					}
					if (Token.IsKeyword (buffer.ToString ())) {
						return new Token (TokenType.Keyword, buffer.ToString (), source);
					} else
						return new Token (TokenType.Id, buffer.ToString (), source);
				}

				// TODO: Report error
				//return new Token(Token.Invalid, source);
				return NextToken ();
			}
		}

		/// <summary>
		/// Peeks the next character in the source text.
		/// </summary>
		/// <returns>The char.</returns>
		char PeekChar () {
			return source.File [source.Position + 1];
		}

		/// <summary>
		/// Gets the current character in the source text.
		/// </summary>
		/// <returns>The char.</returns>
		char CurrentChar () {
			return source.File [source.Position];
		}

		/// <summary>
		/// Gets the next character in the source text.
		/// </summary>
		/// <returns>The char.</returns>
		char NextChar () {
			return source.File [source.Position++];
		}

		/// <summary>
		/// Gets a value indicating whether this instance is EOF.
		/// </summary>
		/// <value><c>true</c> if this instance is EOF; otherwise, <c>false</c>.</value>
		public bool IsEOF { get { return CurrentChar () == source.EOF; } }

		public Comment Comments { get { return comments; } }

		public Source Source { get { return source; } }

		bool IsWhiteSpace { get { return Char.IsWhiteSpace (CurrentChar ()); } }

		bool IsTabSpace { get { return CurrentChar () == '\t'; } }

		char NewLine { get { return Environment.NewLine.ToCharArray () [0]; } }

		/// <summary>
		/// Skips all white spaces
		/// </summary>
		void Ignore(){
			for (;; NextChar ()) {
				if(CurrentChar () == NewLine){
					source.Line++;
				}
				else if (IsWhiteSpace || IsTabSpace)
					continue;
				else break;
			}
			if(CurrentChar () == '#'){
				NextChar ();
				while (true) {
					if (CurrentChar () == NewLine) {
						source.Line++;
						NextChar ();
						break;
					}
					NextChar ();
				}
			}
			if(CurrentChar () == '/' && PeekChar () == '*'){
				source.Position += 2;
				while (true) {
					if(CurrentChar () != '*' && PeekChar () != '/'){
						if(CurrentChar () == NewLine) 
							source.Line++;
						NextChar ();
					} else{
						break;
					}
				}
				if(CurrentChar () == '*' && PeekChar () == '/'){
					source.Position += 2;
				}else {
					// TODO Report error
				}
			}
		}


		/// <summary>
		/// Escape the character.
		/// </summary>
		string Escape () {
			switch (CurrentChar ()) {
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
				// TODO: Report error
				return "";
			}
		}
	}
}


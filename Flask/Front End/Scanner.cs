using System;
using Flask.FrontEnd.Lex;
using System.Text;

namespace Flask.FrontEnd {
	public class Scanner {
		StringBuilder buffer;
		string file;
		int position;

		public Scanner () {
			buffer = new StringBuilder ();
			file = "";
			position = 0;
		}

		public Scanner (string file){
			buffer = new StringBuilder ();
			this.file = file;
			position = 0;
		}

		public Token NextToken() {
			if(CurrentChar () == NewLine){
				buffer.Append (NewLine);
				NextChar ();
			}
			switch (CurrentChar ()) {
			case '#':
				// Consume '#'
				NextChar ();
				while (CurrentChar () != NewLine) {
					NextChar ();
				}
				NextChar ();
				break;
			case '/':
				// Consume '/'
				NextChar ();
				if(NextChar () == '*'){
					while (CurrentChar () != '/') {
						buffer.Append (NewLine);
						NextChar ();
					}
					NextChar ();
				}
				break;
			default:
				SkipWhiteSpace ();
				buffer.Append (CurrentChar ());
				NextChar ();
				break;
			}
			return null;
		}

		char CurrentChar(){
			return file [position];
		}

		char NextChar(){
			return file [position++];
		}

		char PeekChar(){
			return file [position + 1];
		}

		public string File { get { return buffer.ToString ();} }
		public bool IsEOF { get { return position == file.Length; }}
		bool IsWhiteSpace { get { return Char.IsWhiteSpace (CurrentChar ()); } }
		bool IsTabSpace { get { return CurrentChar () == '\t'; } }
		char NewLine { get {return Environment.NewLine.ToCharArray () [0];} }
		/// <summary>
		/// Skips all white spaces
		/// </summary>
		void SkipWhiteSpace (){
			// Skip white spaces
			for(;; NextChar ()){
				if (IsWhiteSpace){
					continue;
				}
				break;
			}
		}
		void SkipNewLine (){
			for (;; NextChar ()) {
				if(CurrentChar () == NewLine){
					buffer.Append (NewLine);
					continue;
				}
				break;
			}
		}
	}
}


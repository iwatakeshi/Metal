using System;
using Flask.FrontEnd;
using Flask.FrontEnd.Scan;

namespace Flask.IO {
	public class Comment {
		Scanner scanner;
		public Comment () {
			scanner = new Scanner ();
		}

		public Comment (string file) {
			/*
			* Psuedo code:
			* 1. Set text
			* 2. Strip and compile all comments from text
			* 3. Return the striped text and the result
			*/

			scanner = new Scanner (file);
			while(!scanner.IsEOF){
				scanner.NextToken ();
			}
		}

		public string File { get { return scanner.File; }}
	}
}


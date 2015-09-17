using System;
using Newtonsoft.Json.Linq;

namespace Metal.IO {

	public class Source {
		string seperator = System.IO.Path.DirectorySeparatorChar.ToString ();
		string directory = System.IO.Directory.GetCurrentDirectory ();
		public Source(){
		}
		public Source (string fileName) {
			FileName = fileName;
			Line = 1;
			Position = 0;
			Cursor = 0;
			File = Metal.IO.File.ReadAllText (directory + fileName) + EOF;
			Length = File.Length;
		}

		public Source(string path, string fileName){
			Path = path;
			FileName = fileName;
			Line = 1;
			Position = 0;
			Cursor = 0;
			File = Metal.IO.File.ReadAllText (path + seperator + fileName) + EOF;
			Length = File.Length;
		}

		public Source (string fileName, int line, int position, int cursor) {
			FileName = fileName;
			Line = line;
			Position = position;
			Cursor = cursor;
			File = Metal.IO.File.ReadAllText (directory + seperator + fileName) + EOF;
			Length = File.Length;
		}

		public Source(string path, string fileName, int line, int position, int cursor){
			Path = path;
			FileName = fileName;
			Line = line;
			Position = position;
			Cursor = cursor;
			File = Metal.IO.File.ReadAllText (path + fileName) + EOF;
			Length = File.Length;
		}
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Metal.IO.Source"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Metal.IO.Source"/>.</returns>
		public override string ToString () {
			return string.Format ("FileName: '{0}', Path: '{1}', Line: {2}, Position: {3}", FileName, Path, Line, Position);
		}

		/// <summary>
		/// Returns a JSON object that represents the current <see cref="Metal.IO.Source"/>.
		/// </summary>
		/// <returns>The JSON object.</returns>
		public object ToJson () {
			return JObject.Parse (string.Format ("{{ \"source\": {{ \"name\": \"{0}\", \"path\": \"{1}\", \"line\": {2}, \"position\": {3} }} }}", FileName, Path, Line, Position));
		}

		public string FileName { get; private set; }

		public string File { get; private set; }

		public string Path { get; private set; }

		public int Line { get; set; }

		public int Position { get; set; }

		public int Length { get; private set;}

		public char EOF { get { return '\0'; } }

		public int Cursor { get; set; }
	}
}


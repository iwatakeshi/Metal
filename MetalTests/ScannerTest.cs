using NUnit.Framework;
using System;
using Metal.FrontEnd.Lex;
using System.IO;

namespace MetalTests {
	[TestFixture ()]
	public class ScannerTest {
		string seperator = System.IO.Path.DirectorySeparatorChar.ToString ();
		string directory = Directory.GetParent (Directory.GetCurrentDirectory ()).Parent.FullName;

		[Test ()]
		public void TestTokens () {
			Scanner scanner = new Scanner(directory + seperator + "Sources", "scanner.ms");
			while (!scanner.IsEOF) {
				scanner.NextToken ();
			}
		}
	}
}


using NUnit.Framework;
using System;
using Metal.FrontEnd.Lex;
using System.IO;
using System.Collections.Generic;

namespace MetalTests {
	[TestFixture ()]
	public class ScannerTest {
		string seperator = System.IO.Path.DirectorySeparatorChar.ToString ();
		string directory = Directory.GetParent (Directory.GetCurrentDirectory ()).Parent.FullName;
		List<TokenType> tokenTypes = new List<TokenType> () {
			TokenType.Number, TokenType.Id, TokenType.Keyword, TokenType.CharacterLiteral, 
			TokenType.StringLiteral,
			// Test all keywords
			TokenType.Keyword, TokenType.Keyword, TokenType.Keyword, TokenType.Keyword,
			TokenType.Keyword, TokenType.Keyword, TokenType.Keyword, TokenType.Keyword,
			TokenType.Keyword, TokenType.Keyword, TokenType.Keyword, TokenType.Keyword,
			TokenType.Keyword, TokenType.Keyword, TokenType.Keyword, TokenType.Keyword,
			TokenType.Keyword, TokenType.Keyword, TokenType.Keyword, TokenType.Keyword,
			TokenType.Keyword, TokenType.Keyword, TokenType.Keyword, TokenType.Keyword,
			TokenType.Keyword, TokenType.Keyword,
		};

		[Test ()]
		public void TestTokens () {
			Scanner scanner = new Scanner(directory + seperator + "Sources", "scanner.ms");
			Assert.IsNotEmpty (scanner.Source.File);

			var count = 0;
			while (!scanner.IsEOF) {
				Assert.AreEqual (scanner.NextToken ().Type, tokenTypes[count++]);
			}
		}
	}
}


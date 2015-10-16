using NUnit.Framework;
using System;
using Metal.FrontEnd.Scan;
using System.IO;
using System.Collections.Generic;

namespace MetalTests {
  [TestFixture ()]
  public class ScannerTest {
    static string seperator = System.IO.Path.DirectorySeparatorChar.ToString ();
    static string directory = Directory.GetParent (Directory.GetCurrentDirectory ()).Parent.FullName;
    static Scanner scanner = new Scanner (directory + seperator + "Sources", "scanner.ms");
    List<Tuple<string, TokenType>> expected = new List<Tuple<string, TokenType>> () {
      /* Literals */
      Tuple.Create ("Hello", TokenType.StringLiteral),
      Tuple.Create ("Hello world!", TokenType.StringLiteral),
      Tuple.Create ("100", TokenType.IntegerLiteral),
      Tuple.Create ("3.14", TokenType.FloatingPointLiteral),
      Tuple.Create ("true", TokenType.BooleanLiteral),
      Tuple.Create ("false", TokenType.BooleanLiteral),
      /* Keywords */
      Tuple.Create ("class", TokenType.Keyword),
      Tuple.Create ("enum", TokenType.Keyword),
      Tuple.Create ("extends", TokenType.Keyword),
      Tuple.Create ("fn", TokenType.Keyword),
      Tuple.Create ("import", TokenType.Keyword),
      Tuple.Create ("export", TokenType.Keyword),
      Tuple.Create ("init", TokenType.Keyword),
      Tuple.Create ("let", TokenType.Keyword),
      Tuple.Create ("var", TokenType.Keyword),
      Tuple.Create ("private", TokenType.Keyword),
      Tuple.Create ("public", TokenType.Keyword),
      Tuple.Create ("break", TokenType.Keyword),
      Tuple.Create ("continue", TokenType.Keyword),
      Tuple.Create ("default", TokenType.Keyword),
      Tuple.Create ("do", TokenType.Keyword),
      Tuple.Create ("else", TokenType.Keyword),
      Tuple.Create ("for", TokenType.Keyword),
      Tuple.Create ("if", TokenType.Keyword),
      Tuple.Create ("in", TokenType.Keyword),
      Tuple.Create ("return", TokenType.Keyword),
      Tuple.Create ("switch", TokenType.Keyword),
      Tuple.Create ("while", TokenType.Keyword),
      Tuple.Create ("yield", TokenType.Keyword),
      Tuple.Create ("catch", TokenType.Keyword),
      Tuple.Create ("null", TokenType.Keyword),
      Tuple.Create ("super", TokenType.Keyword),
      Tuple.Create ("this", TokenType.Keyword),
      Tuple.Create ("throw", TokenType.Keyword),
      Tuple.Create ("try", TokenType.Keyword),
      Tuple.Create ("get", TokenType.Keyword),
      Tuple.Create ("set", TokenType.Keyword),
      Tuple.Create ("$", TokenType.Identifier),
      Tuple.Create ("_", TokenType.Identifier),
      /* Identifiers */
      Tuple.Create ("num", TokenType.Identifier),
      Tuple.Create ("num2", TokenType.Identifier),
      Tuple.Create ("$var", TokenType.Identifier),
      Tuple.Create ("_var", TokenType.Identifier),
      /* Operators */
      Tuple.Create ("=", TokenType.Operator),
      Tuple.Create ("+", TokenType.Operator),
      Tuple.Create ("-", TokenType.Operator),
      Tuple.Create ("*", TokenType.Operator),
      Tuple.Create ("/", TokenType.Operator),
      Tuple.Create ("%", TokenType.Operator),
      Tuple.Create ("++", TokenType.Operator),
      Tuple.Create ("--", TokenType.Operator),
      Tuple.Create ("+=", TokenType.Operator),
      Tuple.Create ("-=", TokenType.Operator),
      Tuple.Create ("*=", TokenType.Operator),
      Tuple.Create ("/=", TokenType.Operator),
      Tuple.Create ("%=", TokenType.Operator),
      Tuple.Create ("&&=", TokenType.Operator),
      Tuple.Create ("||=", TokenType.Operator),
      Tuple.Create ("&=", TokenType.Operator),
      Tuple.Create ("|=", TokenType.Operator),
      Tuple.Create ("==", TokenType.Operator),
      Tuple.Create ("!=", TokenType.Operator),
      Tuple.Create (">", TokenType.Operator),
      Tuple.Create ("<", TokenType.Operator),
      Tuple.Create (">=", TokenType.Operator),
      Tuple.Create ("<=", TokenType.Operator),
      Tuple.Create ("===", TokenType.Operator),
      Tuple.Create ("!==", TokenType.Operator),
      Tuple.Create ("?", TokenType.Operator),
      Tuple.Create ("&&", TokenType.Operator),
      Tuple.Create ("||", TokenType.Operator),
      Tuple.Create ("!", TokenType.Operator),
      Tuple.Create ("=>", TokenType.Operator),
      Tuple.Create (".", TokenType.Operator),
      Tuple.Create ("~", TokenType.Operator),
      Tuple.Create ("&", TokenType.Operator),
      Tuple.Create ("|", TokenType.Operator),
      Tuple.Create ("^", TokenType.Operator),
      Tuple.Create ("<<", TokenType.Operator),
      Tuple.Create (">>", TokenType.Operator),
      Tuple.Create ("...", TokenType.Operator),
      Tuple.Create ("as", TokenType.Operator),
      Tuple.Create ("is", TokenType.Operator),
    };

    [Test ()]
    public void TestTokens () {
      
      Assert.IsNotEmpty (scanner.Source.File);

      var stream = scanner.Scan ();
      int count = 0;
      foreach (var token in stream.Tokens){
        Assert.AreEqual (expected[count].Item1, token.Value);
        Assert.AreEqual (expected[count].Item2, token.Type);
        count++;
      }
    }
  }
}


namespace MetalTests {
  //[TestFixture ()]
  //public class ScannerTest {
  //  static string seperator = System.IO.Path.DirectorySeparatorChar.ToString ();
  //  static string directory = Directory.GetParent (Directory.GetCurrentDirectory ()).Parent.FullName;
  //  static Scanner scanner = new Scanner (directory + seperator + "Sources", "scanner.ms");
  //  List<Tuple<string, TokenType>> expected = new List<Tuple<string, TokenType>> () {
  //    /* Literals */
  //    Tuple.Create ("Hello", TokenType.StringLiteral),
  //    Tuple.Create ("Hello world!", TokenType.StringLiteral),
  //    Tuple.Create ("100", TokenType.IntegerLiteral),
  //    Tuple.Create ("3.14", TokenType.FloatingPointLiteral),
  //    Tuple.Create ("true", TokenType.BooleanLiteral),
  //    Tuple.Create ("false", TokenType.BooleanLiteral),
  //    /* Keywords */
  //    Tuple.Create ("class", TokenType.Reserved),
  //    Tuple.Create ("enum", TokenType.Reserved),
  //    Tuple.Create ("extends", TokenType.Reserved),
  //    Tuple.Create ("fn", TokenType.Reserved),
  //    Tuple.Create ("import", TokenType.Reserved),
  //    Tuple.Create ("export", TokenType.Reserved),
  //    Tuple.Create ("init", TokenType.Reserved),
  //    Tuple.Create ("let", TokenType.Reserved),
  //    Tuple.Create ("var", TokenType.Reserved),
  //    Tuple.Create ("private", TokenType.Reserved),
  //    Tuple.Create ("public", TokenType.Reserved),
  //    Tuple.Create ("break", TokenType.Reserved),
  //    Tuple.Create ("continue", TokenType.Reserved),
  //    Tuple.Create ("default", TokenType.Reserved),
  //    Tuple.Create ("do", TokenType.Reserved),
  //    Tuple.Create ("else", TokenType.Reserved),
  //    Tuple.Create ("for", TokenType.Reserved),
  //    Tuple.Create ("if", TokenType.Reserved),
  //    Tuple.Create ("in", TokenType.Reserved),
  //    Tuple.Create ("return", TokenType.Reserved),
  //    Tuple.Create ("switch", TokenType.Reserved),
  //    Tuple.Create ("while", TokenType.Reserved),
  //    Tuple.Create ("yield", TokenType.Reserved),
  //    Tuple.Create ("catch", TokenType.Reserved),
  //    Tuple.Create ("null", TokenType.Reserved),
  //    Tuple.Create ("super", TokenType.Reserved),
  //    Tuple.Create ("this", TokenType.Reserved),
  //    Tuple.Create ("throw", TokenType.Reserved),
  //    Tuple.Create ("try", TokenType.Reserved),
  //    Tuple.Create ("get", TokenType.Reserved),
  //    Tuple.Create ("set", TokenType.Reserved),
  //    Tuple.Create ("$", TokenType.Identifier),
  //    Tuple.Create ("_", TokenType.Identifier),
  //    /* Identifiers */
  //    Tuple.Create ("num", TokenType.Identifier),
  //    Tuple.Create ("num2", TokenType.Identifier),
  //    Tuple.Create ("$var", TokenType.Identifier),
  //    Tuple.Create ("_var", TokenType.Identifier),
  //    /* Operators */
  //    Tuple.Create ("=", TokenType.Operator),
  //    Tuple.Create ("+", TokenType.Operator),
  //    Tuple.Create ("-", TokenType.Operator),
  //    Tuple.Create ("*", TokenType.Operator),
  //    Tuple.Create ("/", TokenType.Operator),
  //    Tuple.Create ("%", TokenType.Operator),
  //    Tuple.Create ("++", TokenType.Operator),
  //    Tuple.Create ("--", TokenType.Operator),
  //    Tuple.Create ("+=", TokenType.Operator),
  //    Tuple.Create ("-=", TokenType.Operator),
  //    Tuple.Create ("*=", TokenType.Operator),
  //    Tuple.Create ("/=", TokenType.Operator),
  //    Tuple.Create ("%=", TokenType.Operator),
  //    Tuple.Create ("&&=", TokenType.Operator),
  //    Tuple.Create ("||=", TokenType.Operator),
  //    Tuple.Create ("&=", TokenType.Operator),
  //    Tuple.Create ("|=", TokenType.Operator),
  //    Tuple.Create ("==", TokenType.Operator),
  //    Tuple.Create ("!=", TokenType.Operator),
  //    Tuple.Create (">", TokenType.Operator),
  //    Tuple.Create ("<", TokenType.Operator),
  //    Tuple.Create (">=", TokenType.Operator),
  //    Tuple.Create ("<=", TokenType.Operator),
  //    Tuple.Create ("===", TokenType.Operator),
  //    Tuple.Create ("!==", TokenType.Operator),
  //    Tuple.Create ("?", TokenType.Operator),
  //    Tuple.Create ("&&", TokenType.Operator),
  //    Tuple.Create ("||", TokenType.Operator),
  //    Tuple.Create ("!", TokenType.Operator),
  //    Tuple.Create ("=>", TokenType.Operator),
  //    Tuple.Create (".", TokenType.Operator),
  //    Tuple.Create ("~", TokenType.Operator),
  //    Tuple.Create ("&", TokenType.Operator),
  //    Tuple.Create ("|", TokenType.Operator),
  //    Tuple.Create ("^", TokenType.Operator),
  //    Tuple.Create ("<<", TokenType.Operator),
  //    Tuple.Create (">>", TokenType.Operator),
  //    Tuple.Create ("...", TokenType.Operator),
  //    Tuple.Create ("as", TokenType.Operator),
  //    Tuple.Create ("is", TokenType.Operator),
  //  };

  //  [Test ()]
  //  public void TestTokens () {

  //    Assert.IsNotEmpty (scanner.Source.File);

  //    var stream = scanner.Scan ();
  //    int count = 0;
  //    foreach (var token in stream.ToBuffer ()){
  //      Assert.AreEqual (expected[count].Item1, token.Value);
  //      Assert.AreEqual (expected[count].Item2, token.Type);
  //      count++;
  //    }
  //  }
  //}
}


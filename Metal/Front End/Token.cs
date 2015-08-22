using System;
using System.Collections.Generic;
using Metal.IO;

namespace Metal.FrontEnd.Lex {
	public enum TokenType {
		/*
			Keywords
		*/
		// as
		As,
		// bool
		Bool,
		// char
		Char,
		// class
		Class,
		// else
		Else,
		// extends
		Extends,
		// export
		Export,
		// false
		False,
		// fn
		Fn,
		// hidden
		Hidden,
		// if
		If,
		// is
		Is,
		// import
		Import,
		// let
		Let,
		// Module
		Module,
		// new
		New,
		// null
		Null,
		// public
		Public,
		// return
		Return,
		// shared
		Shared,
		// static
		Static,
		// string
		String,
		// super
		Super,
		// this
		This,
		// true
		True,
		// var
		Var,
		// while
		While,

		/*
			Punctuation
		*/
		// .
		Dot,
		// ,
		Comma,
		// :
		Colon,
		// ;
		SemiColon,
		// (
		LeftParenthesis,
		// )
		RightParenthesis,
		// {
		LeftBraces,
		// }
		RightBraces,
		// [
		LeftBracket,
		// ]
		RightBracket,

		/*
		 	Arithmetic	
		 */
		// +
		Plus,
		// -
		Minus,
		// *
		Times,
		// /
		Divide,
		/*
			Operators
		*/
		// =
		Assign,
		// --
		Decrement,
		// ++
		Increment,
		// +=
		SumAssignment,
		// -=
		DifferenceAssignment,
		// *=
		ProductAssignment,
		// /=
		RemainderAssignment,
		// ==
		Equal,
		// !=
		NotEqual,
		// ===
		StrictEqual,
		// >
		GreaterThan,
		// >=
		GreaterThanEqualTo,
		// &&
		And,
		// <
		LessThan,
		// <=
		LessThanEqualTo,
		// !
		LogicalNot,
		// %
		Modulus,
		// ||
		Or,
		/*
			Data type
		*/
		CharacterLiteral,
		// ^["|']([\s\S]*)["\']$
		StringLiteral,
		// [-+]?([0-9]*\.[0-9]+|[0-9]+)
		Number,
		// ^(?![0-9])[0-9A-Za-z]+
		Id,
		// Anything else not defined by grammar.
		Keyword,
		Invalid
	}

	public class Token {
		
		static Dictionary<string, Tuple<TokenType, string>> keywords;

		/* Keywords */
		public static readonly Tuple<TokenType, string> As = Tuple.Create (TokenType.As, "as");
		public static readonly Tuple<TokenType, string> Bool = Tuple.Create (TokenType.Bool, "bool");
		public static readonly Tuple<TokenType, string> Char = Tuple.Create (TokenType.Char, "char");
		public static readonly Tuple<TokenType, string> Class = Tuple.Create (TokenType.Class, "class");
		public static readonly Tuple<TokenType, string> Else = Tuple.Create (TokenType.Else, "else");
		public static readonly Tuple<TokenType, string> Extends = Tuple.Create (TokenType.Extends, "extends");
		public static readonly Tuple<TokenType, string> Export = Tuple.Create (TokenType.Export, "export");
		public static readonly Tuple<TokenType, string> False = Tuple.Create (TokenType.False, "false");
		public static readonly Tuple<TokenType, string> Fn = Tuple.Create (TokenType.Fn, "fn");
		public static readonly Tuple<TokenType, string> Hidden = Tuple.Create (TokenType.Hidden, "hidden");
		public static readonly Tuple<TokenType, string> If = Tuple.Create (TokenType.If, "if");
		public static readonly Tuple<TokenType, string> Is = Tuple.Create (TokenType.Is, "is");
		public static readonly Tuple<TokenType, string> Import = Tuple.Create (TokenType.Import, "import");
		public static readonly Tuple<TokenType, string> Let = Tuple.Create (TokenType.Let, "let");
		public static readonly Tuple<TokenType, string> Module = Tuple.Create (TokenType.Module, "module");
		public static readonly Tuple<TokenType, string> New = Tuple.Create (TokenType.New, "new");
		public static readonly Tuple<TokenType, string> Null = Tuple.Create (TokenType.Null, "null");
		public static readonly Tuple<TokenType, string> Public = Tuple.Create (TokenType.Public, "public");
		public static readonly Tuple<TokenType, string> Return = Tuple.Create (TokenType.Return, "return");
		public static readonly Tuple<TokenType, string> Shared = Tuple.Create (TokenType.Shared, "shared");
		public static readonly Tuple<TokenType, string> Static = Tuple.Create (TokenType.Static, "static");
		public static readonly Tuple<TokenType, string> String = Tuple.Create (TokenType.String, "string");
		public static readonly Tuple<TokenType, string> Super = Tuple.Create (TokenType.Super, "super");
		public static readonly Tuple<TokenType, string> This = Tuple.Create (TokenType.This, "this");
		public static readonly Tuple<TokenType, string> True = Tuple.Create (TokenType.True, "true");
		public static readonly Tuple<TokenType, string> Var = Tuple.Create (TokenType.Var, "var");
		public static readonly Tuple<TokenType, string> While = Tuple.Create (TokenType.While, "while");

		/* Punctuation */
		public static readonly Tuple<TokenType, string> Dot = Tuple.Create(TokenType.Dot, "dot");
		public static readonly Tuple<TokenType, string> Comma = Tuple.Create(TokenType.Comma, ",");
		public static readonly Tuple<TokenType, string> Colon = Tuple.Create(TokenType.Colon, ":");
		public static readonly Tuple<TokenType, string> SemiColon = Tuple.Create(TokenType.SemiColon, ";");
		public static readonly Tuple<TokenType, string> LeftParenthesis = Tuple.Create(TokenType.LeftParenthesis, "(");
		public static readonly Tuple<TokenType, string> RightParenthesis = Tuple.Create(TokenType.RightParenthesis, ")");
		public static readonly Tuple<TokenType, string> LeftBraces = Tuple.Create(TokenType.LeftBraces, "{");
		public static readonly Tuple<TokenType, string> RightBraces = Tuple.Create(TokenType.RightBraces, "}");
		public static readonly Tuple<TokenType, string> LeftBracket = Tuple.Create(TokenType.LeftBracket, "[");
		public static readonly Tuple<TokenType, string> RightBracket = Tuple.Create(TokenType.RightBracket, "]");

		/* Arithmetic	*/
		public static readonly Tuple<TokenType, string> Plus = Tuple.Create(TokenType.Plus, "+");
		public static readonly Tuple<TokenType, string> Minus = Tuple.Create(TokenType.Minus, "-");
		public static readonly Tuple<TokenType, string> Times = Tuple.Create(TokenType.Times, "*");
		public static readonly Tuple<TokenType, string> Divide = Tuple.Create(TokenType.Divide, "/");

		/* Operators */
		public static readonly Tuple<TokenType, string> Assign = Tuple.Create(TokenType.Assign, "=");
		public static readonly Tuple<TokenType, string> Decrement = Tuple.Create(TokenType.Decrement, "--");
		public static readonly Tuple<TokenType, string> Increment = Tuple.Create(TokenType.Increment, "++");
		public static readonly Tuple<TokenType, string> SumAssignment = Tuple.Create(TokenType.SumAssignment, "+=");
		public static readonly Tuple<TokenType, string> DifferenceAssignment = Tuple.Create(TokenType.DifferenceAssignment, "-=");
		public static readonly Tuple<TokenType, string> ProductAssignment = Tuple.Create (TokenType.ProductAssignment, "*=");
		public static readonly Tuple<TokenType, string> RemainderAssignment = Tuple.Create (TokenType.RemainderAssignment, "/=");
		public static readonly Tuple<TokenType, string> Equal = Tuple.Create(TokenType.Equal, "==");
		public static readonly Tuple<TokenType, string> NotEqual = Tuple.Create (TokenType.NotEqual, "!=");
		public static readonly Tuple<TokenType, string> StrictEqual = Tuple.Create(TokenType.StrictEqual, "===");
		public static readonly Tuple<TokenType, string> GreaterThan = Tuple.Create(TokenType.GreaterThan, ">");
		public static readonly Tuple<TokenType, string> GreaterThanEqualTo = Tuple.Create(TokenType.GreaterThanEqualTo, ">=");
		public static readonly Tuple<TokenType, string> And = Tuple.Create(TokenType.And, "&&");
		public static readonly Tuple<TokenType, string> Or = Tuple.Create (TokenType.Or, "||");
		public static readonly Tuple<TokenType, string> LessThan = Tuple.Create(TokenType.LessThan, "<");
		public static readonly Tuple<TokenType, string> LessThanEqualTo = Tuple.Create(TokenType.LessThanEqualTo, "<=");
		public static readonly Tuple<TokenType, string> LogicalNot = Tuple.Create (TokenType.LogicalNot, "!");
		public static readonly Tuple<TokenType, string> Modulus = Tuple.Create (TokenType.Modulus, "%");

		/* Data type */
		public static Tuple<TokenType, string> CharacterLiteral = Tuple.Create (TokenType.CharacterLiteral,"");
		public static Tuple<TokenType, string> StringLiteral = Tuple.Create (TokenType.StringLiteral, "");
		public static Tuple<TokenType, string> Number = Tuple.Create(TokenType.Number, "");
		public static Tuple<TokenType, string> Id = Tuple.Create(TokenType.Id, "");
		public static Tuple<TokenType, string> Invalid = Tuple.Create(TokenType.Invalid, "");
		public static Tuple<TokenType, string> Keyword = Tuple.Create (TokenType.Keyword, "");
		/// <summary>
		/// The token names.
		/// </summary>
		private static readonly Dictionary<Tuple<TokenType, string>, string> tokenNames = new Dictionary<Tuple<TokenType, string>, string>(){
			{And, "And"}, {Assign, "Assign"}, {Char, "Char"}, {CharacterLiteral, "Character Literal"}, {Colon, "Colon"}, {Comma, "Comma"}, 
			{Decrement, "Decrement"}, {DifferenceAssignment, "Difference Assignment"}, {Divide, "Divide"}, {Dot, "Dot"}, {Else, "Else"}, 
			{Equal, "Equal"}, {GreaterThan, "Greater Than"}, {GreaterThanEqualTo, "Greater Than or Equal To"}, {Id, "Identifier"}, 
			{Increment, "Increment"}, {LeftBraces, "Left Braces"}, {LeftBracket, "Right Braces"}, {LeftParenthesis, "Left Parenthesis"}, 
			{LessThan, "Less Than"}, {LessThanEqualTo, "Less Than or Equal To"}, {LogicalNot, "Logical Not"}, {Minus, "Minus"},
			{Modulus, "Modulus"}, {NotEqual, "Not Equal"}, {Or, "Or"}, {Plus, "Plus"}, {ProductAssignment, "Product Assignment"},
			{RightBraces, "Right Braces"}, {RightBracket, "Right Bracket"}, {RightParenthesis, "Right Parenthesis"}, {SemiColon, "Semi-Colon"}, 
			{StrictEqual, "Strict Equal"},	{StringLiteral, "String Literal"}, {SumAssignment, "Sum Assignment"}, {Times, "Times"}
		};

		Source source;
		Tuple<TokenType, string> token;

		static Token () {
			keywords = new Dictionary<string, Tuple<TokenType, string>>{
				{As.Item2, As}, {Bool.Item2, Bool}, {Char.Item2, Char}, {Class.Item2, Class}, {Else.Item2, Else}, 
				{Extends.Item2, Extends}, {Export.Item2, Export}, {False.Item2, False}, {Fn.Item2, Fn}, {Hidden.Item2, Hidden}, 
				{Is.Item2, Is}, {Import.Item2, Import}, {Let.Item2, Let}, {Module.Item2, Module}, {New.Item2, New}, 
				{Null.Item2, Null}, {Public.Item2, Public}, {Return.Item2, Return}, {Shared.Item2, Shared}, 
				{Static.Item2, Static}, {String.Item2, String}, {Super.Item2, Super}, {This.Item2, This}, {True.Item2, True}, 
				{Var.Item2, Var}, {While.Item2, While}
			};
		} 

		public Token () {
		}

		public Token(Tuple<TokenType, string> token, Source source){
			this.token = token;
			this.source = source;
		}

		public Token(string keyword, Source source){
			this.token = keywords [keyword];
			this.source = source;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Metal.FrontEnd.Scan.Token"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="literal">Literal.</param>
		/// <param name="source">Source.</param>
		public Token(TokenType type, string literal, Source source){
			this.token = Tuple.Create (type, literal);
			this.source = source;
		}
		public Source Source {get { return source; } }

		/// <summary>
		/// Gets the name of the current token.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get { return GetTokenName (token); } }

		/// <summary>
		/// Gets the type of the current token.
		/// </summary>
		/// <value>The type.</value>
		public TokenType Type { get { return token.Item1; } }

		public override string ToString () {
			return tokenNames.ContainsKey(token) ? 
				string.Format ("Name={0}, {1}", Name, source.ToString ()) :
				string.Format ("Name={0}, Value={1}, {2}",Name, token.Item2, source.ToString ());
		}
		/// <summary>
		/// Gets the name of the token.
		/// </summary>
		/// <returns>The token name.</returns>
		/// <param name="token">Token.</param>
		public static string GetTokenName (Tuple<TokenType, string> token) {
			return tokenNames.ContainsKey(token) ? tokenNames[token] : new Func<string> (() => {
				switch (token.Item1) {
				case TokenType.CharacterLiteral:
					return "Character Literal";
				case TokenType.StringLiteral:
					return "String Literal";
				case TokenType.Number:
					return "Number";
				case TokenType.Id:
					return "Identifier";
				case TokenType.Invalid:
					return "Invalid";
				case TokenType.Keyword:
					return "Keyword";
				default:
					// TODO: Report error
					return "";
				}
			})();
		}
		public static bool IsKeyword(string key){
			return keywords.ContainsKey (key);
		}
	}
}


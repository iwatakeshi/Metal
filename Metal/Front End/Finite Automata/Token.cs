using System;
using System.Collections.Generic;
using Metal.IO;

namespace Metal.FrontEnd.FA {
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
		IncrementAssign,
		// -=
		DecrementAssign,
		// ==
		Equal,
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
		/*
			Data type
		*/
		// ^["|']([\s\S]*)["\']$
		StringLiteral,
		// [-+]?([0-9]*\.[0-9]+|[0-9]+)
		Number,
		// ^(?![0-9])[0-9A-Za-z]+
		Id,
		// Anything not defined by grammar.
		Invalid
	}

	public class FAToken {

		public static class Token {
			
			static List<Tuple<TokenType, string>> reserved = new List<Tuple<TokenType, string>>(){
				As, Bool, Char, Class, Else,
				Extends, Export, False, Fn, Hidden,
				Is, Import, Module, New, Null,
				Public, Return, Shared, Static, String,
				Super, This, True, While
			};
			public static List<Tuple<TokenType, string>> Reserved { get { return reserved; } }

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
			public static readonly Tuple<TokenType, string> IncrementAssign = Tuple.Create(TokenType.IncrementAssign, "+=");
			public static readonly Tuple<TokenType, string> DecrementAssign = Tuple.Create(TokenType.DecrementAssign, "-=");
			public static readonly Tuple<TokenType, string> Equal = Tuple.Create(TokenType.Equal, "==");
			public static readonly Tuple<TokenType, string> StrictEqual = Tuple.Create(TokenType.StrictEqual, "===");
			public static readonly Tuple<TokenType, string> GreaterThan = Tuple.Create(TokenType.GreaterThan, ">");
			public static readonly Tuple<TokenType, string> GreaterThanEqualTo = Tuple.Create(TokenType.GreaterThanEqualTo, ">=");
			public static readonly Tuple<TokenType, string> And = Tuple.Create(TokenType.And, "&&");
			public static readonly Tuple<TokenType, string> LessThan = Tuple.Create(TokenType.LessThan, "<");
			public static readonly Tuple<TokenType, string> LessThanEqualTo = Tuple.Create(TokenType.LessThanEqualTo, "<=");

			/* Data type */
			public static Tuple<TokenType, string> StringLiteral = Tuple.Create (TokenType.StringLiteral, "");
			public static Tuple<TokenType, string> Number = Tuple.Create(TokenType.Number, "");
			public static Tuple<TokenType, string> Id = Tuple.Create(TokenType.Id, "");
			public static Tuple<TokenType, string> Invalid = Tuple.Create(TokenType.Invalid, "");
		}

		Source source;
		Tuple<TokenType, string> token;
		public FAToken () {
			source = new Source ();
		}

		public FAToken(Tuple<TokenType, string> token, Source source){
			this.source = source;
			this.token = token;
		}

		public Source Source {get { return source; } }
		public string Name { get {return GetTokenTypeName (token); } }

		public override string ToString () {
			return string.Format ("Name={0}, {1}", Name, source.ToString ());
		}

		public static string GetTokenTypeName (Tuple<TokenType, string> token) {
			switch (token) {
			case Token.And:
				return "And";
			case Token.As:
				return "As";
			case Token.Assign:
				return "Assign";
			case Token.Bool:
				return "Minus";
			case Token.Char:
				return "Char";
			case Token.Class:
				return "Class";
			case Token.Colon:
				return "Colon";
			case Token.Comma:
				return "Comma";
			case Token.Decrement:
				return "Decrement";
			case Token.DecrementAssign:
				return "Decrement and Assign";
			case Token.Divide:
				return "Divide";
			case Token.Dot:
				return "Dot";
			case Token.Else:
				return "Else";
			case Token.Equal:
				return "Equal";
			case Token.Export:
				return "Export";
			case Token.Extends:
				return "Extends";
			case Token.False:
				return "False";
			case Token.Fn:
				return "Function";
			case Token.GreaterThan:
				return "Greater Than";
			case Token.GreaterThanEqualTo:
				return "Greater Than or Equal To";
			case Token.Hidden:
				return "Hidden";
			case Token.Id:
				return "Identifier";
			case Token.If:
				return "If";
			case Token.Import:
				return "import";
			case Token.Increment:
				return "Increment";
			case Token.IncrementAssign:
				return "Increment and Assign";
			case Token.Invalid:
				return "Invalid";
			case Token.Is:
				return "Is";
			case Token.LeftBraces:
				return "Left Brace";
			case Token.LeftBracket:
				return "Left Bracket";
			case Token.LeftParenthesis:
				return "Left Parenthesis";
			case Token.LessThan:
				return "Less Than";
			case Token.LessThanEqualTo:
				return "Less Than or Equal To";
			case Token.Minus:
				return "Minus";
			case Token.Module:
				return "Module";
			case Token.New:
				return "New";
			case Token.Null:
				return "Null";
			case Token.Number:
				return "Number";
			case Token.Plus:
				return "Plus";
			case Token.Public:
				return "Public";
			case Token.Return:
				return "Return";
			case Token.RightBraces:
				return "Right Brace";
			case Token.RightBracket:
				return "Right Bracket";
			case Token.RightParenthesis:
				return "Right Parenthesis";
			case Token.SemiColon:
				return "SemiColon";
			case Token.Shared:
				return "Shared";
			case Token.Static:
				return "Staic";
			case Token.StrictEqual:
				return "String Equal";
			case Token.String:
				return "String";
			case Token.StringLiteral:
				return "String Literal";
			case Token.Super:
				return "Super";
			case Token.This:
				return "This";
			case Token.Times:
				return "Times";
			case Token.Times:
				return "Times";
			case Token.True:
				return "True";
			case Token.While:
				return "While"; 
			default:
				return "";
			}
		}
	}
}


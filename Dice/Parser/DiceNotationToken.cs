using Superpower.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wgaffa.DMToolkit.Parser
{
	public enum DiceNotationToken
	{
		[Token(Category = "literal")]
		Number,

		[Token(Category = "literal")]
		String,

		[Token(Category = "operator", Example = "+")]
		Plus,

		[Token(Category = "operator", Example = "-")]
		Minus,

		[Token(Category = "operator", Example = "*")]
		Multiplication,

		[Token(Category = "operator", Example = "/")]
		Divide,

		[Token(Example = "(")]
		LParen,

		[Token(Example = ")")]
		RParen,

		[Token(Example = "[")]
		LBracket,

		[Token(Example = "]")]
		RBracket,

		[Token(Example = ",")]
		Comma,

		[Token(Example = ";")]
		SemiColon,

		[Token(Example = ":")]
		Colon,

		[Token(Category = "keyword")]
		Keyword,

		[Token(Example = "=")]
		Equal,

		Dice,

		[Token(Category = "repeat")]
		Repeat,

		Identifier,
	}
}

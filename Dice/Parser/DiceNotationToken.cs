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
		Number,

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

		Dice,

		[Token(Category = "repeat")]
		Repeat,

		Identifier,
	}
}

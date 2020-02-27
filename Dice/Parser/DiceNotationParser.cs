using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMToolkit.Parser
{
    public static class DiceNotationParser
    {
        private static readonly TextParser<IExpression> DiceParser =
            from rolls in Numerics.IntegerInt32.OptionalOrDefault(1)
            from _ in Character.EqualTo('d')
            from sides in Numerics.IntegerInt32
            select (IExpression)new DiceExpression(new Dice(sides), rolls);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Constant =
            Token.EqualTo(DiceNotationToken.Number)
            .Apply(Numerics.DecimalDouble)
            .Select(n => (IExpression)new NumberExpression((float)n))
            .Named("constant");

        private static readonly TokenListParser<DiceNotationToken, IExpression> DiceExpression =
            Token.EqualTo(DiceNotationToken.Dice)
            .Apply(DiceParser)
            .Named("dice");

        public static readonly TokenListParser<DiceNotationToken, IExpression> Expr =
            DiceExpression.Or(Constant);

        public static TokenListParser<DiceNotationToken, IExpression> Notation =
            Expr.AtEnd();
    }
}

using Superpower;
using Superpower.Parsers;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMToolkit.Parser
{
    public static class DiceNotationParser
    {
        private static readonly TokenListParser<DiceNotationToken, IExpression> Constant =
            Token.EqualTo(DiceNotationToken.Number)
            .Apply(Numerics.DecimalDouble)
            .Select(n => (IExpression)new NumberExpression((float)n));

        public static TokenListParser<DiceNotationToken, IExpression> Lambda =
            Constant.AtEnd();
    }
}

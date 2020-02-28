using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;

namespace Wgaffa.DMToolkit.Parser
{
    public static class DiceNotationParser
    {
        private enum OperatorType
        {
            Addition,
            Subtraction,
            Multiplication,
            Division,
        }

        private static readonly TextParser<IExpression> DiceParser =
            from rolls in Numerics.IntegerInt32.OptionalOrDefault(1)
            from _ in Character.EqualTo('d')
            from sides in Numerics.IntegerInt32
            select (IExpression)new DiceExpression(new Dice(sides), rolls);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Addition =
            Token.EqualTo(DiceNotationToken.Plus).Value(OperatorType.Addition);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Subtraction =
            Token.EqualTo(DiceNotationToken.Minus).Value(OperatorType.Subtraction);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Multiplication =
            Token.EqualTo(DiceNotationToken.Multiplication).Value(OperatorType.Multiplication);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Division =
            Token.EqualTo(DiceNotationToken.Divide).Value(OperatorType.Division);

        private static readonly TokenListParser<DiceNotationToken, IExpression> DiceExpression =
            Token.EqualTo(DiceNotationToken.Dice)
            .Apply(DiceParser);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Number =
            Token.EqualTo(DiceNotationToken.Number)
            .Apply(Numerics.DecimalDouble)
            .Select(n => (IExpression)new NumberExpression((float)n));

        private static readonly TokenListParser<DiceNotationToken, IExpression> Constant =
            Number.Or(DiceExpression);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Term =
            Parse.Chain(Multiplication.Or(Division), Constant, MakeBinary);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Expr =
            Parse.Chain(Addition.Or(Subtraction), Term, MakeBinary);

        private static IExpression MakeBinary(OperatorType @operator, IExpression left, IExpression right)
        {
            switch (@operator)
            {
                case OperatorType.Addition:
                    return new AdditionExpression(left, right);

                case OperatorType.Subtraction:
                    return new SubtractionExpression(left, right);

                case OperatorType.Multiplication:
                    return new MultiplicationExpression(left, right);

                case OperatorType.Division:
                    return new DivisionExpression(left, right);

                default:
                    throw new InvalidOperationException();
            }
        }

        public static TokenListParser<DiceNotationToken, IExpression> Notation =
            Expr.AtEnd();
    }
}

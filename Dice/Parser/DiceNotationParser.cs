using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Collections.Generic;
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
            DropLowest,
        }

        private static readonly TextParser<IExpression> DiceParser =
            from rolls in Numerics.IntegerInt32.OptionalOrDefault(1)
            from _ in Character.EqualTo('d')
            from sides in Numerics.IntegerInt32.Or(Character.EqualTo('%').Value(100))
            select (IExpression)new DiceExpression(new Dice(sides), rolls);

        private static readonly TextParser<int> RepeatParser =
            from repeat in Numerics.IntegerInt32
            from _ in Character.EqualTo('x')
            select repeat;

        private static readonly TextParser<int> KeepCount =
            from keepIdentifier in Character.EqualTo('k')
            from count in Numerics.IntegerInt32
            select count;

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Addition =
            Token.EqualTo(DiceNotationToken.Plus).Value(OperatorType.Addition);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Subtraction =
            Token.EqualTo(DiceNotationToken.Minus).Value(OperatorType.Subtraction);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Multiplication =
            Token.EqualTo(DiceNotationToken.Multiplication).Value(OperatorType.Multiplication);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Division =
            Token.EqualTo(DiceNotationToken.Divide).Value(OperatorType.Division);

        private static readonly TokenListParser<DiceNotationToken, DropExpression.StrategyFunc> DropLowest =
            Token.EqualToValue(DiceNotationToken.Identifier, "L").Value((DropExpression.StrategyFunc)(x => new int[] { x.Min() }));

        private static readonly TokenListParser<DiceNotationToken, DropExpression.StrategyFunc> DropHighest =
            Token.EqualToValue(DiceNotationToken.Identifier, "H").Value((DropExpression.StrategyFunc)(x => new int[] { x.Max() }));

        private static readonly TokenListParser<DiceNotationToken, IExpression> Identifier =
            Token.EqualTo(DiceNotationToken.Identifier)
            .Select(t => (IExpression)new VariableExpression(t.ToStringValue()));

        private static readonly TokenListParser<DiceNotationToken, IExpression> Dice =
            Token.EqualTo(DiceNotationToken.Dice)
            .Apply(DiceParser);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Keep =
            Token.EqualTo(DiceNotationToken.Dice)
            .Apply(DiceParser)
            .Then(expr => (from lparen in Token.EqualTo(DiceNotationToken.LParen)
                           from count in Token.EqualTo(DiceNotationToken.Identifier).Apply(KeepCount)
                           from rparen in Token.EqualTo(DiceNotationToken.RParen)
                           select (IExpression)new KeepExpression(expr, count)));

        private static readonly TokenListParser<DiceNotationToken, IExpression> Drop =
            Token.EqualTo(DiceNotationToken.Dice)
            .Apply(DiceParser)
            .Then(expr => (from minus in Token.EqualTo(DiceNotationToken.Minus)
                           from strategy in DropHighest.Or(DropLowest)
                           select (IExpression)new DropExpression(expr, strategy)));

        private static readonly TokenListParser<DiceNotationToken, IExpression> DiceNotation =
            Drop.Try()
            .Or(Keep.Try())
            .Or(Dice);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Number =
            Token.EqualTo(DiceNotationToken.Number)
            .Apply(Numerics.DecimalDouble)
            .Select(n => (IExpression)new NumberExpression(n));

        private static readonly TokenListParser<DiceNotationToken, IExpression> Constant =
            DiceNotation.Or(Identifier).Or(Number);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Factor =
            (from lparen in Token.EqualTo(DiceNotationToken.LParen)
             from expr in Parse.Ref(() => Expr)
             from rparen in Token.EqualTo(DiceNotationToken.RParen)
             select expr)
            .Or(Constant);

        private static readonly TokenListParser<DiceNotationToken, IExpression> List =
            (from lbracket in Token.EqualTo(DiceNotationToken.LBracket)
             from expr in Parse.Ref(() => Expr).AtLeastOnceDelimitedBy(Token.EqualTo(DiceNotationToken.Comma))
             from rbracket in Token.EqualTo(DiceNotationToken.RBracket)
             select (IExpression)new ListExpression(expr))
            .Named("list");

        private static readonly TokenListParser<DiceNotationToken, IExpression> Repeat =
            Token.EqualTo(DiceNotationToken.Repeat)
            .Apply(RepeatParser)
            .Then(x => List.Or(Factor).Select(f => (IExpression)new RepeatExpression(f, x)))
            .Named("repeat");

        private static readonly TokenListParser<DiceNotationToken, IExpression> FunctionCall =
            from identifier in Token.EqualTo(DiceNotationToken.Identifier).Apply(Superpower.Parsers.Identifier.CStyle)
            from lparen in Token.EqualTo(DiceNotationToken.LParen)
            from expr in Expr.ManyDelimitedBy(Token.EqualTo(DiceNotationToken.Comma))
            from rparen in Token.EqualTo(DiceNotationToken.RParen)
            select (IExpression)new FunctionCallExpression(identifier.ToStringValue(), expr);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Operand =
            (
            from _ in Token.EqualTo(DiceNotationToken.Minus)
            from factor in Factor
            select (IExpression)new NegateExpression(factor)
            )
            .Or(List)
            .Or(Repeat)
            .Or(FunctionCall.Try())
            .Or(Factor)
            .Named("expression");

        private static readonly TokenListParser<DiceNotationToken, IExpression> Term =
            Parse.Chain(Multiplication.Or(Division), Operand, MakeBinary);

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

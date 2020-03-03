﻿using Superpower;
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

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Addition =
            Token.EqualTo(DiceNotationToken.Plus).Value(OperatorType.Addition);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Subtraction =
            Token.EqualTo(DiceNotationToken.Minus).Value(OperatorType.Subtraction);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Multiplication =
            Token.EqualTo(DiceNotationToken.Multiplication).Value(OperatorType.Multiplication);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Division =
            Token.EqualTo(DiceNotationToken.Divide).Value(OperatorType.Division);

        private static readonly TokenListParser<DiceNotationToken, DropType> DropLowest =
            Token.EqualToValue(DiceNotationToken.Identifier, "L").Value(DropType.Lowest);

        private static readonly TokenListParser<DiceNotationToken, DropType> DropHighest =
            Token.EqualToValue(DiceNotationToken.Identifier, "H").Value(DropType.Highest);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Identifier =
            Token.EqualTo(DiceNotationToken.Identifier)
            .Select(t => (IExpression)new VariableExpression(t.ToStringValue()));

        private static readonly TokenListParser<DiceNotationToken, IExpression> Dice =
            Token.EqualTo(DiceNotationToken.Dice)
            .Apply(DiceParser);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Drop =
            Token.EqualTo(DiceNotationToken.Dice)
            .Apply(DiceParser)
            .Then(expr => (from minus in Token.EqualTo(DiceNotationToken.Minus)
                           from type in DropHighest.Or(DropLowest)
                           select (IExpression)new DropExpression(expr, type)));

        private static readonly TokenListParser<DiceNotationToken, IExpression> DiceNotation =
            Drop.Try().Or(Dice);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Number =
            Token.EqualTo(DiceNotationToken.Number)
            .Apply(Numerics.DecimalDouble)
            .Select(n => (IExpression)new NumberExpression((float)n));

        private static readonly TokenListParser<DiceNotationToken, IExpression> Constant =
            Number.Or(DiceNotation).Or(Identifier);

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

        private static readonly TokenListParser<DiceNotationToken, IExpression> Operand =
            (
            from _ in Token.EqualTo(DiceNotationToken.Minus)
            from factor in Factor
            select (IExpression)new NegateExpression(factor)
            )
            .Or(List)
            .Or(Repeat)
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

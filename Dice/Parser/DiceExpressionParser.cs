using DMTools.Die.Term;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DMTools.Die.Parser
{
    public class DiceExpressionParser
    {
        static readonly TextParser<(int timesToRoll, int sidesOfDie)> diceParser =
            from rolls in Numerics.Natural.OptionalOrDefault(new TextSpan("1"))
            from _ in Character.In(new[] { 'd', 'D' })
            from sides in Numerics.Natural
            select (Convert.ToInt32(rolls.ToStringValue()), Convert.ToInt32(sides.ToStringValue()));

        static readonly TokenListParser<DiceToken, ExpressionType> Add =
            Token.EqualTo(DiceToken.Plus).Value(ExpressionType.AddChecked);

        static readonly TokenListParser<DiceToken, ExpressionType> Subtract =
            Token.EqualTo(DiceToken.Minus).Value(ExpressionType.SubtractChecked);

        static readonly TokenListParser<DiceToken, ExpressionType> Multiply =
            Token.EqualTo(DiceToken.Multiply).Value(ExpressionType.MultiplyChecked);

        static readonly TokenListParser<DiceToken, ExpressionType> Divide =
            Token.EqualTo(DiceToken.Divide).Value(ExpressionType.Divide);

        static readonly TokenListParser<DiceToken, Expression> Dice =
            Token.EqualTo(DiceToken.Dice)
            .Apply(diceParser)
            .Select(d => (Expression)Expression.Constant((double)
                new TimesTerm(
                    new Dice(d.sidesOfDie, _randomGenerator)
                        , d.timesToRoll)
                        .GetResults().Sum()
                ));

        static readonly TokenListParser<DiceToken, Expression> Constant =
            Token.EqualTo(DiceToken.Number)
            .Apply(Numerics.DecimalDouble)
            .Select(n => (Expression)Expression.Constant(n));

        static readonly TokenListParser<DiceToken, Expression> Operand =
            (from sign in Token.EqualTo(DiceToken.Minus)
                from factor in Constant
                select (Expression)Expression.Negate(factor))
            .Or(Dice).Or(Constant).Named("expression");

        static readonly TokenListParser<DiceToken, Expression> Term =
            Parse.Chain(Multiply.Or(Divide), Operand, Expression.MakeBinary);

        static readonly TokenListParser<DiceToken, Expression> Expr =
            Parse.Chain(Add.Or(Subtract), Term, Expression.MakeBinary);

        static readonly TokenListParser<DiceToken, Expression> DiceExpression =
            Expr.Or(Dice).Or(Constant);

        static public readonly TokenListParser<DiceToken, Expression<Func<double>>> Lambda =
            DiceExpression.AtEnd().Select(body => Expression.Lambda<Func<double>>(body));
        
        static private IRandomGenerator _randomGenerator = new DefaultRandomGenerator();

        public static IRandomGenerator RandomGenerator {
            get => _randomGenerator;
            set
            {
                _randomGenerator = value ?? throw new ArgumentNullException("randomGenerator");
            }
        }
    }
}

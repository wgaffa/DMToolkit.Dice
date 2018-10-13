using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Parser
{
    public class DiceExpressionParser
    {
        static readonly TokenListParser<DiceToken, ExpressionType> Add =
            Token.EqualTo(DiceToken.Plus).Value(ExpressionType.AddChecked);

        static readonly TokenListParser<DiceToken, ExpressionType> Subtract =
            Token.EqualTo(DiceToken.Minus).Value(ExpressionType.SubtractChecked);

        static readonly TokenListParser<DiceToken, ExpressionType> Multiply =
            Token.EqualTo(DiceToken.Multiply).Value(ExpressionType.MultiplyChecked);

        static readonly TokenListParser<DiceToken, ExpressionType> Divide =
            Token.EqualTo(DiceToken.Divide).Value(ExpressionType.Divide);

        static readonly TokenListParser<DiceToken, Expression> Constant =
            Token.EqualTo(DiceToken.Number)
            .Apply(Numerics.IntegerInt32)
            .Select(n => (Expression)Expression.Constant(n));

        static readonly TokenListParser<DiceToken, Expression> Factor =
            (from expr in Parse.Ref(() => Expr)
             select expr)
            .Or(Constant);

        static readonly TokenListParser<DiceToken, Expression> Operand =
            (from sign in Token.EqualTo(DiceToken.Minus)
                from factor in Constant
                select (Expression)Expression.Negate(factor))
            .Or(Constant).Named("expression");

        static readonly TokenListParser<DiceToken, Expression> Term =
            Parse.Chain(Multiply.Or(Divide), Operand, Expression.MakeBinary);

        static readonly TokenListParser<DiceToken, Expression> Expr =
            Parse.Chain(Add.Or(Subtract), Term, Expression.MakeBinary);

        public static readonly TokenListParser<DiceToken, Expression<Func<int>>> Lambda =
            Expr.AtEnd().Select(body => Expression.Lambda<Func<int>>(body));
    }
}

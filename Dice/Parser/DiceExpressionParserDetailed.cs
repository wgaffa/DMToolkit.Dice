using DMTools.Die.Algorithm;
using DMTools.Die.Rollers;
using DMTools.Die.Term;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Globalization;
using System.Linq;

namespace DMTools.Die.Parser
{
    public class DiceExpressionParserDetailed
    {
        public DiceExpressionParserDetailed(IDiceRoller diceRoller)
        {
            _diceRoller = diceRoller ?? throw new ArgumentNullException(nameof(diceRoller));

            _diceParser =
            from rolls in Numerics.Natural.OptionalOrDefault(new TextSpan("1"))
                .Select(s => Convert.ToInt32(s.ToStringValue(), CultureInfo.CurrentCulture))
            from _ in Character.In(new[] { 'd', 'D' })
            from sides in _diceSidesParser
            select (rolls, sides);

            // Have to set the parsers in here or set all fields to static
            _dice =
                Token.EqualTo(DiceToken.Dice)
                .Apply(_diceParser)
                .Select(n => (IDiceExpression)new DiceTermExpression(
                    new TimesTerm(
                        new Dice(n.sidesOfDie, _diceRoller)
                        , n.timesToRoll
                        ))
                    );

            _operand =
                (from sign in Token.EqualTo(DiceToken.Minus)
                 from factor in _constant
                 select (IDiceExpression)new Negate(factor)
                 ).Or(_dice).Or(_constant).Named("expression");

            _term =
                Parse.Chain(_multiply.Or(_divide), _operand, BinaryExpression.MakeBinary);

            _expr =
                Parse.Chain(_add.Or(_subtract), _term, BinaryExpression.MakeBinary);

            _expression =
                _expr.Or(_dice).Or(_constant);
        }

        public IDiceExpression ParseString(string input)
        {
            DiceExpressionTokenizer tokenizer = new DiceExpressionTokenizer();
            var tokenlist = tokenizer.Tokenize(input);

            return _expression.Parse(tokenlist);
        }

        private readonly TextParser<int> _diceSidesParser =
                        from sides in Numerics.Natural.Or(Span.MatchedBy(Character.In('%')))
                        select sides.EqualsValue("%") ? 100 : Convert.ToInt32(sides.ToStringValue(), CultureInfo.CurrentCulture);

        private readonly TextParser<(int timesToRoll, int sidesOfDie)> _diceParser;

        private readonly TokenListParser<DiceToken, OperatorType> _add =
            Token.EqualTo(DiceToken.Plus).Value(OperatorType.Addition);

        private readonly TokenListParser<DiceToken, OperatorType> _subtract =
            Token.EqualTo(DiceToken.Minus).Value(OperatorType.Subtraction);

        private readonly TokenListParser<DiceToken, OperatorType> _multiply =
            Token.EqualTo(DiceToken.Multiply).Value(OperatorType.Multiplication);

        private readonly TokenListParser<DiceToken, OperatorType> _divide =
            Token.EqualTo(DiceToken.Divide).Value(OperatorType.Division);

        private readonly TokenListParser<DiceToken, IDiceExpression> _constant =
            Token.EqualTo(DiceToken.Number)
            .Apply(Numerics.DecimalDouble)
            .Select(n => (IDiceExpression)new Constant(n));

        private readonly TokenListParser<DiceToken, IDiceExpression> _dice;

        private readonly TokenListParser<DiceToken, IDiceExpression> _operand;

        private readonly TokenListParser<DiceToken, IDiceExpression> _term;

        private readonly TokenListParser<DiceToken, IDiceExpression> _expr;

        private readonly TokenListParser<DiceToken, IDiceExpression> _expression;
        private readonly IDiceRoller _diceRoller;
    }
}

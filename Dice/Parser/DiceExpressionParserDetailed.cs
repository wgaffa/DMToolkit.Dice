using DMTools.Die.Algorithm;
using DMTools.Die.Term;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Die.Parser
{
    public class DiceExpressionParserDetailed
    {
        public DiceExpressionParserDetailed(IRandomGenerator randomGenerator)
        {
            _randomGenerator = randomGenerator ?? throw new ArgumentNullException("randomGenerator");

            // Have to set the parsers in here or set all fields to static
            _dice =
                Token.EqualTo(DiceToken.Dice)
                .Apply(_diceParser)
                .Select(n => (IComponent)new DiceComponent(
                    new TimesTerm(
                        new Dice(n.sidesOfDie, _randomGenerator)
                        , n.timesToRoll
                        ))
                    );

            _operand =
                (from sign in Token.EqualTo(DiceToken.Minus)
                 from factor in _constant
                 select (IComponent)new Negate(factor)
                 ).Or(_dice).Or(_constant).Named("expression");

            _term =
                Parse.Chain(_multiply.Or(_divide), _operand, BinaryComponent.MakeBinary);

            _expr =
                Parse.Chain(_add.Or(_subtract), _term, BinaryComponent.MakeBinary);

            _expression =
                _expr.Or(_dice).Or(_constant);
        }

        public IComponent ParseString(string input)
        {
            DiceExpressionTokenizer tokenizer = new DiceExpressionTokenizer();
            var tokenlist = tokenizer.Tokenize(input);

            return _expression.Parse(tokenlist);
        }

        private readonly TextParser<(int timesToRoll, int sidesOfDie)> _diceParser =
            from rolls in Numerics.Natural.OptionalOrDefault(new TextSpan("1"))
            from _ in Character.In(new[] { 'd', 'D' })
            from sides in Numerics.Natural
            select (Convert.ToInt32(rolls.ToStringValue()), Convert.ToInt32(sides.ToStringValue()));

        private readonly TokenListParser<DiceToken, OperatorType> _add =
            Token.EqualTo(DiceToken.Plus).Value(OperatorType.Addition);

        private readonly TokenListParser<DiceToken, OperatorType> _subtract =
            Token.EqualTo(DiceToken.Minus).Value(OperatorType.Subtraction);

        private readonly TokenListParser<DiceToken, OperatorType> _multiply =
            Token.EqualTo(DiceToken.Multiply).Value(OperatorType.Multiplication);

        private readonly TokenListParser<DiceToken, OperatorType> _divide =
            Token.EqualTo(DiceToken.Divide).Value(OperatorType.Division);

        private readonly TokenListParser<DiceToken, IComponent> _constant =
            Token.EqualTo(DiceToken.Number)
            .Apply(Numerics.DecimalDouble)
            .Select(n => (IComponent)new Constant(n));

        private readonly TokenListParser<DiceToken, IComponent> _dice;

        private readonly TokenListParser<DiceToken, IComponent> _operand;

        private readonly TokenListParser<DiceToken, IComponent> _term;

        private readonly TokenListParser<DiceToken, IComponent> _expr;

        private readonly TokenListParser<DiceToken, IComponent> _expression;

        private IRandomGenerator _randomGenerator = new DefaultRandomGenerator();
    }
}

using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Die.Parser
{
    public class DiceExpressionTokenizer : Tokenizer<DiceToken>
    {
        readonly Dictionary<char, DiceToken> _operators = new Dictionary<char, DiceToken>
        {
            ['+'] = DiceToken.Plus,
            ['-'] = DiceToken.Minus,
            ['*'] = DiceToken.Multiply,
            ['/'] = DiceToken.Divide
        };

        protected override IEnumerable<Result<DiceToken>> Tokenize(TextSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {

                if (next.Value.ToString(CultureInfo.InvariantCulture).ToUpperInvariant() == "D")
                {
                    var diceStart = next.Location;

                    // Go past the letter 'd'
                    next = next.Remainder.ConsumeChar();

                    // Should be a positive number after the letter 'd'
                    Result<TextSpan> natural = ParseDiceSides(ref next);

                    if (!natural.HasValue)
                    {
                        yield return Result.Empty<DiceToken>(next.Location, new[] { "dice" });
                    }

                    next = natural.Remainder.ConsumeChar();
                    yield return Result.Value(DiceToken.Dice, diceStart, next.Remainder);
                }
                else if (Char.IsDigit(next.Value))
                {
                    // Keep a location of the start if this is a dice token
                    var tokenStart = next.Location;

                    // This can be a Number or a Dice token
                    var natural = Numerics.Decimal(next.Location);
                    next = natural.Remainder.ConsumeChar();

                    if (next.HasValue && (next.Value == 'd' || next.Value == 'D'))
                    {
                        // We are now sure it's a Dice token
                        StringBuilder diceString = new StringBuilder();
                        diceString.Append(natural.Value);

                        // Past letter 'd'
                        next = next.Remainder.ConsumeChar();

                        var sides = ParseDiceSides(ref next);

                        if (!sides.HasValue)
                        {
                            yield return Result.Empty<DiceToken>(next.Location, new[] { "dice" });
                        }

                        next = sides.Remainder.ConsumeChar();
                        yield return Result.Value(DiceToken.Dice, tokenStart, sides.Remainder);
                    }
                    else
                    {
                        yield return Result.Value(DiceToken.Number, natural.Location, natural.Remainder);
                    }
                }
                else if (_operators.TryGetValue(next.Value, out DiceToken charToken))
                {
                    yield return Result.Value(charToken, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else
                {
                    yield return Result.Empty<DiceToken>(next.Location, new[] { "number", "operator" });
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }

        private Result<TextSpan> ParseDiceSides(ref Result<char> next)
        {
            return _hundredSidedDieParser
                                    .Select(s => s.EqualsValue("%") ? new TextSpan("100") : s)
                                    .TryParse(next.Location.ToStringValue());
        }

        private readonly TextParser<TextSpan> _hundredSidedDieParser =
                        from sides in Numerics.Natural.Or(Span.MatchedBy(Character.In('%')))
                        select sides;
    }
}

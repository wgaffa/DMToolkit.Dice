using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System.Collections.Generic;

namespace Wgaffa.DMToolkit.Parser
{
    public class DiceNotationTokenizer : Tokenizer<DiceNotationToken>
    {
        private readonly Dictionary<char, DiceNotationToken> _operators = new Dictionary<char, DiceNotationToken>()
        {
            ['+'] = DiceNotationToken.Plus,
            ['-'] = DiceNotationToken.Minus,
            ['*'] = DiceNotationToken.Times,
            ['/'] = DiceNotationToken.Divide,
            ['('] = DiceNotationToken.LParen,
            [')'] = DiceNotationToken.RParen
        };

        protected override IEnumerable<Result<DiceNotationToken>> Tokenize(TextSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {
                var ch = next.Value;

                if (ch >= '0' && ch <= '9')
                {
                    var beginNumber = next.Location;
                    var natural = Numerics.Natural(next.Location);
                    next = natural.Remainder.ConsumeChar();

                    if (next.HasValue && next.Value == 'd')
                    {
                        next = next.Remainder.ConsumeChar();
                        var sides = Numerics.Natural(next.Location);
                        next = sides.Remainder.ConsumeChar();
                        yield return Result.Value(DiceNotationToken.Dice, beginNumber, sides.Remainder);
                    }
                    else if (next.HasValue && next.Value == '.')
                    {
                        next = next.Remainder.ConsumeChar();
                        var decimals = Numerics.Natural(next.Location);
                        next = decimals.Remainder.ConsumeChar();
                        yield return Result.Value(DiceNotationToken.Number, beginNumber, decimals.Remainder);
                    }
                    else
                    {
                        yield return Result.Value(DiceNotationToken.Number, natural.Location, natural.Remainder);
                    }
                }
                else if (ch == 'd')
                {
                    var beginDice = next.Location;
                    next = next.Remainder.ConsumeChar(); // consume 'd'

                    var natural = Numerics.Natural(next.Location);
                    next = natural.Remainder.ConsumeChar();

                    yield return Result.Value(DiceNotationToken.Dice, beginDice, natural.Remainder);
                }
                else if (_operators.TryGetValue(ch, out var charToken))
                {
                    yield return Result.Value(charToken, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else
                {
                    yield return Result.Empty<DiceNotationToken>(next.Location, new[] { "number", "operator" });
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }
    }
}

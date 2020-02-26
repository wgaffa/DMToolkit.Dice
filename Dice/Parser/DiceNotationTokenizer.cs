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
                    var diceParser = Span.Regex("\\d*d\\d+");
                    var dice = diceParser(next.Location);

                    if (dice.HasValue)
                    {
                        yield return Result.Value(DiceNotationToken.Dice, dice.Location, dice.Remainder);
                        next = dice.Remainder.ConsumeChar();
                    }
                    else
                    {
                        var beginNumber = next.Location;
                        var natural = Numerics.Natural(next.Location);
                        next = natural.Remainder.ConsumeChar();

                        if (next.HasValue && next.Value == '.')
                        {
                            next = next.Remainder.ConsumeChar();
                            var decimals = Numerics.Natural(next.Location);

                            yield return Result.Value(DiceNotationToken.Number, beginNumber, decimals.Remainder);
                            next = next.Remainder.ConsumeChar();
                        }
                        else
                        {
                            yield return Result.Value(DiceNotationToken.Number, beginNumber, next.Location);
                        }
                    }

                    if (!IsDelimiter(next))
                        yield return Result.Empty<DiceNotationToken>(next.Location);
                }
                else if (ch == 'd')
                {
                    var beginDice = next.Location;
                    next = next.Remainder.ConsumeChar();

                    var natural = Numerics.Natural(next.Location);

                    if (natural.HasValue)
                        yield return Result.Value(DiceNotationToken.Dice, beginDice, natural.Remainder);
                    else
                        yield return Result.Empty<DiceNotationToken>(natural.Location, new[] { "dice" });

                    next = natural.Remainder.ConsumeChar();

                    if (!IsDelimiter(next))
                        yield return Result.Empty<DiceNotationToken>(next.Location);
                }
                else if (_operators.TryGetValue(ch, out var charToken))
                {
                    yield return Result.Value(charToken, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else
                {
                    yield return Result.Empty<DiceNotationToken>(next.Location, new[] { "number", "operator", "dice" });
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }

        private static bool IsDelimiter(Result<char> next)
        {
            return !next.HasValue
                || char.IsWhiteSpace(next.Value);
        }
    }
}

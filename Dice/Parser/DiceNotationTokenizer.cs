﻿using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System.Collections.Generic;

namespace Wgaffa.DMToolkit.Parser
{
    public class DiceNotationTokenizer : Tokenizer<DiceNotationToken>
    {
        private readonly static TextParser<TextSpan> _diceParser = Span.Regex("\\d*d(\\d+|%)");
        private readonly static TextParser<TextSpan> _repeatParser = Span.Regex("\\d+x");

        private readonly static Dictionary<char, DiceNotationToken> _operators = new Dictionary<char, DiceNotationToken>()
        {
            ['+'] = DiceNotationToken.Plus,
            ['-'] = DiceNotationToken.Minus,
            ['*'] = DiceNotationToken.Multiplication,
            ['/'] = DiceNotationToken.Divide,
            ['('] = DiceNotationToken.LParen,
            [')'] = DiceNotationToken.RParen,
            ['['] = DiceNotationToken.LBracket,
            [']'] = DiceNotationToken.RBracket,
            [','] = DiceNotationToken.Comma,
        };

        protected override IEnumerable<Result<DiceNotationToken>> Tokenize(TextSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {
                var ch = next.Value;

                var dice = _diceParser(next.Location);
                var repeat = _repeatParser(next.Location);

                if (dice.HasValue)
                {
                    yield return Result.Value(DiceNotationToken.Dice, dice.Location, dice.Remainder);
                    next = dice.Remainder.ConsumeChar();

                    if (!IsDelimiter(next))
                        yield return Result.Empty<DiceNotationToken>(next.Location);
                }
                else if (repeat.HasValue)
                {
                    yield return Result.Value(DiceNotationToken.Repeat, repeat.Location, repeat.Remainder);
                    next = repeat.Remainder.ConsumeChar();
                }
                else if (ch >= '0' && ch <= '9')
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

                    if (!IsDelimiter(next))
                        yield return Result.Empty<DiceNotationToken>(next.Location);
                }
                else if (_operators.TryGetValue(ch, out var charToken))
                {
                    yield return Result.Value(charToken, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else if (char.IsLetter(ch) && ch != 'd')
                {
                    var beginIdentifier = next.Location;
                    do
                    {
                        next = next.Remainder.ConsumeChar();
                    } while (next.HasValue && char.IsLetterOrDigit(next.Value));

                    yield return Result.Value(DiceNotationToken.Identifier, beginIdentifier, next.Location);
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
                || char.IsWhiteSpace(next.Value)
                || _operators.ContainsKey(next.Value);
        }
    }
}

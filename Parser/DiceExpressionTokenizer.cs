﻿using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Parser
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
                DiceToken charToken;

                if (next.Value.ToString().ToLower() == "d")
                {
                    StringBuilder diceBuilder = new StringBuilder();

                    var diceStart = next.Location;
                    diceBuilder.Append(next.Value);

                    // Go past the letter 'd'
                    next = next.Remainder.ConsumeChar();

                    // Should be a positive number after the letter 'd'
                    var natural = Numerics.Natural(next.Location);

                    if (!natural.HasValue)
                    {
                        yield return Result.Empty<DiceToken>(next.Location, new[] { "number", "operator" });
                    }

                    diceBuilder.Append(natural.Value);

                    next = natural.Remainder.ConsumeChar();
                    yield return Result.Value(DiceToken.Dice, diceStart, next.Remainder);
                }
                else if (Char.IsDigit(next.Value))
                {
                    var natural = Numerics.Natural(next.Location);
                    next = natural.Remainder.ConsumeChar();
                    yield return Result.Value(DiceToken.Number, natural.Location, natural.Remainder);
                }
                else if (_operators.TryGetValue(next.Value, out charToken))
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

        private TextParser<string> diceParser =
            from rolls in Numerics.Natural.OptionalOrDefault(new TextSpan("1"))
            from _ in Character.In(new[] { 'd', 'D' })
            from sides in Numerics.Natural
            select rolls.ToString() + 'd' + sides;
    }
}

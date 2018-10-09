using Superpower;
using Superpower.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Parser
{
    public enum DiceToken
    {
        Undefined,
        Dice,
        Operator,
        Number
    }

    public class DiceExpressionParser : Tokenizer<DiceToken>
    {
        protected override IEnumerable<Result<DiceToken>> Tokenize(TextSpan input)
        {
            var next = SkipWhiteSpace(input);

            if (!next.HasValue)
            {
                yield break;
            }

            do
            {
                // Could be 1d12 or a real number
                if (char.IsLetter(next.Value))
                {
                    
                }
            } while (next.HasValue);
        }
    }
}

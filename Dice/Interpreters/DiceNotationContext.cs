using Ardalis.GuardClauses;
using System.Collections.Generic;
using Wgaffa.DMToolkit.DiceRollers;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Parser;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class DiceNotationContext
    {
        public IExpression Expression { get; }
        public ISymbolTable SymbolTable { get; set; }
        public IDiceRoller DiceRoller { get; set; }
        public List<RollResult> RollResults { get; } = new List<RollResult>();

        public DiceNotationContext(IExpression expression)
        {
            Guard.Against.Null(expression, nameof(expression));

            Expression = expression;
        }
    }
}

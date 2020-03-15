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
        public IExpression Result { get; set; }
        public ISymbolTable SymbolTable { get; set; }
        public IDiceRoller DiceRoller { get; set; }
        public List<RollResultExpression> RollResults { get; } = new List<RollResultExpression>();

        public DiceNotationContext(IExpression expression)
        {
            Guard.Against.Null(expression, nameof(expression));

            Expression = expression;
        }
    }
}

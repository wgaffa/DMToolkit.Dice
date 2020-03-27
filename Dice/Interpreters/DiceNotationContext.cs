using Ardalis.GuardClauses;
using System.Collections.Generic;
using Wgaffa.DMToolkit.DiceRollers;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Parser;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class DiceNotationContext
    {
        public ISymbolTable SymbolTable { get; set; }
        public IDiceRoller DiceRoller { get; set; }
        public List<RollResult> RollResults { get; } = new List<RollResult>();
    }
}

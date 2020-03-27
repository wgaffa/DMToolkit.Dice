using Wgaffa.DMToolkit.DiceRollers;
using Wgaffa.DMToolkit.Parser;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class Configuration
    {
        public ISymbolTable SymbolTable { get; set; }
        public IDiceRoller DiceRoller { get; set; }
    }
}
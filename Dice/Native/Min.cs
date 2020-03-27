using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Interpreters;

namespace Wgaffa.DMToolkit.Native
{
    public class Min : ICallable
    {
        public int Arity => 2;

        public object Call(DiceNotationInterpreter _, IEnumerable<object> arguments)
        {
            return arguments.Cast<double>().Min();
        }
    }
}
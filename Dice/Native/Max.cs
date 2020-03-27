using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Interpreters;

namespace Wgaffa.DMToolkit.Native
{
    public class Max : ICallable
    {
        public int Arity => 2;

        public object Call(DiceNotationInterpreter interpreter, IEnumerable<object> arguments)
        {
            return arguments.Cast<double>().Max();
        }
    }
}
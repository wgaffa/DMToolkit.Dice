using System.Collections.Generic;
using Wgaffa.DMToolkit.Interpreters;

namespace Wgaffa.DMToolkit.Expressions
{
    public interface ICallable
    {
        int Arity { get; }

        object Call(DiceNotationInterpreter interpreter, IEnumerable<object> arguments);
    }
}
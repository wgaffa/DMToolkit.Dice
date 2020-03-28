using System;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Interpreters;

namespace Wgaffa.DMToolkit.Native
{
    public class Print : ICallable
    {
        public int Arity => 1;

        public object Call(DiceNotationInterpreter interpreter, IEnumerable<object> arguments)
        {
            arguments.ToList().ForEach(Console.WriteLine);

            return null;
        }
    }
}
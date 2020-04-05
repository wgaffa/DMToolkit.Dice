using System;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Interpreters;
using Wgaffa.DMToolkit.Types;

namespace Wgaffa.DMToolkit.Native
{
    public class Print : ICallable
    {
        public int Arity => 1;

        public object Call(DiceNotationInterpreter interpreter, IEnumerable<object> arguments)
        {
            object argument = arguments.First();

            switch (argument)
            {
                case bool b:
                    Console.WriteLine(b.ToString().ToLower());
                    break;

                default:
                    Console.WriteLine(argument);
                    break;
            }

            return Unit.Value;
        }
    }
}
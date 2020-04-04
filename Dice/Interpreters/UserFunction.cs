using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.DMToolkit.Statements;

namespace Wgaffa.DMToolkit.Interpreters
{
    public class UserFunction : ICallable
    {
        private readonly Function _declaration;

        public UserFunction(Function declaration)
        {
            _declaration = declaration;
        }

        public int Arity => _declaration.Parameters.Count;

        public object Call(DiceNotationInterpreter interpreter, IEnumerable<object> arguments)
        {
            var env = interpreter.CurrentEnvironment;

            var argumentList = arguments.ToList();
            for (int i = 0; i < Arity; i++)
            {
                env[_declaration.Parameters[i].Value] = argumentList[i];
            }

            interpreter.Execute(_declaration.Body);

            return null;
        }
    }
}

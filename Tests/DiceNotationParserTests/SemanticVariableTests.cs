using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Interpreters;
using Wgaffa.DMToolkit.Interpreters.Errors;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace DiceNotationParserTests
{
    public class SemanticVariableTests
    {
        public class SetupBuiltinSymbols : IEnumerable<ISymbol>
        {
            public IEnumerator<ISymbol> GetEnumerator()
            {
                yield return new BuiltinTypeSymbol("real");
                yield return new BuiltinTypeSymbol("int");
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)GetEnumerator();
            }
        }

        [Test]
        public void Analyze_ShouldReturnError_GivenNoDefinedVariable()
        {
            var symbolTable = new SymbolTable(new SetupBuiltinSymbols());
            var semantic = new SemanticAnalyzer();
            var expression = new VariableExpression("foo");
            var context = new DiceNotationContext(expression)
            {
                SymbolTable = symbolTable
            };

            Result<IExpression, IList<SemanticError>> result = semantic.Analyze(context);

            List<SemanticError> errors = new List<SemanticError>();
            result.OnError(l => errors.AddRange(l));

            Assert.That(errors.Count, Is.EqualTo(1));
        }
    }
}

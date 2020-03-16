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

        public static readonly List<IExpression> InvalidVariableTestCaseData = new List<IExpression>()
        {
            new VariableExpression("foo"),
            new VariableDeclarationExpression(new string[] { "foo" }, "fake"),
            new VariableDeclarationExpression(new string[] { "bar", "bar" }, "int"),
            new AssignmentExpression("foo", new NumberExpression(3)),
            new CompoundExpression(new List<IExpression>()
            {
                new VariableDeclarationExpression(new string[] { "foo" }, "int"),
                new AssignmentExpression("foo", new VariableExpression("bar")),
            }),
        };

        [TestCaseSource(nameof(InvalidVariableTestCaseData))]
        public void Analyze_ShouldReturnError_GivenNoDefinedVariable(IExpression expression)
        {
            var symbolTable = new SymbolTable(new SetupBuiltinSymbols());
            var semantic = new SemanticAnalyzer();
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

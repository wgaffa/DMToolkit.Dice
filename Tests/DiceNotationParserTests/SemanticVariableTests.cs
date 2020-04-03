using NUnit.Framework;
using Superpower;
using System.Collections;
using System.Collections.Generic;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Interpreters;
using Wgaffa.DMToolkit.Interpreters.Errors;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.DMToolkit.Statements;
using Wgaffa.Functional;

namespace DiceNotationParserTests
{
    public class SemanticVariableTests
    {
        public class SetupBuiltinSymbols : IEnumerable<Symbol>
        {
            public IEnumerator<Symbol> GetEnumerator()
            {
                yield return new BuiltinTypeSymbol("real");
                yield return new BuiltinTypeSymbol("int");
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)GetEnumerator();
            }
        }

        public static readonly List<string> InvalidVariableTestCaseData = new List<string>()
        {
            "foo;",
            "fake foo;",
            "int bar, bar;",
            "foo = 3;",
            "int foo; foo = bar;",
            "int foo, bar = zar;",
            "int foo; def foo = 10;",
        };

        [TestCaseSource(nameof(InvalidVariableTestCaseData))]
        public void Analyze_ShouldReturnError_GivenInvalidString(string input)
        {
            var tokens = new DiceNotationTokenizer().Tokenize(input);
            var expression = DiceNotationParser.Program.Parse(tokens);

            var symbolTable = new ScopedSymbolTable(new SetupBuiltinSymbols());
            var configuration = new Configuration()
            {
                SymbolTable = symbolTable
            };
            var semantic = new SemanticAnalyzer(configuration);

            Result<IStatement, IList<SemanticError>> result = semantic.Analyze(expression);

            List<SemanticError> errors = new List<SemanticError>();
            result.OnError(l => errors.AddRange(l));

            Assert.That(errors.Count, Is.EqualTo(1));
        }

        public static List<string> ValidTestCaseData = new List<string>()
        {
            "int foo() real bar; end real bar;",
            "int bar; int foo() int bar; end",
            "def Attack = 5; Attack;",
            "int x; inf foo(int a, int b) int bar(int a, int b) 10 + a + b; end a - b; end",
        };

        [TestCaseSource(nameof(ValidTestCaseData))]
        public void Analyze_ShouldSucceed(string input)
        {
            var tokens = new DiceNotationTokenizer().Tokenize(input);
            var program = DiceNotationParser.Program.Parse(tokens);

            var global = new ScopedSymbolTable(new SetupBuiltinSymbols());
            var configuration = new Configuration() { SymbolTable = global };
            var semantic = new SemanticAnalyzer(configuration);

            bool success = false;
            var result = semantic.Analyze(program).OnSuccess(_ => success = true);

            Assert.That(success, Is.True);
        }
    }
}

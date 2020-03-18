﻿using NUnit.Framework;
using Superpower;
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

        public static readonly List<string> InvalidVariableTestCaseData = new List<string>()
        {
            "foo",
            "fake foo;",
            "int bar, bar;",
            "foo = 3",
            "int foo; foo = bar;",
            "int foo, bar = zar",
            "int foo; def foo = 10;",
        };

        [TestCaseSource(nameof(InvalidVariableTestCaseData))]
        public void Analyze_ShouldReturnError_GivenNoDefinedVariable(string input)
        {
            var tokens = new DiceNotationTokenizer().Tokenize(input);
            var expression = DiceNotationParser.Notation.Parse(tokens);

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

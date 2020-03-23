using Moq;
using NUnit.Framework;
using Superpower;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit;
using Wgaffa.DMToolkit.DiceRollers;
using Wgaffa.DMToolkit.Interpreters;
using Wgaffa.DMToolkit.Parser;

namespace DiceNotationParserTests
{
    public class ParseEvaluateTests
    {
        private Mock<IDiceRoller> _mockRoller;
        private ISymbolTable _symbolTable;
        private DiceNotationInterpreter _interpreter;

        [SetUp]
        public void SetUp()
        {
            _mockRoller = new Mock<IDiceRoller>();

            _mockRoller.SetupSequence(d => d.RollDice(It.IsAny<Dice>()))
                .Returns(3)
                .Returns(9)
                .Returns(18)
                .Returns(18)
                .Returns(1)
                .Returns(1)
                .Returns(20);

            _symbolTable = new ScopedSymbolTable();
            var realSymbol = new BuiltinTypeSymbol("real");
            var intSymbol = new BuiltinTypeSymbol("int");
            var parameters = new ISymbol[] { new VariableSymbol("a", realSymbol), new VariableSymbol("b", realSymbol) };
            _symbolTable.Add(realSymbol);
            _symbolTable.Add(intSymbol);
            _symbolTable.Add(new FunctionSymbol("max", realSymbol, x => (double)((double)x["a"] > (double)x["b"] ? x["a"] : x["b"]), parameters));
            _symbolTable.Add(new FunctionSymbol("min", realSymbol, x => (double)((double)x["a"] <= (double)x["b"] ? x["a"] : x["b"]), parameters));

            _symbolTable.Add(new VariableSymbol("INTMOD", intSymbol));
            _symbolTable.Add(new VariableSymbol("STRMOD", intSymbol));
            _symbolTable.Add(new VariableSymbol("DEXMOD", intSymbol));
            _symbolTable.Add(new VariableSymbol("BAB", intSymbol));
            _symbolTable.Add(new VariableSymbol("Ranks", intSymbol));
            _symbolTable.Add(new VariableSymbol("ClassSkill", intSymbol));
            _symbolTable.Add(new VariableSymbol("Size", intSymbol));
            _symbolTable.Add(new VariableSymbol("Misc", intSymbol));
            _symbolTable.Add(new VariableSymbol("ArmorBonus", intSymbol));
            _symbolTable.Add(new VariableSymbol("ShieldBonus", intSymbol));
            _symbolTable.Add(new VariableSymbol("RangePenalty", intSymbol));
            _symbolTable.Add(new VariableSymbol("MaxDex", intSymbol));

            _interpreter = new DiceNotationInterpreter();
        }

        public class NotationTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("1+5")
                    .Returns(6);
                yield return new TestCaseData("1.5 * d6")
                    .Returns(4.5);
                yield return new TestCaseData("6d20-L")
                    .Returns(49);
                yield return new TestCaseData("4d20-H")
                    .Returns(30);
                yield return new TestCaseData("4d20(k3)")
                    .Returns(45);
            }
        }

        public class PFRollsTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("d20 + INTMOD + Ranks + ClassSkill + Misc")
                    .Returns(11)
                    .SetName("Skill check roll");
                yield return new TestCaseData("d20 + STRMOD + BAB + Size")
                    .Returns(10)
                    .SetName("Attackroll melee small");
                yield return new TestCaseData("d20 + DEXMOD + BAB + Size + RangePenalty")
                    .Returns(8)
                    .SetName("Attackroll ranged small");
                yield return new TestCaseData("10 + ArmorBonus + ShieldBonus + DEXMOD + Size + Misc")
                    .Returns(22)
                    .SetName("ArmorBonus");
                yield return new TestCaseData("10 + ArmorBonus + ShieldBonus + max(0, min(DEXMOD, MaxDex)) + Size + Misc")
                    .Returns(20)
                    .SetName("ArmorBonus with MaxDex");
            }
        }

        public class FunctionTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("5+max(7, 3+8)")
                    .Returns(16);
                yield return new TestCaseData("int Add5(int a) a + 5; end Add5(10);")
                    .Returns(15)
                    .Ignore("Not fully implemented, was created at the wrong step");
                yield return new TestCaseData("real Pi() 3.14; end Pi();")
                    .Returns(3.14)
                    .Ignore("Not fully implemented, was created at the wrong step");
            }
        }

        public class VariableTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("real foo; foo = 5.2;")
                    .Returns(5.2);
                yield return new TestCaseData("int foo, bar; foo = 5; bar = 2d6; foo+bar;")
                    .Returns(17);
                yield return new TestCaseData("int foo, bar = 5; foo+bar;")
                    .Returns(10);
                yield return new TestCaseData("def Attack = (d20 + 5) * 1.5; Attack + 5;")
                    .Returns(17);
            }
        }

        public class VariableSetup : IEnumerable<KeyValuePair<string, double>>
        {
            public IEnumerator<KeyValuePair<string, double>> GetEnumerator()
            {
                yield return new KeyValuePair<string, double>("INTMOD", 3);
                yield return new KeyValuePair<string, double>("Ranks", 2);
                yield return new KeyValuePair<string, double>("ClassSkill", 3);
                yield return new KeyValuePair<string, double>("STRMOD", 2);
                yield return new KeyValuePair<string, double>("BAB", 4);
                yield return new KeyValuePair<string, double>("Size", 1);
                yield return new KeyValuePair<string, double>("RangePenalty", -4);
                yield return new KeyValuePair<string, double>("ArmorBonus", 5);
                yield return new KeyValuePair<string, double>("ShieldBonus", 2);
                yield return new KeyValuePair<string, double>("MaxDex", 2);
                yield return new KeyValuePair<string, double>("DEXMOD", 4);
                yield return new KeyValuePair<string, double>("Misc", 0);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)GetEnumerator();
            }
        }

        [TestCaseSource(typeof(NotationTestCaseData))]
        [TestCaseSource(typeof(PFRollsTestCaseData))]
        [TestCaseSource(typeof(FunctionTestCaseData))]
        [TestCaseSource(typeof(VariableTestCaseData))]
        public double Evaluate_ShouldReturnCorrect(string input)
        {
            var tokenlist = new DiceNotationTokenizer().Tokenize(input);
            var expression = DiceNotationParser.Notation.Parse(tokenlist);

            var context = new DiceNotationContext(expression)
            {
                DiceRoller = _mockRoller.Object,
                SymbolTable = _symbolTable,
            };

            var semantic = new SemanticAnalyzer();
            double result = 0;
            var ast = semantic.Analyze(context);
            ast.Map(expr =>
                _interpreter.Interpret(new DiceNotationContext(expr)
                { SymbolTable = context.SymbolTable, DiceRoller = _mockRoller.Object },
                    new VariableSetup()))
                .OnSuccess(r => result = r);

            return result;
        }
    }
}
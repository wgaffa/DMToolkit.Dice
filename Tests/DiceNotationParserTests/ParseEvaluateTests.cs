using Moq;
using NUnit.Framework;
using Superpower;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.DMToolkit;
using Wgaffa.DMToolkit.DiceRollers;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Interpreters;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace DiceNotationParserTests
{
    public class ParseEvaluateTests
    {
        private Mock<IDiceRoller> _mockRoller;
        private Mock<ISymbolTable> _symbolTable;
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

            _symbolTable = new Mock<ISymbolTable>();
            _symbolTable.SetupGet(x => x["INTMOD"])
                .Returns(new NumberExpression(3));
            _symbolTable.SetupGet(x => x["STRMOD"])
                .Returns(new NumberExpression(2));
            _symbolTable.SetupGet(x => x["DEXMOD"])
                .Returns(new NumberExpression(4));
            _symbolTable.SetupGet(x => x["Ranks"])
                .Returns(new NumberExpression(2));
            _symbolTable.SetupGet(x => x["ClassSkill"])
                .Returns(new NumberExpression(3));
            _symbolTable.SetupGet(x => x["Misc"])
                .Returns(new NumberExpression(0));
            _symbolTable.SetupGet(x => x["RangePenalty"])
                .Returns(new NegateExpression(new NumberExpression(4)));
            _symbolTable.SetupGet(x => x["BAB"])
                .Returns(new NumberExpression(4));
            _symbolTable.SetupGet(x => x["Size"])
                .Returns(new NumberExpression(1));
            _symbolTable.SetupGet(x => x["ArmorBonus"])
                .Returns(new NumberExpression(5));
            _symbolTable.SetupGet(x => x["ShieldBonus"])
                .Returns(new NumberExpression(2));
            _symbolTable.SetupGet(x => x["MaxDex"])
                .Returns(new NumberExpression(2));

            var realSymbol = new BuiltinTypeSymbol("real");
            _symbolTable.Setup(x => x.Lookup("max"))
                .Returns(new FunctionSymbol(
                    "max",
                    None.Value,
                    args => args.Max(),
                    new ISymbol[]
                    {
                        new VariableSymbol("a", realSymbol),
                        new VariableSymbol("b", realSymbol)
                    }));
            _symbolTable.Setup(x => x.Lookup("min"))
                .Returns(new FunctionSymbol(
                    "min",
                    None.Value,
                    args => args.Min(),
                    new ISymbol[]
                    {
                        new VariableSymbol("a", realSymbol),
                        new VariableSymbol("b", realSymbol)
                    }));

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
            }
        }

        [TestCaseSource(typeof(NotationTestCaseData))]
        [TestCaseSource(typeof(PFRollsTestCaseData))]
        [TestCaseSource(typeof(FunctionTestCaseData))]
        public float Evaluate_ShouldReturnCorrect(string input)
        {
            var tokenlist = new DiceNotationTokenizer().Tokenize(input);
            var expression = DiceNotationParser.Notation.Parse(tokenlist);

            var context = new DiceNotationContext(expression)
            {
                DiceRoller = _mockRoller.Object,
                SymbolTable = _symbolTable.Object,
            };

            return _interpreter.Interpret(context);
        }
    }
}

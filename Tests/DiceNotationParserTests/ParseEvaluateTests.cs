﻿using Moq;
using NUnit.Framework;
using Superpower;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Wgaffa.DMToolkit;
using Wgaffa.DMToolkit.DiceRollers;
using Wgaffa.DMToolkit.Interpreters;
using Wgaffa.DMToolkit.Native;
using Wgaffa.DMToolkit.Parser;

namespace DiceNotationParserTests
{
    public class ParseEvaluateTests
    {
        private Mock<IDiceRoller> _mockRoller;
        private ScopedSymbolTable _symbolTable;

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
            var stringSymbol = new BuiltinTypeSymbol("string");
            var parameters = new Symbol[] { new VariableSymbol("a", realSymbol), new VariableSymbol("b", realSymbol) };
            _symbolTable.Add(realSymbol);
            _symbolTable.Add(intSymbol);
            _symbolTable.Add(new FunctionSymbol(
                "max",
                realSymbol,
                new Max(),
                parameters));
            _symbolTable.Add(new FunctionSymbol(
                "min",
                realSymbol,
                new Min(),
                parameters));
            _symbolTable.Add(new FunctionSymbol(
                "print",
                realSymbol,
                new Print(),
                new Symbol[] { new VariableSymbol("a", stringSymbol) }));

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
        }

        public class NotationTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("print(1+5);")
                    .Returns(6);
                yield return new TestCaseData("print(1.5 * d6);")
                    .Returns(4.5);
                yield return new TestCaseData("print(6d20-L);")
                    .Returns(49);
                yield return new TestCaseData("print(4d20-H);")
                    .Returns(30);
                yield return new TestCaseData("print(4d20(k3));")
                    .Returns(45);
            }
        }

        public class PFRollsTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("print(d20 + INTMOD + Ranks + ClassSkill + Misc);")
                    .Returns(11)
                    .SetName("Skill check roll");
                yield return new TestCaseData("print(d20 + STRMOD + BAB + Size);")
                    .Returns(10)
                    .SetName("Attackroll melee small");
                yield return new TestCaseData("print(d20 + DEXMOD + BAB + Size + RangePenalty);")
                    .Returns(8)
                    .SetName("Attackroll ranged small");
                yield return new TestCaseData("print(10 + ArmorBonus + ShieldBonus + DEXMOD + Size + Misc);")
                    .Returns(22)
                    .SetName("ArmorBonus");
                yield return new TestCaseData("print(10 + ArmorBonus + ShieldBonus + max(0, min(DEXMOD, MaxDex)) + Size + Misc);")
                    .Returns(20)
                    .SetName("ArmorBonus with MaxDex");
            }
        }

        public class FunctionTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("print(5+max(7, 3+8));")
                    .Returns(16);
                yield return new TestCaseData("int Add5(int a) return a + 5; end print(Add5(10));")
                    .Returns(15);
                yield return new TestCaseData("real Pi() return 3.14; end print(Pi());")
                    .Returns(3.14);
            }
        }

        public class ScopingTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("real foo = 5.5; real Bar(real a) foo = a; end Bar(3.2); print(foo);")
                    .Returns(3.2);
                yield return new TestCaseData("int x = 3; " +
                    "int P() x = x - 1; end " +
                    "int Q() " +
                    "   int y = x; int R() x = x + 1; y = y + x; P(); end " +
                    "R(); P(); end " +
                    "x = 2; Q(); print(x);")
                    .Returns(1);
            }
        }

        public class VariableTestCaseData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new TestCaseData("real foo; foo = 5.2; print(foo);")
                    .Returns(5.2);
                yield return new TestCaseData("int foo, bar; foo = 5; bar = 2d6; print(foo+bar);")
                    .Returns(17);
                yield return new TestCaseData("int foo, bar = 5; print(foo+bar);")
                    .Returns(10);
                yield return new TestCaseData("def Attack = (d20 + 5) * 1.5; print(Attack + 5);")
                    .Returns(17);
                yield return new TestCaseData("def Attack = 1d20 + STRMOD; print(Attack);")
                    .Returns(5);
                yield return new TestCaseData("def Block = 5 + max(4, STRMOD); print(Block);")
                    .Returns(9);
                yield return new TestCaseData("int Max(int a, int b) return 5; end def Parry = 5 + Max(4, STRMOD); print(Parry);")
                    .Returns(10);
                yield return new TestCaseData("int x = 5; int D() def Durdle = 5 + max(4, STRMOD); int P() int max(int a, int b) return 20; end x = Durdle; end P(); end D(); print(x);")
                    .Returns(25);
                yield return new TestCaseData("int x = 5; int D() def Durdle = 5 + STRMOD; int P() STRMOD = 1; x = Durdle; end P(); end D(); print(x);")
                    .Returns(6);
                yield return new TestCaseData("int x = 5; int D() def Durdle = 5 + max(4, STRMOD); int P() int max(int a, int b) return b; end STRMOD = 3; x = Durdle; end P(); end D(); print(x);")
                    .Returns(8);
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
        [TestCaseSource(typeof(ScopingTestCaseData))]
        public double Evaluate_ShouldReturnCorrect(string input)
        {
            var tokenlist = new DiceNotationTokenizer().Tokenize(input);
            var expression = DiceNotationParser.Program.Parse(tokenlist);

            var configuration = new Configuration()
            {
                DiceRoller = _mockRoller.Object,
                SymbolTable = _symbolTable,
            };

            var semantic = new SemanticAnalyzer(configuration);
            var ast = semantic.Analyze(expression);

            using StringWriter sw = new StringWriter();
            Console.SetOut(sw);

            var interpreter = new DiceNotationInterpreter(configuration);
            ast.OnError(e => throw new Exception($"{e.Count} errors during semantic analyze"))
                .OnSuccess(expr =>
                interpreter.Interpret(
                    expr,
                    new VariableSetup()));

            return Convert.ToDouble(sw.ToString());
        }
    }
}
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
using Wgaffa.DMToolkit.Interpreters;
using Wgaffa.DMToolkit.Parser;

namespace DiceNotationParserTests
{
    public class ParseEvaluateTests
    {
        private Mock<IDiceRoller> _mockRoller;
        private DiceNotationInterpreter _interpreter;

        [SetUp]
        public void SetUp()
        {
            _mockRoller = new Mock<IDiceRoller>();

            _mockRoller.Setup(d => d.RollDice(It.IsAny<Dice>()))
                .Returns(3);

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
            }
        }

        [NUnit.Framework.TestCaseSource(typeof(NotationTestCaseData))]
        public float Evaluate_ShouldReturnCorrect(string input)
        {
            var tokenlist = new DiceNotationTokenizer().Tokenize(input);
            var expression = DiceNotationParser.Notation.Parse(tokenlist);


            var context = new DiceNotationContext(expression)
            {
                DiceRoller = _mockRoller.Object
            };


            return _interpreter.Interpret(context);
        }
    }
}

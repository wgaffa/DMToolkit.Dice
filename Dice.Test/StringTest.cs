using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceTest
{
    [TestClass]
    public class StringTest
    {
        [TestMethod]
        public void DiceStringOneRoll()
        {
            string diceExpression = "d12";

            string[] actual = diceExpression.Split('d');
            
            CollectionAssert.AreEqual(new string[] { "", "12" }, actual);
        }

        [TestMethod]
        public void DiceStringMultiRoll()
        {
            string diceExpression = "5d12";

            string[] actual = diceExpression.Split('d');
            CollectionAssert.AreEqual(new string[] { "5", "12" }, actual);
        }

        [TestMethod]
        public void DiceStringMultiRollUpperCase()
        {
            string diceExpression = "7D4";

            string[] actual = diceExpression.Split(new char[]{ 'd', 'D'});
            CollectionAssert.AreEqual(new string[] { "7", "4" }, actual);
        }
    }
}

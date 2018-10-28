using DMTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceTest
{
    class MockRandomGenerator : IRandomGenerator
    {
        private readonly int _constant;

        public MockRandomGenerator(int constant = 5)
        {
            _constant = constant;
        }

        public int Generate(int min, int max)
        {
            return _constant;
        }
    }
}

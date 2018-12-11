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

    class MockListGenerator : IRandomGenerator
    {
        private readonly List<int> _numbers;
        private int _currentIndex = 0;

        public MockListGenerator(IEnumerable<int> numbers)
        {
            _numbers = numbers.ToList();
        }

        public int Generate(int min, int max)
        {
            int number = _numbers[_currentIndex];
            _currentIndex = _currentIndex + 1 == _numbers.Count() ? 0 : _currentIndex + 1;

            return number;
        }
    }
}

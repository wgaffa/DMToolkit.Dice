using DMTools.Die;
using DMTools.Die.Rollers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceTest
{
    class MockRandomGenerator : IDiceRoller
    {
        private readonly int _constant;

        public MockRandomGenerator(int constant = 5)
        {
            _constant = constant;
        }

        public int RollDice(int sides)
        {
            return _constant;
        }
    }

    class MockListGenerator : IDiceRoller
    {
        private readonly List<int> _numbers;
        private int _currentIndex = 0;

        public MockListGenerator(IEnumerable<int> numbers)
        {
            _numbers = numbers.ToList();
        }

        public int RollDice(int sides)
        {
            int number = _numbers[_currentIndex];
            _currentIndex = _currentIndex + 1 == _numbers.Count() ? 0 : _currentIndex + 1;

            return number;
        }
    }
}

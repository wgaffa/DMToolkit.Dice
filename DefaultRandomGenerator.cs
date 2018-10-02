using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice
{
    public class DefaultRandomGenerator : IRandomGenerator
    {
        public DefaultRandomGenerator()
        {
            _generator = new Random();
        }

        public DefaultRandomGenerator(Random generator)
        {
            _generator = generator;
        }

        public int Generate(int min, int max)
        {
            return _generator.Next(min, max);
        }

        private readonly Random _generator = new Random();
    }
}

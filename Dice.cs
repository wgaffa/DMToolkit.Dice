using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice
{
    public class Dice
    {
        public Dice(int sides)
        {
            Sides = sides;
        }

        public Dice(int sides, IRandomGenerator randomGen)
        {
            Sides = sides;
            randomGenerator = randomGen;
        }

        public int Roll()
        {
            return randomGenerator.Generate(1, Sides);
        }

        public int Sides { get; private set; }
        private readonly IRandomGenerator randomGenerator = new DefaultRandomGenerator();
    }
}

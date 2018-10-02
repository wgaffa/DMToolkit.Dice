using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice
{
    public interface IRandomGenerator
    {
        int Generate(int min, int max);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Core
{
    /// <summary>
    /// Interface for generating a random number
    /// </summary>
    public interface IRandomGenerator
    {
        /// <summary>
        /// Generate a random number
        /// </summary>
        /// <param name="min">Lower bound of random number</param>
        /// <param name="max">Upper bound of random number</param>
        /// <returns>Random number between [min, max)</returns>
        int Generate(int min, int max);
    }
}

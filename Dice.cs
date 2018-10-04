namespace DMTools.Dice
{
    /// <summary>
    /// Represent a physical die with a set number of sides.
    /// </summary>
    public class Dice
    {
        /// <summary>
        /// Constructor to create a die with DefaultRandomGenerator for randomizing
        /// </summary>
        /// <param name="sides">Number of sides of die</param>
        public Dice(int sides = 6)
        {
            Sides = sides;
        }

        /// <summary>
        /// Constructor to create a die
        /// </summary>
        /// <param name="sides">Number of sides of die</param>
        /// <param name="randomGen">Random generator to generate random numbers from</param>
        public Dice(int sides, IRandomGenerator randomGen)
        {
            Sides = sides;
            randomGenerator = randomGen;
        }

        /// <summary>
        /// Rolls the die
        /// </summary>
        /// <returns>Random number between 1 and Sides</returns>
        public int Roll()
        {
            return randomGenerator.Generate(1, Sides + 1);
        }

        /// <summary>
        /// Number of sides of the die
        /// </summary>
        public int Sides { get; private set; }

        /// <summary>
        /// Random number generator when rolling die
        /// </summary>
        private readonly IRandomGenerator randomGenerator = new DefaultRandomGenerator();
    }
}

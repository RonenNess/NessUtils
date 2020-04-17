using System;


namespace Ness.Utils
{
    /// <summary>
    /// A very fast and simple random numbers generator with seed, gueranteed to remain the same no matter what C# or .net version we run.
    /// This is for things that must absolutely always generate the same results.
    /// 
    /// Note: System.Random with provided seed is usually enough, I used this class when I needed the same randomness in C# and a corresponding JavaScript code.
    /// </summary>
    public class ConstantSeededRandom
    {
        // current seed
        int _seed = 1;

        /// <summary>
        /// Create the seeded random.
        /// </summary>
        public ConstantSeededRandom(int seed)
        {
            _seed = seed;
        }

        /// <summary>
        /// Get double between 0 and 1.
        /// </summary>
        public double NextDouble()
        {
            var x = Math.Sin(_seed++) * 10000;
            return x - Math.Floor(x);
        }

        /// <summary>
        /// Get random int with max value.
        /// </summary>
        public int Next(int max)
        {
            return (int)Math.Floor(NextDouble() * max);
        }
    }
}

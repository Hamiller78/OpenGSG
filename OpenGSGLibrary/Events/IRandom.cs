namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Abstraction for random number generation.
    /// Allows deterministic testing by injecting predictable implementations.
    /// </summary>
    public interface IRandom
    {
        /// <summary>
        /// Returns a random double between 0.0 and 1.0.
        /// </summary>
        double NextDouble();
    }

    /// <summary>
    /// Production implementation using System.Random.
    /// </summary>
    public class SystemRandom : IRandom
    {
        private readonly Random _random = new Random();

        public double NextDouble()
        {
            return _random.NextDouble();
        }
    }

    /// <summary>
    /// Test implementation with deterministic, predictable values.
    /// </summary>
    public class DeterministicRandom : IRandom
    {
        private readonly double[] _values;
        private int _index = 0;

        /// <summary>
        /// Creates a deterministic random that returns the specified sequence of values.
        /// </summary>
        /// <param name="values">Sequence of values to return (cycles when exhausted)</param>
        public DeterministicRandom(params double[] values)
        {
            _values = values;
        }

        public double NextDouble()
        {
            var value = _values[_index];
            _index = (_index + 1) % _values.Length;
            return value;
        }
    }

    /// <summary>
    /// Test implementation using a seeded Random for reproducible sequences.
    /// </summary>
    public class SeededRandom : IRandom
    {
        private readonly Random _random;

        public SeededRandom(int seed)
        {
            _random = new Random(seed);
        }

        public double NextDouble()
        {
            return _random.NextDouble();
        }
    }
}

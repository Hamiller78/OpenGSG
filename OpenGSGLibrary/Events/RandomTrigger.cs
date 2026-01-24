using System.Diagnostics;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Trigger that generates a random number and compares it to a threshold.
    /// Syntax: random < 0.7 (fires 70% of the time)
    /// </summary>
    public class RandomTrigger : IEventTrigger
    {
        public double Threshold { get; set; }
        public string Operator { get; set; } = "<"; // <, >, <=, >=, ==

        /// <summary>
        /// Random number generator (injectable for testing)
        /// </summary>
        public IRandom Random { get; set; } = new SystemRandom();

        public bool Evaluate(object context)
        {
            double roll = Random.NextDouble(); // 0.0 to 1.0

            return Operator switch
            {
                "<" => roll < Threshold,
                "<=" => roll <= Threshold,
                ">" => roll > Threshold,
                ">=" => roll >= Threshold,
                "==" or "=" => Math.Abs(roll - Threshold) < 0.0001,
                _ => false,
            };
        }
    }
}

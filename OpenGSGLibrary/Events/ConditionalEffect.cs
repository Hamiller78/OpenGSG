using System.Diagnostics;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Conditional effect that executes different effects based on a condition.
    /// Syntax: if = { limit = { ... } effects... } else = { effects... }
    /// </summary>
    public class ConditionalEffect : IEventEffect
    {
        /// <summary>
        /// Condition triggers (from limit block)
        /// </summary>
        public List<IEventTrigger> Condition { get; set; } = new List<IEventTrigger>();

        /// <summary>
        /// Effects to execute if condition is true
        /// </summary>
        public List<IEventEffect> ThenEffects { get; set; } = new List<IEventEffect>();

        /// <summary>
        /// Effects to execute if condition is false
        /// </summary>
        public List<IEventEffect> ElseEffects { get; set; } = new List<IEventEffect>();

        public void Execute(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return;

            // Evaluate all conditions (AND logic)
            bool conditionMet = true;
            foreach (var trigger in Condition)
            {
                if (!trigger.Evaluate(evalContext))
                {
                    conditionMet = false;
                    break;
                }
            }

            // Execute appropriate effects
            var effectsToExecute = conditionMet ? ThenEffects : ElseEffects;
            foreach (var effect in effectsToExecute)
            {
                effect.Execute(context);
            }
        }
    }
}

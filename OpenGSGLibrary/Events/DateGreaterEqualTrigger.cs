namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Checks if the current game date is >= the specified date.
    /// </summary>
    public class DateGreaterEqualTrigger : IEventTrigger
    {
        public DateTime Date { get; set; }

        public bool Evaluate(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return false;

            return evalContext.CurrentDate >= Date;
        }
    }
}

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Represents a news event that is shown globally but can be filtered by recipients.
    /// News events evaluate triggers globally once, then filter which countries receive them.
    /// </summary>
    public class NewsEvent : GameEvent
    {
        /// <summary>
        /// List of recipient conditions that determine which countries receive this news event.
        /// If empty, all countries receive the event.
        /// </summary>
        public List<IEventTrigger> Recipients { get; set; } = new List<IEventTrigger>();

        /// <summary>
        /// Evaluates whether a specific country should receive this news event.
        /// </summary>
        /// <param name="context">Evaluation context with country information</param>
        /// <returns>True if country should receive the event, false otherwise</returns>
        public bool EvaluateRecipients(EventEvaluationContext context)
        {
            // If no recipients specified, all countries receive the event
            if (Recipients.Count == 0)
                return true;

            // Check if this country matches recipient conditions
            foreach (var recipient in Recipients)
            {
                if (recipient.Evaluate(context))
                    return true;
            }

            return false;
        }
    }
}

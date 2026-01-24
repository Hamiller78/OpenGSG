namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Represents an event scheduled to fire in the future.
    /// </summary>
    public class ScheduledEvent
    {
        /// <summary>
        /// ID of the event to fire.
        /// </summary>
        public string EventId { get; set; } = string.Empty;

        /// <summary>
        /// Country tag for which the event should fire.
        /// </summary>
        public string TargetCountryTag { get; set; } = string.Empty;

        /// <summary>
        /// Date when the event should fire.
        /// </summary>
        public DateTime FireDate { get; set; }

        /// <summary>
        /// Is this a news event?
        /// </summary>
        public bool IsNewsEvent { get; set; }
    }
}

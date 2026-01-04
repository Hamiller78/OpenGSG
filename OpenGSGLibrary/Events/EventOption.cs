namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Represents a choice/option within a game event.
    /// </summary>
    public class EventOption
    {
        /// <summary>
        /// Localization key for the option name/button text.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// List of effects to execute when this option is chosen.
        /// </summary>
        public List<IEventEffect> Effects { get; set; } = new List<IEventEffect>();

        /// <summary>
        /// List of effects that are hidden from the player but still execute.
        /// </summary>
        public List<IEventEffect> HiddenEffects { get; set; } = new List<IEventEffect>();

        /// <summary>
        /// Executes all effects (visible and hidden) associated with this option.
        /// </summary>
        /// <param name="context">Game state context.</param>
        public void Execute(object context)
        {
            foreach (var effect in Effects)
            {
                effect.Execute(context);
            }

            foreach (var effect in HiddenEffects)
            {
                effect.Execute(context);
            }
        }
    }
}

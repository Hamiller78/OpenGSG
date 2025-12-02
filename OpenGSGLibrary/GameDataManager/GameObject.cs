namespace OpenGSGLibrary.GameDataManager
{
    /// <summary>
    /// Base class for game object classes like provinces, countries, armies, etc...
    /// Always stores its parser data
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// Sets file name and parser data of the object.
        /// This doesn't work in the constructor with generic types, hence we use an extra method.
        /// </summary>
        /// <param name="fileName">Name of the source file for the object's data without extension.</param>
        /// <param name="parsedData">Structure with the parsed data.</param>
        public virtual void SetData(string fileName, ILookup<string, object> parsedData)
        {
            fileName_ = fileName;
            parsedData_ = parsedData;
        }

        /// <summary>
        /// Handler for tick done events by TickHandler.
        /// Should be reimplemented for all game elements which require an update with each game tick.
        /// E.g. provinces can have population growth, armies replenish their morale, etc.
        /// </summary>
        /// <param name="sender">sender object of the event</param>
        /// <param name="e">EventArgs passed by the tick handler (may be TickEventArgs in some implementations)</param>
        // Handler for tick done events. Uses System.EventArgs to avoid cross-project type coupling.
        public virtual void OnTickDone(object sender, EventArgs e) { }

        private string fileName_ = string.Empty;
        private ILookup<string, object> parsedData_ = null!;
    }
}

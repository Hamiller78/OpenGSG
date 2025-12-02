using OpenGSGLibrary.GameDataManager;

namespace OpenGSGLibrary.Military
{
    /// <summary>
    /// Represents a military division with a name, type and size.
    /// Instances are populated from parsed game data using <see cref="GameObject.SetData"/>.
    /// </summary>
    public class Division : GameObject
    {
        private string name_ = string.Empty;
        private string type_ = string.Empty;
        private int size_ = 0;
        private string owner_ = string.Empty;

        /// <summary>
        /// Populate division fields from parsed data.
        /// Expected keys: "name", "type", "size".
        /// </summary>
        /// <param name="fileName">Source file name used as identifier (unused in this implementation).</param>
        /// <param name="parsedData">Parser output as an <see cref="ILookup{TKey, TValue}"/>.</param>
        public override void SetData(string fileName, ILookup<string, object> parsedData)
        {
            name_ = parsedData.Contains("name") ? parsedData["name"].Single()?.ToString() ?? string.Empty : string.Empty;
            type_ = parsedData.Contains("type") ? parsedData["type"].Single()?.ToString() ?? string.Empty : string.Empty;
            size_ = parsedData.Contains("size") ? Convert.ToInt32(parsedData["size"].Single()) : 0;
        }

        /// <summary>
        /// Sets the owner tag for the division.
        /// </summary>
        /// <param name="tag">Nation tag owning this division.</param>
        public void SetOwner(string tag) => owner_ = tag;
    }
}

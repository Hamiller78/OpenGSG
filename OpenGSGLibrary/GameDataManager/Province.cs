namespace OpenGSGLibrary.GameDataManager
{
    /// <summary>
    /// Base class for provinces. Keeps model as a simple POCO — notifications live on ViewModels.
    /// </summary>
    public class Province : GameObject
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// List of country tags that consider this province their core territory.
        /// Used for territorial claims, nationalism, and liberation mechanics.
        /// </summary>
        public List<string> Cores { get; set; } = new List<string>();

        public override void SetData(string fileName, ILookup<string, object> parsedData)
        {
            base.SetData(fileName, parsedData);
            var fileNameParts = GameObjectFactory.ExtractFromFilename(fileName);
            Id = Convert.ToInt32(fileNameParts[0]);
            Name = fileNameParts.Length > 1 ? fileNameParts[1] : string.Empty;

            Controller = parsedData.Contains("controller")
                ? parsedData["controller"].Single()?.ToString() ?? string.Empty
                : string.Empty;
            Owner = parsedData.Contains("owner")
                ? parsedData["owner"].Single()?.ToString() ?? string.Empty
                : string.Empty;

            // Parse cores (using add_core keyword)
            if (parsedData.Contains("add_core"))
            {
                foreach (var coreTag in parsedData["add_core"])
                {
                    var tag = coreTag?.ToString();
                    if (!string.IsNullOrEmpty(tag) && !Cores.Contains(tag))
                    {
                        Cores.Add(tag);
                    }
                }
            }
        }
    }
}

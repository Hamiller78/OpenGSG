using System;
using System.Collections.Generic;
using OpenGSGLibrary.Military;

namespace OpenGSGLibrary.GameDataManager
{
    /// <summary>
    /// Base class for provinces. Handles the most basic province properties.
    /// </summary>
    public class Province : GameObject
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public List<Division>? Units { get; } = [];

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
        }
    }
}

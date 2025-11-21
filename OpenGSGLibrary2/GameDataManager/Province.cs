using System;
using System.Collections.Generic;

namespace WorldData
{
    /// <summary>
    /// Base class for provinces. Handles the most basic province properties.
    /// </summary>
    public class Province : GameObject
    {
        private int id_;
        private string name_ = string.Empty;
        private string controller_ = string.Empty;
        private string owner_ = string.Empty;
        private List<Military.Division>? units_ = null;

        public int GetId() => id_;
        public string GetName() => name_;
        public string GetController() => controller_;
        public string GetOwner() => owner_;

        public override void SetData(string fileName, ILookup<string, object> parsedData)
        {
            base.SetData(fileName, parsedData);
            var fileNameParts = GameObjectFactory.ExtractFromFilename(fileName);
            id_ = Convert.ToInt32(fileNameParts[0]);
            name_ = fileNameParts.Length > 1 ? fileNameParts[1] : string.Empty;

            controller_ = parsedData.Contains("controller") ? parsedData["controller"].Single()?.ToString() ?? string.Empty : string.Empty;
            owner_ = parsedData.Contains("owner") ? parsedData["owner"].Single()?.ToString() ?? string.Empty : string.Empty;
        }
    }
}

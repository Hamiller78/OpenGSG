using System.Collections.Generic;

namespace Military
{
    public class NationMilitary : WorldData.GameObject
    {
        private string owner_ = string.Empty;
        private List<Army> armies_ = new List<Army>();

        public override void SetData(string fileName, ILookup<string, object> parsedData)
        {
            base.SetData(fileName, parsedData);
            owner_ = parsedData.Contains("tag") ? parsedData["tag"].Single()?.ToString() ?? string.Empty : string.Empty;
            armies_ = WorldData.GameObjectFactory.ListFromLookup<Army>(parsedData, "army");
            foreach (var army in armies_) army.SetOwner(owner_);
        }

        public List<Army> GetArmiesList() => armies_;
    }
}

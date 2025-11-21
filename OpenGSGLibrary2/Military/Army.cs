using System;
using System.Collections.Generic;

namespace Military
{
    public class Army : WorldData.GameObject
    {
        public List<Division>? units { get; set; }

        private string owner_ = string.Empty;
        private string name_ = string.Empty;
        private int location_ = 0;
        private List<Division>? divisions_ = null;

        public override void SetData(string fileName, ILookup<string, object> parsedData)
        {
            name_ = parsedData.Contains("name") ? parsedData["name"].Single()?.ToString() ?? string.Empty : string.Empty;
            location_ = parsedData.Contains("location") ? Convert.ToInt32(parsedData["location"].Single()) : 0;
            divisions_ = WorldData.GameObjectFactory.ListFromLookup<Division>(parsedData, "division");
        }

        public void SetOwner(string tag)
        {
            owner_ = tag;
            if (divisions_ != null)
            {
                foreach (var division in divisions_) division.SetOwner(tag);
            }
        }

        public int GetLocation() => location_;
        public void SetLocation(int provinceId) => location_ = provinceId;

        public override string ToString() => name_;
    }
}

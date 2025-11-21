namespace Military
{
    public class Division : WorldData.GameObject
    {
        private string name_ = string.Empty;
        private string type_ = string.Empty;
        private int size_ = 0;
        private string owner_ = string.Empty;

        public override void SetData(string fileName, ILookup<string, object> parsedData)
        {
            name_ = parsedData.Contains("name") ? parsedData["name"].Single()?.ToString() ?? string.Empty : string.Empty;
            type_ = parsedData.Contains("type") ? parsedData["type"].Single()?.ToString() ?? string.Empty : string.Empty;
            size_ = parsedData.Contains("size") ? Convert.ToInt32(parsedData["size"].Single()) : 0;
        }

        public void SetOwner(string tag) => owner_ = tag;
    }
}

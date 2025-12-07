using CommunityToolkit.Mvvm.ComponentModel;
using OpenGSGLibrary.Military;

namespace OpenGSGLibrary.GameDataManager
{
    /// <summary>
    /// Base class for provinces. Handles the most basic province properties.
    /// Converted to an MVVM model using CommunityToolkit.Mvvm source generators so
    /// property changes raise notifications automatically.
    /// </summary>
    [INotifyPropertyChanged]
    public partial class Province : GameObject
    {
        // Explicit backing fields for read-only-from-outside properties
        private int id;
        public int Id
        {
            get => id;
            private set => SetProperty(ref id, value);
        }

        private string name = string.Empty;
        public string Name
        {
            get => name;
            private set => SetProperty(ref name, value);
        }

        // Use source-generator for the mutable properties (public setters)
        [ObservableProperty]
        private string controller = string.Empty;

        [ObservableProperty]
        private string owner = string.Empty;

        [ObservableProperty]
        private List<Division>? units = new();

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

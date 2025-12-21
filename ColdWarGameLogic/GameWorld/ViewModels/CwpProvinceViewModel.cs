using CommunityToolkit.Mvvm.ComponentModel;

namespace ColdWarGameLogic.GameWorld.ViewModels;

/// <summary>
/// ViewModel for <see cref="CwpProvince"/>. Implements INotifyPropertyChanged and exposes Refresh().
/// Models are plain POCOs; call Refresh() whenever the model is updated externally.
/// Uses CommunityToolkit.Mvvm source generators to reduce boilerplate.
/// </summary>
public sealed partial class CwpProvinceViewModel : ObservableObject, IDisposable
{
    public CwpProvince Model { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Production))]
    private long population;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Production))]
    private long industrialization;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Production))]
    private long education;

    [ObservableProperty]
    private string terrain = string.Empty;

    [ObservableProperty]
    private string owner = string.Empty;

    [ObservableProperty]
    private string controller = string.Empty;

    public CwpProvinceViewModel(CwpProvince model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Refresh();
    }

    /// <summary>
    /// Pulls current state from the model and raises notifications for all view properties.
    /// Call this after the model is updated by the game loop, loader, etc.
    /// </summary>
    public void Refresh()
    {
        Population = Model.Population;
        Industrialization = Model.Industrialization;
        Education = Model.Education;
        Terrain = Model.Terrain ?? string.Empty;
        Owner = Model.Owner ?? string.Empty;
        Controller = Model.Controller ?? string.Empty;

        OnPropertyChanged(nameof(Production));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Id));
    }

    // Read-only passthroughs
    public int Id => Model.Id;
    public string Name => Model.Name;

    // Computed
    public long Production => Model.Production;

    partial void OnPopulationChanged(long value) => Model.Population = value;

    partial void OnIndustrializationChanged(long value) => Model.Industrialization = value;

    partial void OnEducationChanged(long value) => Model.Education = value;

    partial void OnTerrainChanged(string value) => Model.Terrain = value;

    partial void OnOwnerChanged(string value) => Model.Owner = value;

    partial void OnControllerChanged(string value) => Model.Controller = value;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

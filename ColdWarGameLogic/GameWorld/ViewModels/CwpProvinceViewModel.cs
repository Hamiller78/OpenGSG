using System;
using ColdWarGameLogic.GameWorld;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ColdWarGameLogic.GameWorld.ViewModels;

/// <summary>
/// ViewModel for <see cref="CwpProvince"/>. Implements INotifyPropertyChanged and exposes Refresh().
/// Models are plain POCOs; call Refresh() whenever the model is updated externally.
/// </summary>
public sealed class CwpProvinceViewModel : ObservableObject, IDisposable
{
    public CwpProvince Model { get; }

    private long population;
    private long industrialization;
    private long education;
    private string terrain = string.Empty;
    private string owner = string.Empty;
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
        population = Model.Population;
        industrialization = Model.Industrialization;
        education = Model.Education;
        terrain = Model.Terrain ?? string.Empty;
        owner = Model.Owner ?? string.Empty;
        controller = Model.Controller ?? string.Empty;

        OnPropertyChanged(nameof(Population));
        OnPropertyChanged(nameof(Industrialization));
        OnPropertyChanged(nameof(Education));
        OnPropertyChanged(nameof(Terrain));
        OnPropertyChanged(nameof(Owner));
        OnPropertyChanged(nameof(Controller));
        OnPropertyChanged(nameof(Production));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Id));
    }

    // Passthrough properties that update model and notify view.
    public long Population
    {
        get => population;
        set
        {
            if (SetProperty(ref population, value))
            {
                Model.Population = value;
                OnPropertyChanged(nameof(Production));
            }
        }
    }

    public long Industrialization
    {
        get => industrialization;
        set
        {
            if (SetProperty(ref industrialization, value))
            {
                Model.Industrialization = value;
                OnPropertyChanged(nameof(Production));
            }
        }
    }

    public long Education
    {
        get => education;
        set
        {
            if (SetProperty(ref education, value))
            {
                Model.Education = value;
                OnPropertyChanged(nameof(Production));
            }
        }
    }

    public string Terrain
    {
        get => terrain;
        set
        {
            if (SetProperty(ref terrain, value ?? string.Empty))
            {
                Model.Terrain = terrain;
            }
        }
    }

    public string Owner
    {
        get => owner;
        set
        {
            if (SetProperty(ref owner, value ?? string.Empty))
            {
                Model.Owner = owner;
            }
        }
    }

    public string Controller
    {
        get => controller;
        set
        {
            if (SetProperty(ref controller, value ?? string.Empty))
            {
                Model.Controller = controller;
            }
        }
    }

    // Read-only passthroughs
    public int Id => Model.Id;
    public string Name => Model.Name;

    // Computed
    public long Production => Model.Production;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

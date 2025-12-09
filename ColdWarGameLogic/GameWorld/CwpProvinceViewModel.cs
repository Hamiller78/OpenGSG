using System;
using System.ComponentModel;
using ColdWarGameLogic.GameWorld;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ColdWarPrototype.ViewModels;

/// <summary>
/// ViewModel wrapper for <see cref="CwpProvince"/>.
/// Forwards model property changes and exposes simple passthrough properties for binding.
/// Subscribes to the model's <see cref="INotifyPropertyChanged"/> and re-raises changes on the ViewModel.
/// </summary>
public sealed class CwpProvinceViewModel : ObservableObject, IDisposable
{
    public CwpProvince Model { get; }

    public CwpProvinceViewModel(CwpProvince model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Model.PropertyChanged += Model_PropertyChanged;
    }

    private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e?.PropertyName))
            return;

        // Forward model property changes to the View (bindings target the ViewModel)
        OnPropertyChanged(e.PropertyName);

        // Production is computed from Population, Industrialization and Education.
        // When any of them change, also notify that Production changed.
        if (
            e.PropertyName
            is nameof(CwpProvince.Population)
                or nameof(CwpProvince.Industrialization)
                or nameof(CwpProvince.Education)
        )
        {
            OnPropertyChanged(nameof(Production));
        }
    }

    // Passthrough properties — set model; model will raise change which is forwarded above.
    public long Population
    {
        get => Model.Population;
        set
        {
            if (Model.Population == value)
                return;
            Model.Population = value;
        }
    }

    public long Industrialization
    {
        get => Model.Industrialization;
        set
        {
            if (Model.Industrialization == value)
                return;
            Model.Industrialization = value;
        }
    }

    public long Education
    {
        get => Model.Education;
        set
        {
            if (Model.Education == value)
                return;
            Model.Education = value;
        }
    }

    public string Terrain
    {
        get => Model.Terrain;
        set
        {
            if (Model.Terrain == value)
                return;
            Model.Terrain = value;
        }
    }

    // Computed read-only property
    public long Production => Model.Production;

    public void Dispose()
    {
        Model.PropertyChanged -= Model_PropertyChanged;
        GC.SuppressFinalize(this);
    }
}

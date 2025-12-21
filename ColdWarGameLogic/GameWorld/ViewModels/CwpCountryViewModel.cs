using System;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ColdWarGameLogic.GameWorld.ViewModels;

/// <summary>
/// ViewModel for <see cref="CwpCountry"/>. Implements INotifyPropertyChanged and exposes Refresh().
/// Models are plain POCOs; call Refresh() whenever the model is updated externally.
/// Uses CommunityToolkit.Mvvm source generators to reduce boilerplate.
/// </summary>
public sealed partial class CwpCountryViewModel : ObservableObject, IDisposable
{
    public CwpCountry Model { get; }

    // Base Country backing fields (read-only from view, updated via Refresh)
    [ObservableProperty]
    private string tag = string.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private Tuple<byte, byte, byte>? color;

    [ObservableProperty]
    private Bitmap? flag;

    // CwpCountry backing fields (read-write)
    [ObservableProperty]
    private string fullName = string.Empty;

    [ObservableProperty]
    private string government = string.Empty;

    [ObservableProperty]
    private string allegiance = string.Empty;

    [ObservableProperty]
    private string leader = string.Empty;

    public CwpCountryViewModel(CwpCountry model)
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
        Tag = Model.Tag ?? string.Empty;
        Name = Model.Name ?? string.Empty;
        Color = Model.Color;
        Flag = Model.Flag;

        FullName = Model.FullName ?? string.Empty;
        Government = Model.Government ?? string.Empty;
        Allegiance = Model.Allegiance ?? string.Empty;
        Leader = Model.Leader ?? string.Empty;
    }

    // Update model when CWP-specific properties change
    partial void OnFullNameChanged(string value) => Model.FullName = value;

    partial void OnGovernmentChanged(string value) => Model.Government = value;

    partial void OnAllegianceChanged(string value) => Model.Allegiance = value;

    partial void OnLeaderChanged(string value) => Model.Leader = value;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

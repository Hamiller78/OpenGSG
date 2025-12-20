using System;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ColdWarGameLogic.GameWorld.ViewModels;

/// <summary>
/// ViewModel for <see cref="CwpCountry"/>. Implements INotifyPropertyChanged and exposes Refresh().
/// Models are plain POCOs; call Refresh() whenever the model is updated externally.
/// </summary>
public sealed class CwpCountryViewModel : ObservableObject, IDisposable
{
    public CwpCountry Model { get; }

    // Base Country backing fields
    private string tag = string.Empty;
    private string name = string.Empty;
    private Tuple<byte, byte, byte>? color;
    private Bitmap? flag;

    // CwpCountry backing fields
    private string fullName = string.Empty;
    private string government = string.Empty;
    private string allegiance = string.Empty;
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
        // Base Country properties
        tag = Model.Tag ?? string.Empty;
        name = Model.Name ?? string.Empty;
        color = Model.Color;
        flag = Model.Flag;

        // CwpCountry properties
        fullName = Model.FullName ?? string.Empty;
        government = Model.Government ?? string.Empty;
        allegiance = Model.Allegiance ?? string.Empty;
        leader = Model.Leader ?? string.Empty;

        OnPropertyChanged(nameof(Tag));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Color));
        OnPropertyChanged(nameof(Flag));
        OnPropertyChanged(nameof(FullName));
        OnPropertyChanged(nameof(Government));
        OnPropertyChanged(nameof(Allegiance));
        OnPropertyChanged(nameof(Leader));
    }

    // Read-only passthroughs for base Country properties
    public string Tag => tag;
    public string Name => name;
    public Tuple<byte, byte, byte>? Color => color;
    public Bitmap? Flag => flag;

    // Passthrough properties that update model and notify view (CwpCountry properties)
    public string FullName
    {
        get => fullName;
        set
        {
            if (SetProperty(ref fullName, value ?? string.Empty))
            {
                Model.FullName = fullName;
            }
        }
    }

    public string Government
    {
        get => government;
        set
        {
            if (SetProperty(ref government, value ?? string.Empty))
            {
                Model.Government = government;
            }
        }
    }

    public string Allegiance
    {
        get => allegiance;
        set
        {
            if (SetProperty(ref allegiance, value ?? string.Empty))
            {
                Model.Allegiance = allegiance;
            }
        }
    }

    public string Leader
    {
        get => leader;
        set
        {
            if (SetProperty(ref leader, value ?? string.Empty))
            {
                Model.Leader = leader;
            }
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

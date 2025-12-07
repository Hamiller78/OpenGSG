using CommunityToolkit.Mvvm.ComponentModel;
using OpenGSGLibrary.GameDataManager;

namespace ColdWarGameLogic.GameWorld;

/// <summary>
/// Province class for cold war game. Adds specialised properties to base province class.
/// Now raises change notifications via CommunityToolkit.Mvvm source generators.
/// </summary>
public partial class CwpProvince : Province
{
    [ObservableProperty]
    private long population = 0;

    [ObservableProperty]
    private long industrialization = 0;

    [ObservableProperty]
    private long education = 0;

    [ObservableProperty]
    private string terrain = string.Empty;

    public long Production => GetProduction();

    public CwpProvince()
        : base() { }

    /// <summary>
    /// Sets the province properties from the parsed data.
    /// </summary>
    /// <param name="fileName">Name of the source file of province object.</param>
    /// <param name="parsedData">Object with the parsed data from that file.</param>
    public override void SetData(string fileName, ILookup<string, object> parsedData)
    {
        base.SetData(fileName, parsedData);

        Population = ToLong(SingleOrDefault(parsedData, "population"));
        Industrialization = ToLong(SingleOrDefault(parsedData, "industrialization"));
        Education = ToLong(SingleOrDefault(parsedData, "education"));
        Terrain = ToStringSafe(SingleOrDefault(parsedData, "terrain"));
    }

    /// <summary>
    /// Handler for game ticks.
    /// In this game, a tick represents a day.
    /// </summary>
    /// <param name="sender">sender TickHandler</param>
    /// <param name="e">TickEventArgs containing current game time</param>
    public override void OnTickDone(object sender, EventArgs e)
    {
        UpdateDaily();
    }

    private void UpdateDaily()
    {
        // Change in population number, small daily growth (preserve integer storage)
        Population = Convert.ToInt64(Math.Round(Population * 1.00003));
        // Change in industrialization
        // Change of education
    }

    public long GetProduction()
    {
        // Use floating arithmetic and convert to long like the original VB behaviour
        return Convert.ToInt64(Population * Industrialization / 100.0 * Education / 100.0);
    }

    // -- Helpers to safely extract single values from ILookup --
    private static object SingleOrDefault(ILookup<string, object> lookup, string key)
    {
        if (lookup == null)
            return null;
        return lookup.Contains(key) ? lookup[key].SingleOrDefault() : null;
    }

    private static long ToLong(object o)
    {
        if (o == null)
            return 0;
        try
        {
            return Convert.ToInt64(o);
        }
        catch
        {
            return 0;
        }
    }

    private static string ToStringSafe(object o) => o?.ToString() ?? string.Empty;
}

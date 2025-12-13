using OpenGSGLibrary.GameDataManager;

namespace ColdWarGameLogic.GameWorld;

/// <summary>
/// Cold War specific province model. Kept as a POCO — ViewModel provides notifications and Refresh().
/// </summary>
public class CwpProvince : Province
{
    public long Population { get; set; } = 0;
    public long Industrialization { get; set; } = 0;
    public long Education { get; set; } = 0;
    public string Terrain { get; set; } = string.Empty;

    public long Production => GetProduction();

    public CwpProvince()
        : base() { }

    public override void SetData(string fileName, ILookup<string, object> parsedData)
    {
        base.SetData(fileName, parsedData);

        Population = ToLong(SingleOrDefault(parsedData, "population"));
        Industrialization = ToLong(SingleOrDefault(parsedData, "industrialization"));
        Education = ToLong(SingleOrDefault(parsedData, "education"));
        Terrain = ToStringSafe(SingleOrDefault(parsedData, "terrain"));
    }

    public override void OnTickDone(object sender, EventArgs e)
    {
        UpdateDaily();
    }

    private void UpdateDaily()
    {
        Population = Convert.ToInt64(Math.Round(Population * 1.00003));
        // industrialization / education updates omitted for now
    }

    public long GetProduction()
    {
        return Convert.ToInt64(Population * Industrialization / 100.0 * Education / 100.0);
    }

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

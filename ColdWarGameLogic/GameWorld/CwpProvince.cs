using OpenGSGLibrary.GameDataManager;

namespace ColdWarGameLogic.GameWorld;

/// <summary>
/// Cold War specific province model. Kept as a POCO — ViewModel provides notifications and Refresh().
/// </summary>
public class CwpProvince : Province
{
    public long Population { get; set; } = 0;
    public float Industrialization { get; set; } = 0f;
    public float Education { get; set; } = 0f;
    public string Terrain { get; set; } = string.Empty;

    public long Production => GetProduction();

    public CwpProvince()
        : base() { }

    public override void SetData(string fileName, ILookup<string, object> parsedData)
    {
        base.SetData(fileName, parsedData);

        Population = ToLong(SingleOrDefault(parsedData, "population"));
        Industrialization = ToFloat(SingleOrDefault(parsedData, "industrialization"));
        Education = ToFloat(SingleOrDefault(parsedData, "education"));
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
        // Base production from population, industrialization, and education
        // Tech multiplier is applied at country level when aggregating
        return Convert.ToInt64(
            Population / 1_000_000.0 * Industrialization / 100.0 * Education / 100.0
        );
    }

    private static object SingleOrDefault(ILookup<string, object> lookup, string key)
    {
        if (lookup == null)
            return null;
        return lookup.Contains(key) ? lookup[key].SingleOrDefault() : null;
    }

    private static float ToFloat(object o)
    {
        if (o == null)
            return 0;
        try
        {
            return Convert.ToSingle(o);
        }
        catch
        {
            return 0;
        }
    }

    private static int ToInteger(object o)
    {
        if (o == null)
            return 0;
        try
        {
            return Convert.ToInt32(o);
        }
        catch
        {
            return 0;
        }
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

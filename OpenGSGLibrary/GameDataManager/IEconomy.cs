namespace OpenGSGLibrary.GameDataManager;

public interface IEconomy
{
    void CalculateTotalProduction(WorldState worldstate);

    void GrowProvinceIndustrialization();
}

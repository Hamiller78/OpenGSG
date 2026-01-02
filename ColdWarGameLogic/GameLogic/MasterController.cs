using ColdWarGameLogic.GameWorld;
using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.GameLogic;

namespace ColdWarGameLogic.GameLogic
{
    public class MasterController
    {
        private const string GAMEDATA_PATH = @"..\..\..\..\ColdWarPrototype\GameData";

        public WorldDataManager WorldData { get; } = new WorldDataManager();
        public TickHandler TickHandler { get; } = new TickHandler();

        private readonly WorldLoader<CwpProvince, CwpCountry> _worldLoader = new();

        public void Init()
        {
            var startState = _worldLoader.CreateStartState(GAMEDATA_PATH);
            TickHandler.ConnectGameObjectEventHandlers(startState); // TODO: set state in separate method

            WorldData.LoadAll(GAMEDATA_PATH); // Only map views are still in WorldDataManager
        }

        public DateTime GetGameDateTime()
        {
            var elapsedTimespan = new TimeSpan((int)TickHandler.GetCurrentTick(), 0, 0, 0);
            return new DateTime(1950, 1, 1).Add(elapsedTimespan);
        }
    }
}

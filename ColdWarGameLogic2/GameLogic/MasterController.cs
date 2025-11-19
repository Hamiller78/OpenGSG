using System;
using OpenGSGLibrary.GameLogic;
using WorldData;

namespace Simulation
{
    public class MasterController
    {
        private const string GAMEDATA_PATH = @"..\..\..\ColdWarPrototype\GameData";

        public readonly WorldDataManager worldData = new WorldDataManager();
        public readonly TickHandler tickHandler = new TickHandler();

        private readonly OpenGSGLibrary.WorldData.WorldLoader<
            WorldData.CwpProvince,
            WorldData.CwpCountry
        > worldLoader_ = new OpenGSGLibrary.WorldData.WorldLoader<
            WorldData.CwpProvince,
            WorldData.CwpCountry
        >();

        public void Init()
        {
            var startState = worldLoader_.CreateStartState(GAMEDATA_PATH);
            tickHandler.ConnectProvinceEventHandlers(startState); // TODO: set state in separate method

            worldData.LoadAll(GAMEDATA_PATH); // Only map views are still in WorldDataManager
        }

        // TODO: redundant while worldData is public; kept for compatibility
        public WorldDataManager GetWorldManager() => worldData;

        public DateTime GetGameDateTime()
        {
            var elapsedTimespan = new TimeSpan((int)tickHandler.GetCurrentTick(), 0, 0, 0);
            return new DateTime(1950, 1, 1).Add(elapsedTimespan);
        }
    }
}

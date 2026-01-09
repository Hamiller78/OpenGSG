using System;
using ColdWarGameLogic.Events;
using ColdWarGameLogic.GameWorld;
using OpenGSGLibrary.Events;
using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.GameLogic;

namespace ColdWarGameLogic.GameLogic
{
    public class MasterController
    {
        private const string GAMEDATA_PATH = @"..\..\..\..\ColdWarPrototype\GameData";

        public WorldDataManager WorldData { get; } = new WorldDataManager();
        public TickHandler TickHandler { get; } = new TickHandler();
        public EventManager EventManager { get; private set; } = new EventManager();

        private readonly WorldLoader<
            CwpProvince,
            CwpCountry,
            CwpCountryEvent,
            CwpNewsEvent
        > _worldLoader = new();

        public void Init()
        {
            var startState = _worldLoader.CreateStartState(GAMEDATA_PATH);
            TickHandler.ConnectProvinceEventHandlers(startState);

            // Store event manager for access during gameplay
            EventManager = _worldLoader.EventManager;

            // Configure tick handler with event manager and start date
            TickHandler.SetEventManager(EventManager);
            TickHandler.SetStartDate(new DateTime(1950, 1, 1));

            WorldData.LoadAll(GAMEDATA_PATH); // Only map views are still in WorldDataManager
        }

        public DateTime GetGameDateTime()
        {
            return TickHandler.GetCurrentDate();
        }
    }
}

using System;
using ColdWarGameLogic.Events;
using ColdWarGameLogic.GameWorld;
using OpenGSGLibrary.Events;
using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.GameLogic;
using OpenGSGLibrary.Localization;

namespace ColdWarGameLogic.GameLogic
{
    public class MasterController
    {
        public const string GAMEDATA_PATH = @"..\..\..\..\ColdWarPrototype\GameData";

        public WorldDataManager WorldData { get; } = new WorldDataManager();
        public TickHandler TickHandler { get; } = new TickHandler();
        public EventManager EventManager { get; private set; } = new EventManager();
        public LocalizationManager LocalizationManager { get; private set; } =
            new LocalizationManager();
        public GameSimulationThread SimulationThread { get; private set; } = default!;

        private readonly WorldLoader<CwpProvince, CwpCountry> _worldLoader = new();

        public void Init()
        {
            var startState = _worldLoader.CreateStartState(GAMEDATA_PATH);
            TickHandler.ConnectProvinceEventHandlers(startState);

            // Store event manager and localization manager for access during gameplay
            EventManager = _worldLoader.EventManager;
            LocalizationManager = _worldLoader.LocalizationManager;

            // Configure tick handler with event manager and start date
            TickHandler.SetEventManager(EventManager);
            TickHandler.SetStartDate(new DateTime(1950, 1, 1));

            // Set default player country (e.g., USA for testing)
            TickHandler.SetPlayerCountry("USA");

            WorldData.LoadAll(GAMEDATA_PATH); // Only map views are still in WorldDataManager

            // Create simulation thread
            SimulationThread = new GameSimulationThread(TickHandler);
            SimulationThread.SetSpeed(SimulationSpeed.Normal);
        }

        public DateTime GetGameDateTime()
        {
            return TickHandler.GetCurrentDate();
        }
    }
}

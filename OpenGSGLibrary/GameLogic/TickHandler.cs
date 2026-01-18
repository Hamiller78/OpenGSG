using System;
using OpenGSGLibrary.Events;
using OpenGSGLibrary.GameDataManager;

namespace OpenGSGLibrary.GameLogic
{
    /// <summary>
    /// Responsible for keeping the current world state and notifying
    /// subscribed provinces when a tick finishes.
    /// </summary>
    public class TickHandler
    {
        /// <summary>
        /// Notifies subscribers (provinces and other systems) that the tick finished.
        /// Provinces are subscribed to this to run their per-tick updates.
        /// </summary>
        public event EventHandler? TickDone;

        /// <summary>
        /// Raised after all TickDone subscribers have been invoked.
        /// Intended for the UI layer (or adapters) to perform a single refresh
        /// after the model update is complete.
        /// Static so UI adapters can wire up easily without holding a reference to a TickHandler instance.
        /// </summary>
        public static event EventHandler<TickEventArgs>? UIRefreshRequested;

        /// <summary>
        /// Raised when an event is triggered and should be presented to the player.
        /// Static for easy UI subscription.
        /// </summary>
        public static event EventHandler<EventTriggeredArgs>? EventTriggered;

        private readonly PlayerManager _playerManager = new();
        private WorldState? _currentWorldState;
        private long _currentTick = 0;
        private EventManager? _eventManager;
        private DateTime _startDate = new DateTime(1950, 1, 1); // Default start date

        /// <summary>
        /// Sets the event manager for event evaluation.
        /// </summary>
        public void SetEventManager(EventManager eventManager)
        {
            _eventManager = eventManager;
        }

        /// <summary>
        /// Sets the game start date for date-based event triggers.
        /// </summary>
        public void SetStartDate(DateTime startDate)
        {
            _startDate = startDate;
        }

        /// <summary>
        /// Gets the current game date based on start date and elapsed ticks.
        /// </summary>
        public DateTime GetCurrentDate()
        {
            var elapsedTimespan = new TimeSpan((int)_currentTick, 0, 0, 0);
            return _startDate.Add(elapsedTimespan);
        }

        /// <summary>
        /// Connects world state to tick handler which includes setting the state in TickHandlers
        /// and associating event handlers.
        /// </summary>
        /// <param name="newState">WorldState to set in TickHandler.</param>
        public void ConnectProvinceEventHandlers(WorldState newState)
        {
            _currentWorldState = newState;

            var provinceDict = _currentWorldState?.GetProvinceTable();
            if (provinceDict != null)
            {
                foreach (var province in provinceDict.Values)
                {
                    // Provinces implement OnTickDone(object, EventArgs)
                    // The TickDone event uses EventArgs; subscribe via a lambda to adapt the signature.
                    TickDone += (s, ev) => province.OnTickDone(s, ev);
                }
            }
        }

        /// <summary>
        /// Returns the world state used by the tick handler
        /// </summary>
        /// <returns>WorldState object for the current tick which will be modified each tick.</returns>
        public WorldState GetState() => _currentWorldState!;

        /// <summary>
        /// Method to do stuff when an new tick starts.
        /// In the original code this collected GUI input and launched AI threads.
        /// </summary>
        public void BeginNewTick()
        {
            // do timed game events
            // launch AI threads
            if (_currentWorldState is not null)
            {
                _playerManager.CalculateStrategies(_currentWorldState);
            }
            // collect GUI input
        }

        /// <summary>
        /// Is the current tick complete?
        /// </summary>
        /// <returns>Boolean whether the current tick is complete</returns>
        public bool IsTickComplete()
        {
            if (_playerManager.IsEverybodyDone() == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method to do stuff when a tick is completed and the next WorldState is calculated.
        /// Notifies provinces (TickDone) and then requests a single UI refresh (UIRefreshRequested).
        /// </summary>
        public void FinishTick()
        {
            // lock GUI input
            // calculate world in next tick
            _currentTick += 1;
            var args = new TickEventArgs((int)_currentTick);

            // notify all interested classes (synchronous invoke)
            TickDone?.Invoke(this, args);

            // Monthly updates (every 30 ticks = 1 month)
            if (_currentTick % 30 == 0)
            {
                PerformMonthlyUpdates();
            }

            // Evaluate events after world updates
            EvaluateEvents();

            // After provinces and other TickDone subscribers have run, request UI refresh
            // so the UI can pull latest model state (or ViewModels can Refresh()).
            UIRefreshRequested?.Invoke(this, args);
        }

        /// <summary>
        /// Performs monthly updates: country stat recalculations, budget allocation, etc.
        /// </summary>
        private void PerformMonthlyUpdates()
        {
            if (_currentWorldState == null)
                return;

            var countries = _currentWorldState.GetCountryTable();
            if (countries != null)
            {
                foreach (var country in countries.Values)
                {
                    // Call base class method - polymorphism handles game-specific implementation
                    country.RecalculateStats();
                }
            }
        }

        /// <summary>
        /// Evaluates all events and fires those whose triggers are met.
        /// Shows UI for active player country, auto-executes for AI countries.
        /// </summary>
        private void EvaluateEvents()
        {
            if (_eventManager == null || _currentWorldState == null)
                return;

            var currentDate = GetCurrentDate();
            var currentTick = _currentTick;

            // Evaluate country events for ALL countries
            var countries = _currentWorldState.GetCountryTable();
            if (countries != null)
            {
                foreach (var country in countries.Values)
                {
                    var context = new EventEvaluationContext
                    {
                        WorldState = _currentWorldState,
                        CurrentDate = currentDate,
                        CurrentCountryTag = country.Tag,
                        TickHandler = this,
                        EventManager = _eventManager,
                    };

                    var countryEvents = _eventManager.GetCountryEvents();
                    foreach (var evt in countryEvents)
                    {
                        // Skip events that can only be triggered explicitly
                        if (evt.IsTriggeredOnly)
                            continue;

                        if (evt.ShouldFire(context, currentTick))
                        {
                            if (evt.TriggerOnlyOnce)
                            {
                                evt.HasTriggered = true;
                            }

                            // Determine if this is the player's country or AI
                            bool isPlayerCountry = country.Tag == ActivePlayerCountryTag;

                            // Hidden events always auto-execute
                            if (evt.Hidden)
                            {
                                if (evt.Options.Count > 0)
                                {
                                    evt.Options[0].Execute(context);
                                }
                                evt.ResetMTTH();
                            }
                            // AI countries auto-execute (choose first option)
                            else if (!isPlayerCountry)
                            {
                                // AI decision-making: for now, just pick first option
                                // Future: AI can evaluate options and choose best one
                                if (evt.Options.Count > 0)
                                {
                                    evt.Options[0].Execute(context);
                                }
                                evt.ResetMTTH();
                            }
                            // Player country: show UI popup
                            else
                            {
                                EventTriggered?.Invoke(this, new EventTriggeredArgs(evt, context));
                            }
                        }
                    }
                }
            }

            // News events fire globally (always show to player if there is one)
            var newsContext = new EventEvaluationContext
            {
                WorldState = _currentWorldState,
                CurrentDate = currentDate,
                CurrentCountryTag = ActivePlayerCountryTag, // Can be null for observer
                TickHandler = this,
                EventManager = _eventManager,
            };

            // Evaluate news events - check against all countries if they have triggers
            var newsEvents = _eventManager.GetNewsEvents();
            foreach (var evt in newsEvents)
            {
                // Skip events that can only be triggered explicitly
                if (evt.IsTriggeredOnly)
                    continue;

                // News events with triggers should be evaluated per-country
                if (evt.Triggers.Count > 0)
                {
                    // Evaluate for each country
                    foreach (var country in countries.Values)
                    {
                        var countryNewsContext = new EventEvaluationContext
                        {
                            WorldState = _currentWorldState,
                            CurrentDate = currentDate,
                            CurrentCountryTag = country.Tag,
                            TickHandler = this,
                            EventManager = _eventManager,
                        };

                        if (evt.ShouldFire(countryNewsContext, currentTick))
                        {
                            if (evt.TriggerOnlyOnce)
                            {
                                evt.HasTriggered = true;
                            }

                            bool isPlayerCountry = country.Tag == ActivePlayerCountryTag;

                            if (evt.Hidden)
                            {
                                if (evt.Options.Count > 0)
                                {
                                    evt.Options[0].Execute(countryNewsContext);
                                }
                                evt.ResetMTTH();
                            }
                            else if (!isPlayerCountry)
                            {
                                // AI auto-executes
                                if (evt.Options.Count > 0)
                                {
                                    evt.Options[0].Execute(countryNewsContext);
                                }
                                evt.ResetMTTH();
                            }
                            else
                            {
                                // Show to player
                                EventTriggered?.Invoke(
                                    this,
                                    new EventTriggeredArgs(evt, countryNewsContext)
                                );
                            }
                        }
                    }
                }
                else
                {
                    // No triggers = truly global news event
                    // Existing global news event logic here
                }
            }
        }

        /// <summary>
        /// Gets the current tick number.
        /// Conversion to a date is the responsibility of the game-specific logic.
        /// </summary>
        /// <returns>Current tick as a long integer.</returns>
        public long GetCurrentTick() => _currentTick;

        /// <summary>
        /// Fires an event immediately for a specific country, bypassing trigger evaluation.
        /// Used for debugging and testing.
        /// </summary>
        /// <param name="eventId">ID of the event to fire (e.g., "event.1")</param>
        /// <param name="countryTag">Tag of the country to fire the event for</param>
        /// <returns>True if event was found and fired, false otherwise</returns>
        public bool FireEventDebug(string eventId, string countryTag)
        {
            if (_eventManager == null || _currentWorldState == null)
                return false;

            var countries = _currentWorldState.GetCountryTable();
            if (!countries.TryGetValue(countryTag, out var country))
                return false;

            // Find the event
            var countryEvents = _eventManager.GetCountryEvents();
            var newsEvents = _eventManager.GetNewsEvents();

            var countryEvent = countryEvents.FirstOrDefault(e => e.Id == eventId);
            if (countryEvent != null)
            {
                var context = new EventEvaluationContext
                {
                    WorldState = _currentWorldState,
                    CurrentDate = GetCurrentDate(),
                    CurrentCountryTag = countryTag,
                    TickHandler = this,
                    EventManager = _eventManager,
                };

                // Fire event using the same pattern as EvaluateEvents()
                if (countryEvent.Hidden)
                {
                    // Auto-execute for hidden events
                    if (countryEvent.Options.Count > 0)
                    {
                        countryEvent.Options[0].Execute(context);
                    }
                }
                else
                {
                    // Raise event for UI to display
                    EventTriggered?.Invoke(this, new EventTriggeredArgs(countryEvent, context));
                }
                return true;
            }

            var newsEvent = newsEvents.FirstOrDefault(e => e.Id == eventId);
            if (newsEvent != null)
            {
                var context = new EventEvaluationContext
                {
                    WorldState = _currentWorldState,
                    CurrentDate = GetCurrentDate(),
                    CurrentCountryTag = countryTag,
                    TickHandler = this,
                    EventManager = _eventManager,
                };

                // Fire event using the same pattern as EvaluateEvents()
                if (newsEvent.Hidden)
                {
                    // Auto-execute for hidden events
                    if (newsEvent.Options.Count > 0)
                    {
                        newsEvent.Options[0].Execute(context);
                    }
                }
                else
                {
                    // Raise event for UI to display
                    EventTriggered?.Invoke(this, new EventTriggeredArgs(newsEvent, context));
                }
                return true;
            }

            return false; // Event not found
        }

        /// <summary>
        /// Gets or sets the active player country tag.
        /// Null represents observer mode (no active country).
        /// </summary>
        public string? ActivePlayerCountryTag { get; private set; }

        /// <summary>
        /// Sets the active player country.
        /// </summary>
        /// <param name="countryTag">Country tag to play as, or null for observer mode</param>
        /// <returns>True if country exists or is observer mode, false otherwise</returns>
        public bool SetPlayerCountry(string? countryTag)
        {
            // Allow null for observer mode
            if (countryTag == null)
            {
                ActivePlayerCountryTag = null;
                return true;
            }

            // Validate country exists
            var countries = _currentWorldState?.GetCountryTable();
            if (countries == null || !countries.ContainsKey(countryTag))
            {
                return false;
            }

            ActivePlayerCountryTag = countryTag;
            return true;
        }

        /// <summary>
        /// Gets whether currently in observer mode.
        /// </summary>
        public bool IsObserverMode => string.IsNullOrEmpty(ActivePlayerCountryTag);

        /// <summary>
        /// Triggers an event for the player (or AI) to handle.
        /// Used when events are triggered by effects (not just during evaluation).
        /// </summary>
        public void TriggerEvent(GameEvent evt, EventEvaluationContext context)
        {
            EventTriggered?.Invoke(this, new EventTriggeredArgs(evt, context));
        }
    }

    /// <summary>
    /// Helper class passed with the TickDone event.
    /// Extra argument is the current tick number.
    /// </summary>
    public class TickEventArgs(int tickPar) : EventArgs
    {
        public int Tick { get; set; } = tickPar;
    }

    /// <summary>
    /// Event args for when a game event is triggered.
    /// </summary>
    public class EventTriggeredArgs : EventArgs
    {
        public GameEvent Event { get; }
        public EventEvaluationContext Context { get; }

        public EventTriggeredArgs(GameEvent evt, EventEvaluationContext context)
        {
            Event = evt;
            Context = context;
        }
    }
}

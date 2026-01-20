using OpenGSGLibrary.Events;
using OpenGSGLibrary.GameDataManager;

namespace OpenGSGLibrary.GameLogic;

/// <summary>
/// Responsible for keeping the current world state and notifying
/// subscribed provinces when a tick finishes.
/// </summary>
public class TickHandler
{
    /// <summary>
    /// Gets or sets the active player country tag.
    /// Null represents observer mode (no active country).
    /// </summary>
    public string? ActivePlayerCountryTag { get; private set; }

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
    public static event EventHandler<GameEventTriggeredEventArgs>? EventTriggered;

    private readonly PlayerManager _playerManager = new();
    private WorldState? _currentWorldState;
    private long _currentTick = 0;
    private EventEvaluator? _eventEvaluator;
    private DateTime _startDate = new DateTime(1950, 1, 1, 0, 0, 0, DateTimeKind.Unspecified); // Default start date

    /// <summary>
    /// Sets the event manager for event evaluation.
    /// </summary>
    public void SetEventManager(EventManager eventManager)
    {
        _eventEvaluator = new EventEvaluator(eventManager);
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
    public bool IsTickComplete() => _playerManager.IsEverybodyDone();

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
        if (_eventEvaluator != null && _currentWorldState != null)
        {
            _eventEvaluator.EvaluateAllEvents(
                _currentWorldState,
                GetCurrentDate(),
                _currentTick,
                ActivePlayerCountryTag
            );
        }

        // After provinces and other TickDone subscribers have run, request UI refresh
        // so the UI can pull latest model state (or ViewModels can Refresh()).
        UIRefreshRequested?.Invoke(null, args);
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
    /// Gets the current tick number.
    /// </summary>
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
        if (_eventEvaluator == null || _currentWorldState == null)
            return false;

        return _eventEvaluator.FireEventDebug(
            eventId,
            countryTag,
            _currentWorldState,
            GetCurrentDate(),
            ActivePlayerCountryTag
        );
    }

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
    public static void TriggerEvent(GameEvent evt, EventEvaluationContext context)
    {
        EventTriggered?.Invoke(null, new GameEventTriggeredEventArgs(evt, context));
    }
}

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
    private DateTime _startDate = new DateTime(1950, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
    private readonly object _tickLock = new();

    /// <summary>
    /// Queue of events scheduled to fire in the future.
    /// </summary>
    private readonly List<ScheduledEvent> _scheduledEvents = new List<ScheduledEvent>();

    /// <summary>
    /// Sets the event manager for event evaluation.
    /// </summary>
    public void SetEventManager(EventManager eventManager)
    {
        var notifier = new TickHandlerEventNotifier();
        _eventEvaluator = new EventEvaluator(eventManager, notifier);
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
    public void ConnectProvinceEventHandlers(WorldState newState)
    {
        _currentWorldState = newState;

        var provinceDict = _currentWorldState?.GetProvinceTable();
        if (provinceDict != null)
        {
            foreach (var province in provinceDict.Values)
            {
                TickDone += (s, ev) => province.OnTickDone(s!, ev);
            }
        }
    }

    /// <summary>
    /// Returns the world state used by the tick handler
    /// </summary>
    public WorldState GetState() => _currentWorldState!;

    /// <summary>
    /// Method to do stuff when an new tick starts.
    /// </summary>
    public void BeginNewTick()
    {
        if (_currentWorldState is not null)
        {
            _playerManager.CalculateStrategies(_currentWorldState);
        }
    }

    /// <summary>
    /// Is the current tick complete?
    /// </summary>
    public bool IsTickComplete() => _playerManager.IsEverybodyDone();

    /// <summary>
    /// Method to do stuff when a tick is completed and the next WorldState is calculated.
    /// Notifies provinces (TickDone) and then requests a single UI refresh (UIRefreshRequested).
    /// </summary>
    public void FinishTick()
    {
        if (!IsTickComplete())
            return;

        lock (_tickLock)
        {
            _currentTick++;

            // Process scheduled events first
            ProcessScheduledEvents();

            // Notify provinces
            TickDone?.Invoke(this, EventArgs.Empty);

            // Then evaluate normal events
            _eventEvaluator?.EvaluateAllEvents(
                _currentWorldState!,
                GetCurrentDate(),
                _currentTick,
                ActivePlayerCountryTag,
                this
            );

            // Perform monthly updates every 30 ticks (days)
            if (_currentTick % 30 == 0)
            {
                PerformMonthlyUpdates();
            }

            // Fire UIRefreshRequested event
            OnUIRefreshRequested();
        }
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
    /// </summary>
    public bool FireEventDebug(string eventId, string countryTag)
    {
        if (_eventEvaluator == null || _currentWorldState == null)
            return false;

        return _eventEvaluator.FireEventDebug(
            eventId,
            countryTag,
            _currentWorldState,
            GetCurrentDate(),
            ActivePlayerCountryTag,
            this
        );
    }

    /// <summary>
    /// Sets the active player country.
    /// </summary>
    public bool SetPlayerCountry(string? countryTag)
    {
        if (countryTag == null)
        {
            ActivePlayerCountryTag = null;
            return true;
        }

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
    /// </summary>
    public static void TriggerEvent(GameEvent evt, EventEvaluationContext context)
    {
        EventTriggered?.Invoke(null, new GameEventTriggeredEventArgs(evt, context));
    }

    /// <summary>
    /// Sets the current game date by adjusting the tick counter.
    /// </summary>
    public void SetCurrentDate(DateTime targetDate)
    {
        TimeSpan difference = targetDate - _startDate;
        long newTick = (long)difference.TotalDays;

        if (newTick < 0)
            newTick = 0;

        _currentTick = newTick;
    }

    /// <summary>
    /// Schedules an event to fire after a specified number of days.
    /// </summary>
    public void ScheduleEvent(string eventId, string targetCountryTag, int days, bool isNewsEvent)
    {
        var fireDate = GetCurrentDate().AddDays(days);

        _scheduledEvents.Add(
            new ScheduledEvent
            {
                EventId = eventId,
                TargetCountryTag = targetCountryTag,
                FireDate = fireDate,
                IsNewsEvent = isNewsEvent,
            }
        );
    }

    /// <summary>
    /// Checks and fires any scheduled events that are due.
    /// </summary>
    private void ProcessScheduledEvents()
    {
        if (_eventEvaluator == null || _currentWorldState == null)
            return;

        var currentDate = GetCurrentDate();
        var eventsToFire = _scheduledEvents.Where(e => e.FireDate <= currentDate).ToList();

        foreach (var scheduledEvent in eventsToFire)
        {
            _eventEvaluator.FireEventDebug(
                scheduledEvent.EventId,
                scheduledEvent.TargetCountryTag,
                _currentWorldState,
                currentDate,
                ActivePlayerCountryTag,
                this
            );

            _scheduledEvents.Remove(scheduledEvent);
        }
    }

    /// <summary>
    /// Invokes the UIRefreshRequested event.
    /// </summary>
    private void OnUIRefreshRequested()
    {
        var args = new TickEventArgs((int)_currentTick);
        UIRefreshRequested?.Invoke(null, args);
    }
}

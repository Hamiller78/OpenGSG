using System;
using System.Diagnostics;
using System.Threading;

namespace OpenGSGLibrary.GameLogic
{
    /// <summary>
    /// Manages the game simulation loop on a background thread.
    /// Advances ticks at a controlled rate with configurable speed.
    /// </summary>
    public class GameSimulationThread
    {
        private readonly TickHandler _tickHandler;
        private Thread? _simulationThread;
        private volatile bool _isRunning;
        private volatile bool _isPaused;
        private volatile float _ticksPerSecond = 1.0f; // Default: 1 tick per second
        private readonly object _lock = new object();

        /// <summary>
        /// Raised when simulation starts.
        /// </summary>
        public event EventHandler? SimulationStarted;

        /// <summary>
        /// Raised when simulation pauses.
        /// </summary>
        public event EventHandler? SimulationPaused;

        /// <summary>
        /// Raised when simulation resumes.
        /// </summary>
        public event EventHandler? SimulationResumed;

        /// <summary>
        /// Raised when simulation stops.
        /// </summary>
        public event EventHandler? SimulationStopped;

        /// <summary>
        /// Gets whether the simulation is currently running.
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Gets whether the simulation is paused.
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// Gets or sets the simulation speed (ticks per second).
        /// Valid range: 0.1 to 100.0
        /// </summary>
        public double TicksPerSecond
        {
            get => _ticksPerSecond;
            set
            {
                if (value < 0.1 || value > 100.0)
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        "Ticks per second must be between 0.1 and 100.0"
                    );
                _ticksPerSecond = (float)value;
            }
        }

        public GameSimulationThread(TickHandler tickHandler)
        {
            _tickHandler = tickHandler ?? throw new ArgumentNullException(nameof(tickHandler));
        }

        /// <summary>
        /// Starts the simulation thread.
        /// </summary>
        public void Start()
        {
            lock (_lock)
            {
                if (_isRunning)
                    return;

                _isRunning = true;
                _isPaused = false;

                _simulationThread = new Thread(SimulationLoop)
                {
                    Name = "GameSimulation",
                    IsBackground =
                        true // Allows app to close without explicit stop
                    ,
                };

                _simulationThread.Start();
                SimulationStarted?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Pauses the simulation (thread continues running but doesn't advance ticks).
        /// </summary>
        public void Pause()
        {
            lock (_lock)
            {
                if (!_isRunning || _isPaused)
                    return;

                _isPaused = true;
                SimulationPaused?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Resumes the simulation from pause.
        /// </summary>
        public void Resume()
        {
            lock (_lock)
            {
                if (!_isRunning || !_isPaused)
                    return;

                _isPaused = false;
                Monitor.PulseAll(_lock); // Wake up simulation thread
                SimulationResumed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Stops the simulation thread completely.
        /// </summary>
        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning)
                    return;

                _isRunning = false;
                _isPaused = false;
                Monitor.PulseAll(_lock); // Wake up thread if waiting
            }

            // Wait for thread to finish (with timeout)
            _simulationThread?.Join(TimeSpan.FromSeconds(2));
            SimulationStopped?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Main simulation loop running on background thread.
        /// </summary>
        private void SimulationLoop()
        {
            var stopwatch = Stopwatch.StartNew();
            long lastTickTime = 0;

            while (_isRunning)
            {
                // Check if paused
                lock (_lock)
                {
                    while (_isPaused && _isRunning)
                    {
                        Monitor.Wait(_lock); // Wait until resumed or stopped
                    }

                    if (!_isRunning)
                        break;
                }

                // Calculate minimum time between ticks based on speed
                long minTickIntervalMs = (long)(1000.0 / _ticksPerSecond);
                long currentTime = stopwatch.ElapsedMilliseconds;
                long timeSinceLastTick = currentTime - lastTickTime;

                // Check if minimum interval has passed
                if (timeSinceLastTick >= minTickIntervalMs)
                {
                    // Begin tick calculations
                    _tickHandler.BeginNewTick();

                    // Wait for tick to complete (all AI, calculations done)
                    while (!_tickHandler.IsTickComplete() && _isRunning && !_isPaused)
                    {
                        Thread.Sleep(1); // Yield to other threads
                    }

                    // Only finish tick if still running
                    if (_isRunning && !_isPaused)
                    {
                        _tickHandler.FinishTick();
                        lastTickTime = stopwatch.ElapsedMilliseconds;
                    }
                }
                else
                {
                    // Sleep for remaining time until next tick
                    int sleepTime = (int)(minTickIntervalMs - timeSinceLastTick);
                    if (sleepTime > 0)
                    {
                        Thread.Sleep(Math.Min(sleepTime, 50)); // Max sleep 50ms for responsiveness
                    }
                }
            }
        }

        /// <summary>
        /// Sets a preset simulation speed.
        /// </summary>
        public void SetSpeed(SimulationSpeed speed)
        {
            TicksPerSecond = speed switch
            {
                SimulationSpeed.VerySlow => 0.5, // 1 tick per 2 seconds
                SimulationSpeed.Slow => 1.0, // 1 tick per second
                SimulationSpeed.Normal => 2.0, // 2 ticks per second
                SimulationSpeed.Fast => 5.0, // 5 ticks per second
                SimulationSpeed.VeryFast => 10.0, // 10 ticks per second
                SimulationSpeed.Maximum => 50.0, // 50 ticks per second
                _ => 1.0,
            };
        }
    }

    /// <summary>
    /// Predefined simulation speeds.
    /// </summary>
    public enum SimulationSpeed
    {
        VerySlow, // 0.5 ticks/sec
        Slow, // 1 tick/sec
        Normal, // 2 ticks/sec
        Fast, // 5 ticks/sec
        VeryFast, // 10 ticks/sec
        Maximum // 50 ticks/sec
        ,
    }
}

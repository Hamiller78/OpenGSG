using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenGSGLibrary.GameFilesParser;
using OpenGSGLibrary.Tools;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Factory for creating GameEvent objects from parsed event files.
    /// Uses the same pattern as GameObjectFactory for provinces/countries.
    /// </summary>
    public static class EventFactory
    {
        /// <summary>
        /// Loads all events from a folder and returns them in an EventManager.
        /// </summary>
        /// <typeparam name="TCountryEvent">Game-specific country event type.</typeparam>
        /// <typeparam name="TNewsEvent">Game-specific news event type.</typeparam>
        /// <param name="eventsPath">Path to events directory.</param>
        /// <returns>EventManager containing all loaded events.</returns>
        public static EventManager LoadFromFolder<TCountryEvent, TNewsEvent>(string eventsPath)
            where TCountryEvent : CountryEvent, new()
            where TNewsEvent : NewsEvent, new()
        {
            var eventManager = new EventManager();

            if (!Directory.Exists(eventsPath))
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Warning, $"Events directory not found: {eventsPath}");
                return eventManager;
            }

            var eventFiles = Directory.GetFiles(eventsPath, "*.txt", SearchOption.AllDirectories);

            GlobalLogger
                .GetInstance()
                .WriteLine(LogLevel.Info, $"Loading events from {eventFiles.Length} file(s)");

            foreach (var file in eventFiles)
            {
                try
                {
                    LoadEventFile<TCountryEvent, TNewsEvent>(file, eventManager);
                }
                catch (Exception ex)
                {
                    GlobalLogger
                        .GetInstance()
                        .WriteLine(LogLevel.Err, $"Error loading event file {file}: {ex.Message}");
                }
            }

            GlobalLogger
                .GetInstance()
                .WriteLine(LogLevel.Info, $"Loaded {eventManager.EventCount} total events");

            return eventManager;
        }

        /// <summary>
        /// Loads events from a single file.
        /// </summary>
        private static void LoadEventFile<TCountryEvent, TNewsEvent>(
            string filePath,
            EventManager eventManager
        )
            where TCountryEvent : CountryEvent, new()
            where TNewsEvent : NewsEvent, new()
        {
            using var rawFile = File.OpenText(filePath);

            var scanner = new Scanner();
            var parser = new Parser();

            var tokenStream = scanner.Scan(rawFile);
            var nextParseData = parser.Parse(tokenStream);

            foreach (var group in nextParseData)
            {
                var eventType = group.Key;
                var eventData = group.ToList();

                if (eventType == "country_event")
                {
                    foreach (var data in eventData)
                    {
                        if (data is ILookup<string, object> eventLookup)
                        {
                            var evt = new TCountryEvent();
                            evt.SetData(eventLookup);
                            eventManager.RegisterEvent(evt);
                            GlobalLogger
                                .GetInstance()
                                .WriteLine(LogLevel.Info, $"Loaded country event: {evt.Id}");
                        }
                    }
                }
                else if (eventType == "news_event")
                {
                    foreach (var data in eventData)
                    {
                        if (data is ILookup<string, object> eventLookup)
                        {
                            var evt = new TNewsEvent();
                            evt.SetData(eventLookup);
                            eventManager.RegisterEvent(evt);
                            GlobalLogger
                                .GetInstance()
                                .WriteLine(LogLevel.Info, $"Loaded news event: {evt.Id}");
                        }
                    }
                }
            }
        }
    }
}

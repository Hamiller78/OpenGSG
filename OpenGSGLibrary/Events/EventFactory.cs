using OpenGSGLibrary.GameFilesParser;
using OpenGSGLibrary.Tools;

namespace OpenGSGLibrary.Events;

/// <summary>
/// Factory for creating GameEvent objects from parsed event files.
/// Uses the same pattern as GameObjectFactory for provinces/countries.
/// </summary>
public static class EventFactory
{
    /// <summary>
    /// Loads all events from a folder and returns them in an EventManager.
    /// </summary>
    public static EventManager LoadFromFolder(string eventsPath)
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
                LoadEventFile(file, eventManager);
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
    private static void LoadEventFile(string filePath, EventManager eventManager)
    {
        using var rawFile = File.OpenText(filePath);

        var scanner = new Scanner();
        var parser = new Parser();

        var tokenStream = scanner.Scan(rawFile);
        var nextParseData = parser.Parse(tokenStream);

        foreach (var group in nextParseData)
        {
            ProcessEventGroup(group, eventManager);
        }
    }

    private static void ProcessEventGroup(
        IGrouping<string, object> group,
        EventManager eventManager
    )
    {
        var eventType = group.Key;
        var eventData = group.ToList();

        foreach (var data in eventData)
        {
            if (data is ILookup<string, object> eventLookup)
            {
                var evt = CreateEvent(eventType, eventLookup);
                if (evt != null)
                {
                    eventManager.RegisterEvent(evt);
                    GlobalLogger
                        .GetInstance()
                        .WriteLine(LogLevel.Info, $"Loaded {eventType}: {evt.Id}");
                }
            }
        }
    }

    private static GameEvent? CreateEvent(string eventType, ILookup<string, object> eventLookup)
    {
        return eventType switch
        {
            "country_event" => CreateCountryEvent(eventLookup),
            "news_event" => CreateNewsEvent(eventLookup),
            _ => LogUnknownEventType(eventType),
        };
    }

    private static GameEvent? LogUnknownEventType(string eventType)
    {
        GlobalLogger.GetInstance().WriteLine(LogLevel.Warning, $"Unknown event type: {eventType}");
        return null;
    }

    private static CountryEvent CreateCountryEvent(ILookup<string, object> parsedData)
    {
        var countryEvent = new CountryEvent();
        countryEvent.SetData(parsedData);
        return countryEvent;
    }

    private static NewsEvent CreateNewsEvent(ILookup<string, object> parsedData)
    {
        var newsEvent = new NewsEvent();
        newsEvent.SetData(parsedData);

        // Parse recipients block (determines who receives the news event)
        if (parsedData.Contains("recipients"))
        {
            var recipientsData =
                parsedData["recipients"].FirstOrDefault() as ILookup<string, object>;
            if (recipientsData != null)
            {
                newsEvent.Recipients = newsEvent.ParseTriggers(recipientsData);
            }
        }

        return newsEvent;
    }
}

using OpenGSGLibrary.Events;
using OpenGSGLibrary.GameDataManager;

namespace OpenGSG_UnitTests.Events
{
    [TestFixture]
    public class EventEvaluatorTests
    {
        [Test]
        public void NewsEvent_WithDateTrigger_FiresGlobally()
        {
            // Arrange
            var eventManager = new EventManager();
            var notifier = new TestEventNotifier();
            var evaluator = new EventEvaluator(eventManager, notifier);

            var newsEvent = new NewsEvent
            {
                Id = "test.1",
                Title = "Test Event",
                Triggers = new List<IEventTrigger>
                {
                    new DateEqualTrigger { Date = new DateTime(1950, 6, 24) },
                },
                Options = new List<EventOption> { new EventOption { Name = "OK" } },
            };
            eventManager.RegisterEvent(newsEvent);

            var worldState = CreateTestWorldState(new[] { "USA", "USSR" });
            var currentDate = new DateTime(1950, 6, 24);

            // Act
            evaluator.EvaluateAllEvents(worldState, currentDate, 1, "USA", null!);

            // Assert
            Assert.That(notifier.TriggeredEvents, Has.Count.EqualTo(1));
            Assert.That(notifier.TriggeredEvents[0].Event.Id, Is.EqualTo("test.1"));
            Assert.That(notifier.TriggeredEvents[0].Context.CurrentCountryTag, Is.EqualTo("USA"));
        }

        [Test]
        public void NewsEvent_WithRecipients_ShowsOnlyToMatchingCountries()
        {
            // Arrange
            var eventManager = new EventManager();
            var notifier = new TestEventNotifier();
            var evaluator = new EventEvaluator(eventManager, notifier);

            var newsEvent = new NewsEvent
            {
                Id = "test.2",
                Title = "Test Event",
                Recipients = new List<IEventTrigger> { new TagTrigger { Tag = "USSR" } },
                Options = new List<EventOption> { new EventOption { Name = "OK" } },
            };
            eventManager.RegisterEvent(newsEvent);

            var worldState = CreateTestWorldState(new[] { "USA", "USSR", "ROK" });
            var currentDate = new DateTime(1950, 1, 1);

            // Act
            evaluator.EvaluateAllEvents(worldState, currentDate, 1, "USSR", null!);

            // Assert
            Assert.That(notifier.TriggeredEvents, Has.Count.EqualTo(1));
            Assert.That(notifier.TriggeredEvents[0].Context.CurrentCountryTag, Is.EqualTo("USSR"));
        }

        [Test]
        public void CountryEvent_EvaluatesPerCountry()
        {
            // Arrange
            var eventManager = new EventManager();
            var notifier = new TestEventNotifier();
            var evaluator = new EventEvaluator(eventManager, notifier);

            var countryEvent = new CountryEvent
            {
                Id = "country.1",
                Title = "USA Event",
                Triggers = new List<IEventTrigger> { new TagTrigger { Tag = "USA" } },
                Options = new List<EventOption> { new EventOption { Name = "OK" } },
            };
            eventManager.RegisterEvent(countryEvent);

            var worldState = CreateTestWorldState(new[] { "USA", "USSR" });
            var currentDate = new DateTime(1950, 1, 1);

            // Act
            evaluator.EvaluateAllEvents(worldState, currentDate, 1, "USA", null!);

            // Assert
            Assert.That(notifier.TriggeredEvents, Has.Count.EqualTo(1));
            Assert.That(notifier.TriggeredEvents[0].Event.Id, Is.EqualTo("country.1"));
            Assert.That(notifier.TriggeredEvents[0].Context.CurrentCountryTag, Is.EqualTo("USA"));
        }

        [Test]
        public void NewsEvent_WithMTTH_FiresWithCorrectProbability()
        {
            // Arrange
            var eventManager = new EventManager();
            var notifier = new TestEventNotifier();
            var evaluator = new EventEvaluator(eventManager, notifier);

            var newsEvent = new NewsEvent
            {
                Id = "nureyev.1",
                Title = "Nureyev Defection",
                MeanTimeToHappenDays = 180,
                Triggers = new List<IEventTrigger>
                {
                    new DateGreaterEqualTrigger { Date = new DateTime(1961, 1, 1) },
                    new DateLessThanTrigger { Date = new DateTime(1962, 1, 1) },
                },
                Options = new List<EventOption> { new EventOption { Name = "Interesting" } },
            };
            eventManager.RegisterEvent(newsEvent);

            var worldState = CreateTestWorldState(new[] { "USA" });
            var currentDate = new DateTime(1961, 6, 1);

            int fireCount = 0;
            // Run 100 times to test MTTH probability
            for (int i = 0; i < 100; i++)
            {
                notifier.TriggeredEvents.Clear();
                newsEvent.ResetMTTH(); // Reset for each test

                evaluator.EvaluateAllEvents(worldState, currentDate, i, "USA", null!);

                if (notifier.TriggeredEvents.Count > 0)
                    fireCount++;
            }

            // With MTTH of 180 days, probability per day is ~0.56%
            // Over 100 ticks, we expect roughly 0-10 fires (allow some variance)
            Assert.That(fireCount, Is.InRange(0, 10));
        }

        [Test]
        public void Event_WithTriggerOnlyOnce_FiresOnlyOnce()
        {
            // Arrange
            var eventManager = new EventManager();
            var notifier = new TestEventNotifier();
            var evaluator = new EventEvaluator(eventManager, notifier);

            var newsEvent = new NewsEvent
            {
                Id = "once.1",
                Title = "One Time Event",
                TriggerOnlyOnce = true,
                Triggers = new List<IEventTrigger>
                {
                    new DateGreaterEqualTrigger { Date = new DateTime(1950, 1, 1) },
                },
                Options = new List<EventOption> { new EventOption { Name = "OK" } },
            };
            eventManager.RegisterEvent(newsEvent);

            var worldState = CreateTestWorldState(new[] { "USA" });
            var currentDate = new DateTime(1950, 1, 1);

            // Act - evaluate twice
            evaluator.EvaluateAllEvents(worldState, currentDate, 1, "USA", null!);
            int firstCount = notifier.TriggeredEvents.Count;

            notifier.TriggeredEvents.Clear();
            evaluator.EvaluateAllEvents(worldState, currentDate, 2, "USA", null!);
            int secondCount = notifier.TriggeredEvents.Count;

            // Assert
            Assert.That(firstCount, Is.EqualTo(1));
            Assert.That(secondCount, Is.EqualTo(0)); // Should not fire second time
        }

        [Test]
        public void NewsEvent_WithORRecipients_ShowsToMatchingCountries()
        {
            // Arrange
            var eventManager = new EventManager();
            var notifier = new TestEventNotifier();
            var evaluator = new EventEvaluator(eventManager, notifier);

            var newsEvent = new NewsEvent
            {
                Id = "acheson.2",
                Title = "Acheson Speech News",
                Recipients = new List<IEventTrigger>
                {
                    new OrTrigger
                    {
                        Triggers = new List<IEventTrigger>
                        {
                            new TagTrigger { Tag = "ROK" },
                            new TagTrigger { Tag = "DRK" },
                            new TagTrigger { Tag = "USSR" },
                            new TagTrigger { Tag = "JAP" },
                            new TagTrigger { Tag = "PRC" },
                        },
                    },
                },
                Options = new List<EventOption> { new EventOption { Name = "Interesting..." } },
            };
            eventManager.RegisterEvent(newsEvent);

            var worldState = CreateTestWorldState(new[] { "USA", "USSR", "ROK", "GBR" });
            var currentDate = new DateTime(1950, 1, 1);

            // Act
            evaluator.EvaluateAllEvents(worldState, currentDate, 1, "USSR", null!);

            // Assert - Should show only to USSR (player), not USA or GBR
            Assert.That(notifier.TriggeredEvents, Has.Count.EqualTo(1));
            Assert.That(notifier.TriggeredEvents[0].Context.CurrentCountryTag, Is.EqualTo("USSR"));
        }

        private WorldState CreateTestWorldState(string[] countryTags)
        {
            var worldState = new WorldState();
            var countries = new Dictionary<string, Country>();

            foreach (var tag in countryTags)
            {
                // Create a minimal test country
                var country = new Country();
                // Use reflection to set Tag (since it has private setter)
                var tagProperty = typeof(Country).GetProperty("Tag");
                tagProperty!.SetValue(country, tag);
                countries[tag] = country;
            }

            worldState.SetCountryTable(countries);
            return worldState;
        }
    }
}

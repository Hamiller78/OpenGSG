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
                // Inject deterministic random: first roll succeeds (0.001 < 0.0056)
                // second roll fails (0.9 > 0.0056), third succeeds
                Random = new DeterministicRandom(0.001, 0.9, 0.9, 0.002, 0.9),
            };
            eventManager.RegisterEvent(newsEvent);

            var worldState = CreateTestWorldState(new[] { "USA" });
            var currentDate = new DateTime(1961, 6, 1);

            // Act - run 5 evaluations
            var results = new List<bool>();
            for (int i = 0; i < 5; i++)
            {
                notifier.TriggeredEvents.Clear();
                evaluator.EvaluateAllEvents(worldState, currentDate, i, "USA", null!);
                results.Add(notifier.TriggeredEvents.Count > 0);
            }

            // Assert - deterministic results based on injected random values
            // With MTTH 180 days, probability = 1/180 ≈ 0.0056
            Assert.That(results[0], Is.True); // 0.001 < 0.0056 → fires
            Assert.That(results[1], Is.False); // 0.9 > 0.0056 → doesn't fire
            Assert.That(results[2], Is.False); // 0.9 > 0.0056 → doesn't fire
            Assert.That(results[3], Is.True); // 0.002 < 0.0056 → fires
            Assert.That(results[4], Is.False); // 0.9 > 0.0056 → doesn't fire
        }

        [Test]
        public void NewsEvent_WithMTTH_UsesCorrectProbability()
        {
            // Arrange
            var eventManager = new EventManager();
            var notifier = new TestEventNotifier();
            var evaluator = new EventEvaluator(eventManager, notifier);

            // Test with known seed for reproducible results
            var newsEvent = new NewsEvent
            {
                Id = "test.mtth",
                Title = "MTTH Test",
                MeanTimeToHappenDays = 100, // probability = 0.01
                Triggers = new List<IEventTrigger>
                {
                    new DateGreaterEqualTrigger { Date = new DateTime(1950, 1, 1) },
                },
                Options = new List<EventOption> { new EventOption { Name = "OK" } },
                Random = new SeededRandom(
                    12345
                ) // Fixed seed for reproducibility
                ,
            };
            eventManager.RegisterEvent(newsEvent);

            var worldState = CreateTestWorldState(new[] { "USA" });
            var currentDate = new DateTime(1950, 1, 1);

            // Act - run 1000 times with fixed seed
            int fireCount = 0;
            for (int i = 0; i < 1000; i++)
            {
                notifier.TriggeredEvents.Clear();
                evaluator.EvaluateAllEvents(worldState, currentDate, i, "USA", null!);

                if (notifier.TriggeredEvents.Count > 0)
                    fireCount++;
            }

            // Assert - with seed 12345, the random sequence produces exactly 8 fires
            // (empirically determined - any change to this indicates a bug or RNG change)
            Assert.That(fireCount, Is.EqualTo(8));
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

        [Test]
        public void NewsEvent_WithMTTH_RollsOnlyOncePerTick()
        {
            // Arrange
            var eventManager = new EventManager();
            var notifier = new TestEventNotifier();
            var evaluator = new EventEvaluator(eventManager, notifier);

            // News event with MTTH and multiple recipients
            var newsEvent = new NewsEvent
            {
                Id = "global.mtth",
                Title = "Global Event with Multiple Recipients",
                MeanTimeToHappenDays = 100, // probability = 0.01
                Triggers = new List<IEventTrigger>
                {
                    new DateGreaterEqualTrigger { Date = new DateTime(1950, 1, 1) },
                },
                Recipients = new List<IEventTrigger>
                {
                    new OrTrigger
                    {
                        Triggers = new List<IEventTrigger>
                        {
                            new TagTrigger { Tag = "USA" },
                            new TagTrigger { Tag = "USSR" },
                            new TagTrigger { Tag = "ROK" },
                        },
                    },
                },
                Options = new List<EventOption> { new EventOption { Name = "OK" } },
                // Deterministic random: tick 1 succeeds (0.005 < 0.01), tick 2 fails (0.9 > 0.01)
                Random = new DeterministicRandom(0.005, 0.9, 0.005, 0.9),
            };
            eventManager.RegisterEvent(newsEvent);

            var worldState = CreateTestWorldState(new[] { "USA", "USSR", "ROK", "GBR" });
            var currentDate = new DateTime(1950, 1, 1);

            // Act - Tick 1: MTTH should roll once and fire for player
            evaluator.EvaluateAllEvents(worldState, currentDate, 1, "USA", null!);
            int tick1Count = notifier.TriggeredEvents.Count;
            bool tick1Fired = tick1Count > 0;

            // Act - Tick 2: MTTH should roll once and NOT fire
            notifier.TriggeredEvents.Clear();
            evaluator.EvaluateAllEvents(worldState, currentDate, 2, "USA", null!);
            int tick2Count = notifier.TriggeredEvents.Count;
            bool tick2Fired = tick2Count > 0;

            // Act - Tick 3: Switch to USSR as player - should fire again
            notifier.TriggeredEvents.Clear();
            evaluator.EvaluateAllEvents(worldState, currentDate, 3, "USSR", null!);
            int tick3Count = notifier.TriggeredEvents.Count;
            bool tick3Fired = tick3Count > 0;
            string tick3RecipientTag =
                tick3Count > 0 ? notifier.TriggeredEvents[0].Context.CurrentCountryTag : "";

            // Act - Tick 4: Still as USSR - should not fire
            notifier.TriggeredEvents.Clear();
            evaluator.EvaluateAllEvents(worldState, currentDate, 4, "USSR", null!);
            int tick4Count = notifier.TriggeredEvents.Count;
            bool tick4Fired = tick4Count > 0;

            // Assert - All-or-nothing pattern proves MTTH rolled once per tick
            Assert.That(
                tick1Fired,
                Is.True,
                "Tick 1: MTTH roll succeeded (0.005 < 0.01), event should fire"
            );
            Assert.That(tick1Count, Is.EqualTo(1), "Tick 1: Event shown to player (USA) only once");

            Assert.That(
                tick2Fired,
                Is.False,
                "Tick 2: MTTH roll failed (0.9 > 0.01), event should not fire"
            );

            Assert.That(
                tick3Fired,
                Is.True,
                "Tick 3: MTTH roll succeeded (0.005 < 0.01), event should fire"
            );
            Assert.That(
                tick3RecipientTag,
                Is.EqualTo("USSR"),
                "Tick 3: Event shown to current player (USSR)"
            );
            Assert.That(tick3Count, Is.EqualTo(1), "Tick 3: Event shown to player only once");

            Assert.That(
                tick4Fired,
                Is.False,
                "Tick 4: MTTH roll failed (0.9 > 0.01), event should not fire"
            );
        }

        [Test]
        public void NewsEvent_WithMTTH_DoesNotRollPerRecipient()
        {
            // Arrange - This test proves MTTH is NOT rolled separately per recipient
            var eventManager = new EventManager();
            var notifier = new TestEventNotifier();
            var evaluator = new EventEvaluator(eventManager, notifier);

            // If MTTH were rolled per recipient, we'd need 3 random values (USA, USSR, ROK)
            // But with global roll, only 1 value is consumed per tick
            var newsEvent = new NewsEvent
            {
                Id = "single.roll",
                Title = "Single MTTH Roll Test",
                MeanTimeToHappenDays = 100, // probability = 0.01
                Recipients = new List<IEventTrigger>
                {
                    new OrTrigger
                    {
                        Triggers = new List<IEventTrigger>
                        {
                            new TagTrigger { Tag = "USA" },
                            new TagTrigger { Tag = "USSR" },
                            new TagTrigger { Tag = "ROK" },
                        },
                    },
                },
                Options = new List<EventOption> { new EventOption { Name = "OK" } },
                // Provide exactly 5 random values for 5 ticks
                // If per-recipient rolling happened, this would run out of values early
                Random = new DeterministicRandom(0.005, 0.9, 0.005, 0.9, 0.005),
            };
            eventManager.RegisterEvent(newsEvent);

            var worldState = CreateTestWorldState(new[] { "USA", "USSR", "ROK" });
            var currentDate = new DateTime(1950, 1, 1);

            // Act - Run 5 evaluations, each should consume exactly 1 random value
            var results = new List<bool>();
            for (int i = 1; i <= 5; i++)
            {
                notifier.TriggeredEvents.Clear();
                evaluator.EvaluateAllEvents(worldState, currentDate, i, "USA", null!);
                results.Add(notifier.TriggeredEvents.Count > 0);
            }

            // Assert - Pattern matches our 5 deterministic values (not 15 if per-recipient)
            Assert.That(results[0], Is.True, "Tick 1: Value 1 (0.005) → fires");
            Assert.That(results[1], Is.False, "Tick 2: Value 2 (0.9) → doesn't fire");
            Assert.That(results[2], Is.True, "Tick 3: Value 3 (0.005) → fires");
            Assert.That(results[3], Is.False, "Tick 4: Value 4 (0.9) → doesn't fire");
            Assert.That(results[4], Is.True, "Tick 5: Value 5 (0.005) → fires");

            // If MTTH were per-recipient (3 countries), we'd need 15 values and see different pattern
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

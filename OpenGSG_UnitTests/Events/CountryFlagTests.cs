using OpenGSGLibrary.Events;
using OpenGSGLibrary.GameDataManager;

namespace OpenGSG_UnitTests.Events
{
    [TestFixture]
    public class CountryFlagTests
    {
        [Test]
        public void SetCountryFlag_SetsFlag()
        {
            // Arrange
            var country = new Country();
            typeof(Country).GetProperty("Tag")!.SetValue(country, "USA");

            var worldState = new WorldState();
            worldState.SetCountryTable(new Dictionary<string, Country> { { "USA", country } });

            var effect = new SetCountryFlagEffect { FlagName = "test_flag" };
            var context = new EventEvaluationContext
            {
                WorldState = worldState,
                CurrentCountryTag = "USA",
            };

            // Act
            effect.Execute(context);

            // Assert
            Assert.That(country.HasFlag("test_flag"), Is.True);
        }

        [Test]
        public void HasCountryFlag_ReturnsTrueWhenFlagSet()
        {
            // Arrange
            var country = new Country();
            typeof(Country).GetProperty("Tag")!.SetValue(country, "USA");
            country.SetFlag("test_flag");

            var worldState = new WorldState();
            worldState.SetCountryTable(new Dictionary<string, Country> { { "USA", country } });

            var trigger = new HasCountryFlagTrigger { FlagName = "test_flag" };
            var context = new EventEvaluationContext
            {
                WorldState = worldState,
                CurrentCountryTag = "USA",
            };

            // Act
            bool result = trigger.Evaluate(context);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasCountryFlag_ReturnsFalseWhenFlagNotSet()
        {
            // Arrange
            var country = new Country();
            typeof(Country).GetProperty("Tag")!.SetValue(country, "USA");

            var worldState = new WorldState();
            worldState.SetCountryTable(new Dictionary<string, Country> { { "USA", country } });

            var trigger = new HasCountryFlagTrigger { FlagName = "test_flag" };
            var context = new EventEvaluationContext
            {
                WorldState = worldState,
                CurrentCountryTag = "USA",
            };

            // Act
            bool result = trigger.Evaluate(context);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ClearCountryFlag_RemovesFlag()
        {
            // Arrange
            var country = new Country();
            typeof(Country).GetProperty("Tag")!.SetValue(country, "USA");
            country.SetFlag("test_flag");

            var worldState = new WorldState();
            worldState.SetCountryTable(new Dictionary<string, Country> { { "USA", country } });

            var effect = new ClearCountryFlagEffect { FlagName = "test_flag" };
            var context = new EventEvaluationContext
            {
                WorldState = worldState,
                CurrentCountryTag = "USA",
            };

            // Act
            effect.Execute(context);

            // Assert
            Assert.That(country.HasFlag("test_flag"), Is.False);
        }

        [Test]
        public void CountryScopeTrigger_EvaluatesFlagInCorrectCountryContext()
        {
            // Arrange
            var usa = new Country();
            typeof(Country).GetProperty("Tag")!.SetValue(usa, "USA");
            usa.SetFlag("test_flag");

            var ussr = new Country();
            typeof(Country).GetProperty("Tag")!.SetValue(ussr, "USSR");

            var worldState = new WorldState();
            worldState.SetCountryTable(
                new Dictionary<string, Country> { { "USA", usa }, { "USSR", ussr } }
            );

            // Create a scoped trigger: USA = { has_country_flag = test_flag }
            var scopedTrigger = new CountryScopeTrigger
            {
                CountryTag = "USA",
                ScopedTriggers = new List<IEventTrigger>
                {
                    new HasCountryFlagTrigger { FlagName = "test_flag" },
                },
            };

            // Context is for USSR (but trigger checks USA's flags)
            var context = new EventEvaluationContext
            {
                WorldState = worldState,
                CurrentCountryTag = "USSR",
            };

            // Act
            bool result = scopedTrigger.Evaluate(context);

            // Assert
            Assert.That(result, Is.True, "Should evaluate USA's flag even when context is USSR");
        }

        [Test]
        public void NewsEvent_WithScopedFlagTrigger_FiresCorrectly()
        {
            // Arrange
            var eventManager = new EventManager();
            var notifier = new TestEventNotifier();
            var evaluator = new EventEvaluator(eventManager, notifier);

            var usa = new Country();
            typeof(Country).GetProperty("Tag")!.SetValue(usa, "USA");
            usa.SetFlag("star_trek_aired");

            var worldState = new WorldState();
            worldState.SetCountryTable(new Dictionary<string, Country> { { "USA", usa } });

            var newsEvent = new NewsEvent
            {
                Id = "star_trek.2",
                Title = "Chekov Appears",
                Triggers = new List<IEventTrigger>
                {
                    new CountryScopeTrigger
                    {
                        CountryTag = "USA",
                        ScopedTriggers = new List<IEventTrigger>
                        {
                            new HasCountryFlagTrigger { FlagName = "star_trek_aired" },
                        },
                    },
                    new DateEqualTrigger
                    {
                        Date = new DateTime(1967, 9, 15, 0, 0, 0, DateTimeKind.Unspecified),
                    },
                },
                Recipients = new List<IEventTrigger> { new TagTrigger { Tag = "USA" } },
                Options = new List<EventOption> { new EventOption { Name = "OK" } },
            };
            eventManager.RegisterEvent(newsEvent);

            var currentDate = new DateTime(1967, 9, 15, 0, 0, 0, DateTimeKind.Unspecified);

            // Act
            evaluator.EvaluateAllEvents(worldState, currentDate, 1, "USA", null!);

            // Assert
            Assert.That(notifier.TriggeredEvents, Has.Count.EqualTo(1));
            Assert.That(notifier.TriggeredEvents[0].Event.Id, Is.EqualTo("star_trek.2"));
        }

        [Test]
        public void CountryScopeEffect_SetsFlagOnCorrectCountry()
        {
            // Arrange
            var usa = new Country();
            typeof(Country).GetProperty("Tag")!.SetValue(usa, "USA");

            var ussr = new Country();
            typeof(Country).GetProperty("Tag")!.SetValue(ussr, "USSR");

            var worldState = new WorldState();
            worldState.SetCountryTable(
                new Dictionary<string, Country> { { "USA", usa }, { "USSR", ussr } }
            );

            // Create scoped effect: USSR = { set_country_flag = chekov_appeared }
            var scopedEffect = new CountryScopeEffect
            {
                CountryTag = "USSR",
                InnerEffects = new List<IEventEffect>
                {
                    new SetCountryFlagEffect { FlagName = "chekov_appeared" },
                },
            };

            // Context is for USA (but effect should set flag on USSR)
            var context = new EventEvaluationContext
            {
                WorldState = worldState,
                CurrentCountryTag = "USA",
            };

            // Act
            scopedEffect.Execute(context);

            // Assert
            Assert.That(usa.HasFlag("chekov_appeared"), Is.False, "USA should not have the flag");
            Assert.That(ussr.HasFlag("chekov_appeared"), Is.True, "USSR should have the flag");
        }
    }
}

using OpenGSGLibrary.Events;
using OpenGSGLibrary.GameDataManager;

namespace OpenGSG_UnitTests.Events
{
    [TestFixture]
    public class ConditionalEffectTests
    {
        [Test]
        public void RandomTrigger_LessThan_EvaluatesCorrectly()
        {
            // Arrange
            var trigger = new RandomTrigger
            {
                Threshold = 0.5,
                Operator = "<",
                Random = new DeterministicRandom(0.3, 0.7, 0.5),
            };

            var context = new EventEvaluationContext();

            // Act & Assert
            Assert.That(trigger.Evaluate(context), Is.True); // 0.3 < 0.5
            Assert.That(trigger.Evaluate(context), Is.False); // 0.7 < 0.5
            Assert.That(trigger.Evaluate(context), Is.False); // 0.5 < 0.5
        }

        [Test]
        public void ConditionalEffect_ExecutesThenBranch_WhenConditionTrue()
        {
            // Arrange
            var usa = new Country();
            typeof(Country).GetProperty("Tag")!.SetValue(usa, "USA");

            var worldState = new WorldState();
            worldState.SetCountryTable(new Dictionary<string, Country> { { "USA", usa } });

            var conditional = new ConditionalEffect
            {
                Condition = new List<IEventTrigger>
                {
                    new RandomTrigger
                    {
                        Threshold = 0.5,
                        Operator = "<",
                        Random = new DeterministicRandom(
                            0.3
                        ) // Always true
                        ,
                    },
                },
                ThenEffects = new List<IEventEffect>
                {
                    new SetCountryFlagEffect { FlagName = "then_executed" },
                },
                ElseEffects = new List<IEventEffect>
                {
                    new SetCountryFlagEffect { FlagName = "else_executed" },
                },
            };

            var context = new EventEvaluationContext
            {
                WorldState = worldState,
                CurrentCountryTag = "USA",
            };

            // Act
            conditional.Execute(context);

            // Assert
            Assert.That(usa.HasFlag("then_executed"), Is.True);
            Assert.That(usa.HasFlag("else_executed"), Is.False);
        }

        [Test]
        public void ConditionalEffect_ExecutesElseBranch_WhenConditionFalse()
        {
            // Arrange
            var usa = new Country();
            typeof(Country).GetProperty("Tag")!.SetValue(usa, "USA");

            var worldState = new WorldState();
            worldState.SetCountryTable(new Dictionary<string, Country> { { "USA", usa } });

            var conditional = new ConditionalEffect
            {
                Condition = new List<IEventTrigger>
                {
                    new RandomTrigger
                    {
                        Threshold = 0.5,
                        Operator = "<",
                        Random = new DeterministicRandom(
                            0.7
                        ) // Always false
                        ,
                    },
                },
                ThenEffects = new List<IEventEffect>
                {
                    new SetCountryFlagEffect { FlagName = "then_executed" },
                },
                ElseEffects = new List<IEventEffect>
                {
                    new SetCountryFlagEffect { FlagName = "else_executed" },
                },
            };

            var context = new EventEvaluationContext
            {
                WorldState = worldState,
                CurrentCountryTag = "USA",
            };

            // Act
            conditional.Execute(context);

            // Assert
            Assert.That(usa.HasFlag("then_executed"), Is.False);
            Assert.That(usa.HasFlag("else_executed"), Is.True);
        }
    }
}

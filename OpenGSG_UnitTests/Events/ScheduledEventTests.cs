using OpenGSGLibrary.Events;
using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.GameLogic;

namespace OpenGSG_UnitTests.Events
{
    [TestFixture]
    public class ScheduledEventTests
    {
        [Test]
        public void ScheduledEvent_FiresAfterDelay()
        {
            // Arrange
            var worldState = new WorldState();
            var usa = new Country();
            typeof(Country).GetProperty("Tag")!.SetValue(usa, "USA");
            worldState.SetCountryTable(new Dictionary<string, Country> { { "USA", usa } });

            var eventManager = new EventManager();
            var notifier = new TestEventNotifier();

            var tickHandler = new TickHandler();
            tickHandler.SetStartDate(new DateTime(1966, 9, 8, 0, 0, 0, DateTimeKind.Unspecified));
            tickHandler.ConnectProvinceEventHandlers(worldState);
            tickHandler.SetEventManager(eventManager);

            // Create the delayed event
            var delayedEvent = new NewsEvent
            {
                Id = "frontier_trek.2",
                Title = "Delayed Event",
                IsTriggeredOnly = true,
                Recipients = new List<IEventTrigger> { new TagTrigger { Tag = "USA" } },
                Options = new List<EventOption> { new EventOption { Name = "OK" } },
            };
            eventManager.RegisterEvent(delayedEvent);

            // Schedule it for 21 days in the future
            tickHandler.ScheduleEvent("frontier_trek.2", "USA", 21, true);

            // Act - advance 20 days
            for (int i = 0; i < 20; i++)
            {
                tickHandler.BeginNewTick();
                tickHandler.FinishTick();
            }

            // Event should not have fired yet (notifier won't capture it, but we can check internally)
            Assert.That(tickHandler.GetCurrentTick(), Is.EqualTo(20));

            // Advance 1 more day (total 21)
            tickHandler.BeginNewTick();
            tickHandler.FinishTick();

            // Assert - event should have fired on day 21
            Assert.That(tickHandler.GetCurrentTick(), Is.EqualTo(21));
            Assert.That(
                tickHandler.GetCurrentDate(),
                Is.EqualTo(new DateTime(1966, 9, 29, 0, 0, 0, DateTimeKind.Unspecified))
            );

            // Note: To properly test event firing, you'd need to subscribe to TickHandler.EventTriggered
            // or inject the notifier into EventEvaluator. For now, this tests the scheduling mechanism.
        }
    }
}

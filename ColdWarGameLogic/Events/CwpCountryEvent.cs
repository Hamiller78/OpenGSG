using OpenGSGLibrary.Events;

namespace ColdWarGameLogic.Events
{
    /// <summary>
    /// Cold War specific country event.
    /// Can be extended with game-specific logic if needed.
    /// </summary>
    public class CwpCountryEvent : CountryEvent { }

    /// <summary>
    /// Cold War specific news event.
    /// Can be extended with game-specific logic if needed.
    /// </summary>
    public class CwpNewsEvent : NewsEvent { }
}

using System;

namespace OpenGSGLibrary.GameLogic
{
    /// <summary>
    /// Helper class passed with the TickDone event.
    /// Extra argument is the current tick number.
    /// </summary>
    public class TickEventArgs(int tickPar) : EventArgs
    {
        public int Tick { get; set; } = tickPar;
    }
}

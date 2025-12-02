namespace OpenGSGLibrary.Military
{
    /// <summary>
    /// Base interface for movable military units. Concrete implementations must provide a name and a terrain speed.
    /// </summary>
    public abstract class Unit
    {
        /// <summary>
        /// Returns display name for the unit.
        /// </summary>
        public abstract string GetName();

        /// <summary>
        /// Returns movement speed for a given terrain type.
        /// </summary>
        /// <param name="terrainType">Terrain type key.</param>
        public abstract double GetSpeed(string terrainType);
    }
}

using System.Drawing;
using OpenGSGLibrary.GameDataManager;

namespace OpenGSGLibrary.WorldMap
{
    /// <summary>
    /// Base class for making maps for different view modes (e.g. political, economical).
    /// Each derived class should correspond to a map mode in the game.
    /// Maps are stored as System.Drawing.Image objects.
    /// </summary>
    public abstract class ModeMapMaker
    {
        protected LayerBitmap _sourceMap;

        protected ModeMapMaker(LayerBitmap sourceMap)
        {
            _sourceMap = sourceMap;
        }

        /// <summary>
        /// Generate an Image for the provided world state using the source map.
        /// </summary>
        public abstract Image MakeMap(WorldState sourceState);
    }
}

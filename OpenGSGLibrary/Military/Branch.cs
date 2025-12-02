using System;

namespace OpenGSGLibrary.Military
{
    /// <summary>
    /// Represents a military branch type (Army, Navy, Airforce, Space) and helpers to parse from string.
    /// </summary>
    public class Branch
    {
        public enum Type
        {
            Army,
            Navy,
            Airforce,
            Space
        }

        private Type type_;

        public Branch(Type type)
        {
            type_ = type;
        }

        public Branch(string typeName)
        {
            switch (typeName.ToLowerInvariant())
            {
                case "army":
                    type_ = Type.Army;
                    break;
                case "navy":
                    type_ = Type.Navy;
                    break;
                case "airforce":
                    type_ = Type.Airforce;
                    break;
                case "space":
                    type_ = Type.Space;
                    break;
                default:
                    throw new ApplicationException("Unknown type for military branch: " + typeName);
            }
        }

        /// <summary>
        /// Gets the branch type enum value.
        /// </summary>
        public Type GetBranchType() => type_;
    }
}

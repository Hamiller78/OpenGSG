using System;

namespace Military
{
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

        public Type GetBranchType() => type_;
    }
}

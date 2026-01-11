using OpenGSGLibrary.Diplomacy;

namespace ColdWarGameLogic.Diplomacy
{
    /// <summary>
    /// Represents a guarantee from one country to another.
    /// The guarantor commits to defend the guaranteed country if attacked.
    /// </summary>
    public class GuaranteeRelation : DiplomaticRelation
    {
        public override string RelationType => "guarantee";
    }
}

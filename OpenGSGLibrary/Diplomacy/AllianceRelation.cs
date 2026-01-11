namespace OpenGSGLibrary.Diplomacy
{
    /// <summary>
    /// Represents an alliance between two countries.
    /// Typically bidirectional.
    /// </summary>
    public class AllianceRelation : DiplomaticRelation
    {
        public override string RelationType => "alliance";
    }
}

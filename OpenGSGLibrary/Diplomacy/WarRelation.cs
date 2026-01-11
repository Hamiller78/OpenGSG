namespace OpenGSGLibrary.Diplomacy
{
    /// <summary>
    /// Represents a state of war between two countries.
    /// War is typically bidirectional (if A is at war with B, B is at war with A).
    /// </summary>
    public class WarRelation : DiplomaticRelation
    {
        public override string RelationType => "war";

        /// <summary>
        /// Who declared the war (optional, for historical record).
        /// </summary>
        public string? Aggressor { get; set; }
    }
}

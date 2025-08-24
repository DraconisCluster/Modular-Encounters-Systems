using System.Xml;
using System.Xml.Serialization;

namespace ModularEncounterSystems.Data.Scripts.ModularEncountersSystems.Entities.Threat.Core {

    [XmlRoot("Category")]
    public class BlockCategoryThreat : ThreatDefinition, System.IEquatable<BlockCategoryThreat>
    {
        public override string GetId() => $"{Category}";

        [XmlText]
        public string Category { get; set; } = string.Empty;
        public static bool operator ==(BlockCategoryThreat left, BlockCategoryThreat right) => left.Equals(right);
        public static bool operator !=(BlockCategoryThreat left, BlockCategoryThreat right) => !(left == right);
        public override bool Equals(object obj) => Equals(obj is ThreatDefinition ? (obj as ThreatDefinition) : null);
        public bool Equals(BlockCategoryThreat other) => Equals(other);
        public override bool Equals(ThreatDefinition other) => (other != null) && GetId() == other.GetId();
        public override int GetHashCode() => GetId().GetHashCode();
    }
}
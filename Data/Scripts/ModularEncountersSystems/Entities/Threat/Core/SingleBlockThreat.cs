using System.Xml;
using System.Xml.Serialization;

namespace ModularEncounterSystems.Data.Scripts.ModularEncountersSystems.Entities.Threat.Core
{
    [XmlRoot("Block")]
    public class SingleBlockThreat : ThreatDefinition, System.IEquatable<SingleBlockThreat>
    {       
        public override string GetId() => $"{BlockType}/{BlockSubType}";
        
        [XmlAttribute("Type")]
        public string BlockType { get; set; } = string.Empty;

        [XmlAttribute("SubType")]
        public string BlockSubType { get; set; } = string.Empty;
        public override bool Equals(object obj) => Equals(obj is ThreatDefinition ? (obj as ThreatDefinition) : null);
        public override bool Equals(ThreatDefinition other) => other != null && GetId() == other.GetId();
        public bool Equals(SingleBlockThreat other) => Equals(other);
        public override int GetHashCode() => GetId().GetHashCode();
        public static bool operator ==(SingleBlockThreat left, SingleBlockThreat right) => left.Equals(right);
        public static bool operator !=(SingleBlockThreat left, SingleBlockThreat right) => !(left == right);
    }
}
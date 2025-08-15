using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ModularEncounterSystems.Data.Scripts.ModularEncountersSystems.Entities.Threat.Core {

    [XmlRoot("Category")]
    public class BlockCategoryThreat : ThreatDefinition, System.IEquatable<BlockCategoryThreat>
    {
        public override string GetId() => $"{Category}";

        [XmlText]
        public string Category { get; set; } = string.Empty;
        public override bool Equals(object obj) => Equals(obj as BlockCategoryThreat);
        public bool Equals(BlockCategoryThreat other) => (other != null) && GetId() == other.GetId();
        public override bool Equals(ThreatDefinition other)=>(other != null) && GetId() == other.GetId();
        public override int GetHashCode() => GetId().GetHashCode();
        public override ThreatDefinition ToDefinition()
        {
            return new BlockCategoryThreat
            {
                Category = Category,
                Threat = Threat,
                Multiplier = Multiplier,
                MultiplierThreshold = MultiplierThreshold,
                FullVolumeThreat = FullVolumeThreat
            };
        }
        public static bool operator ==(BlockCategoryThreat left, BlockCategoryThreat right)
        {
            return EqualityComparer<BlockCategoryThreat>.Default.Equals(left, right);
        }

        public static bool operator !=(BlockCategoryThreat left, BlockCategoryThreat right)
        {
            return !(left == right);
        }
    }
}
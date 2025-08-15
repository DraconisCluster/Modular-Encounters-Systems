using ModularEncounterSystems.Data.Scripts.ModularEncountersSystems.Entities.Threat.Core;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ModularEncounterSystems.Data.Scripts.ModularEncountersSystems.Entities.Threat.Profile
{

    [XmlRoot("ConditionalThreatProfile")]
    public class ConditionalThreatProfile
    {
        public enum ThreatProfileIdentifierType
        {
            Planet,
            Sector,
            Grid,
            FactionName
        }
        public enum ThreatProfileConditionType
        {
            Near,
            Is,
            Contains
        }

        [XmlAttribute("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute("Identifier")]
        public ThreatProfileIdentifierType Identifier;

        [XmlAttribute("Condition")]
        public ThreatProfileConditionType Condition;

        [XmlAttribute("Value")]
        public string Value;

        [XmlArray("BlockThreat")]
        [XmlArrayItem("Block")]
        public List<SingleBlockThreat> SingleBlockThreatEntries;

        [XmlArray("CategoryThreat")]
        [XmlArrayItem("Category")]
        public List<BlockCategoryThreat> BlockCategoryThreatEntries;

        [XmlElement("GridTypeThreatMultipliers")]
        public GridTypeThreatMultiplier GridTypeMultipliers;

        [XmlElement("PowerOutputMultipliers")]
        public GridTypeThreatMultiplier GridPowerOutputMultipliers;

        [XmlElement("BoundingBoxSizeMultipliers")]
        public GridTypeThreatMultiplier BoundingBoxSizeMultipliers;

        [XmlElement("ThreatPerBlockMultipliers")]
        public GridTypeThreatMultiplier ThreatPerBlockMultipliers;

    }
}
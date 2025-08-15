
using ModularEncounterSystems.Data.Scripts.ModularEncountersSystems.Entities.Threat.Core;
using ModularEncounterSystems.Data.Scripts.ModularEncountersSystems.Entities.Threat.Profile;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ModularEncountersSystems.Entities.Threat
{

    [XmlRoot("ThreatSettings")]
    public class ConfigThreatState
    {
        [XmlAttribute("ThreatModVersion")]
        public string ThreatModVersion { get; set; } = "1.0.0";

        [XmlArray("BlockThreat")]
        [XmlArrayItem("Block")]
        public List<SingleBlockThreat> SingleBlockThreatEntries { get; set; }
            = new List<SingleBlockThreat>();

        [XmlArray("CategoryThreat")]
        [XmlArrayItem("Category")]
        public List<BlockCategoryThreat> BlockCategoryThreatEntries { get; set; }
            = new List<BlockCategoryThreat>();

        [XmlArray("ThreatProfiles")]
        [XmlArrayItem("ConditionalThreatProfile")]
        public List<ConditionalThreatProfile> ThreatProfiles { get; set; }
            = new List<ConditionalThreatProfile>();

        [XmlElement("GridTypeThreatMultiplier")]
        public GridTypeThreatMultiplier GridTypeMultipliers { get; set; }
            = new GridTypeThreatMultiplier();

        [XmlElement("PowerOutputMultipliers")]
        public GridTypeThreatMultiplier GridPowerOutputMultipliers { get; set; }
            = new GridTypeThreatMultiplier();

        [XmlElement("ThreatPerBlockMultiplier")]
        public GridTypeThreatMultiplier ThreatPerBlockMultipliers { get; set; }
            = new GridTypeThreatMultiplier
            {
                SmallGridMultiplier = 0.01f,
                LargeGridMultiplier = 0.01f,
                StationMultiplier = 0.01f
            };

        [XmlElement("BoundingBoxSizeMultiplier")]
        public GridTypeThreatMultiplier BoundingBoxSizeMultipliers { get; set; }
            = new GridTypeThreatMultiplier
            {
                SmallGridMultiplier = 0.25f,
                LargeGridMultiplier = 0.25f,
                StationMultiplier = 0.25f
            };

        public ConfigThreatState copy()
        {
            return new ConfigThreatState
            {
                ThreatModVersion = this.ThreatModVersion,
                SingleBlockThreatEntries = this.SingleBlockThreatEntries,
                BlockCategoryThreatEntries = this.BlockCategoryThreatEntries,
                GridTypeMultipliers = this.GridTypeMultipliers,
                GridPowerOutputMultipliers = this.GridPowerOutputMultipliers,
                BoundingBoxSizeMultipliers = this.BoundingBoxSizeMultipliers,
                ThreatPerBlockMultipliers = this.ThreatPerBlockMultipliers,
                ThreatProfiles = this.ThreatProfiles
            };
        }
    }
}
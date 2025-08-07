using System.Security.Permissions;
using System.Xml;
using System.Xml.Serialization;

namespace ModularEncountersSystems.Configuration {

    [XmlRoot("Category")]
    public class BlockCategoryThreat
    {
        [XmlText]
        public string Category;

        [XmlAttribute("Threat")]
        public float Threat { get; set; } = 0.0f;

        [XmlAttribute("Multiplier")]
        public float Multiplier { get; set; } = 1.0f;

        [XmlAttribute("MultiplierThreshold")]
        public int MultiplierThreshold { get; set; } = 2;

        [XmlAttribute("FullVolumeThreat")]
        public float FullVolumeThreat { get; set; } = 0.0f;

        public ThreatDefinition ToDefinition()
        {
            return new ThreatDefinition
            {
                Threat = Threat,
                Multiplier = Multiplier,
                MultiplierThreshold = MultiplierThreshold,
                FullVolumeThreat = FullVolumeThreat
            };
        }
    }
}
using System.Xml;
using System.Xml.Serialization;

namespace ModularEncountersSystems.Configuration {
   
    [XmlRoot("Block")]
    public class SingleBlockThreat
    {
        [XmlAttribute("Type")]
        public string BlockType { get; set; } = "*";

        [XmlAttribute("SubType")]
        public string BlockSubType { get; set; } = "*";

        [XmlAttribute("Threat")]
        public double Threat { get; set; } = 0;

        [XmlAttribute("Multiplier")]
        public double Multiplier { get; set; }  = 1.0;

        [XmlAttribute("MultiplierThreshold")]
        public int MultiplierThreshold { get; set; }  = 2;

        [XmlAttribute("FullVolumeThreat")]
        public double FullVolumeThreat { get; set; } = 0.0;

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
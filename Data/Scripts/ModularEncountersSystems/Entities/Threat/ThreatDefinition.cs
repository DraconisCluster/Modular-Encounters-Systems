using System;

namespace ModularEncountersSystems.Configuration {
    public class ThreatDefinition : IEquatable<ThreatDefinition>
    {
        public double Threat;
        public int MultiplierThreshold;
        public double Multiplier;        
        public double FullVolumeThreat;

        public override bool Equals(object obj)
        {
            return Equals(obj as ThreatDefinition);
        }

        public bool Equals(ThreatDefinition other)
        {
            return !ReferenceEquals(other, null) &&
                   Threat == other.Threat &&
                   MultiplierThreshold == other.MultiplierThreshold &&
                   Multiplier == other.Multiplier &&
                   FullVolumeThreat == other.FullVolumeThreat;
        }

        public override int GetHashCode()
        {
            int hashCode = -96610310;
            hashCode = hashCode * -1521134295 + Threat.GetHashCode();
            hashCode = hashCode * -1521134295 + MultiplierThreshold.GetHashCode();
            hashCode = hashCode * -1521134295 + Multiplier.GetHashCode();
            hashCode = hashCode * -1521134295 + FullVolumeThreat.GetHashCode();
            return hashCode;
        }
    }
}
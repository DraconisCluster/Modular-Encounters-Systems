using ModularEncountersSystems.Logging;
using System;


namespace ModularEncounterSystems.Data.Scripts.ModularEncountersSystems.Entities.Threat.Util
{
    public class TSELogger : TLogInterface
    {
        private TSELogger Default = new TSELogger();
        private static bool debug = false;
        public static bool SetDebug
        {
            get
            {
                return debug;
            }
            set
            {
                debug = value;
            }
        }
        public void Debug(string message)
        {
            if (!debug) return;
            SpawnLogger.Write(message, SpawnerDebugEnum.Threat, debug);
        }
        public void Error(string message)
        {
            SpawnLogger.Write(message, SpawnerDebugEnum.Threat, true);
        }

        public void Info(string message)
        {
            SpawnLogger.Write(message, SpawnerDebugEnum.Threat);
        }

        public void Warn(string message)
        {
            SpawnLogger.Write(message, SpawnerDebugEnum.Threat);
        }
    }
}

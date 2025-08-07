using ModularEncountersSystems.Core;
using ModularEncountersSystems.Logging;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ModularEncountersSystems.Configuration {

    //General

    [XmlRoot("ThreatSettings")]
    public class ConfigThreat {
    public string ModVersion { get; set; }

    [XmlArray("BlockThreat")]
    [XmlArrayItem("Block")]
    public List<SingleBlockThreat> SingleBlockThreatEntries;

    [XmlArray("CategoryThreat")]
    [XmlArrayItem("Category")]
    public List<BlockCategoryThreat> BlockCategoryThreatEntries;

    [XmlIgnore]
    public Dictionary<string, ThreatDefinition> SingleBlockThreatDefinitions => SingleBlockThreatEntries
                            .Where(e => !string.IsNullOrWhiteSpace(e.BlockType))
                            .ToDictionary(e => e.BlockType, e => e.ToDefinition());

    [XmlIgnore]
    public Dictionary<string, ThreatDefinition> BlockCategoryThreatDefinitions => BlockCategoryThreatEntries
                .Where(e => !string.IsNullOrWhiteSpace(e.Category))
                .ToDictionary(e => e.Category, e => e.ToDefinition());


    [XmlElementAttribute("GridTypeThreatMultipliers")]
    public GridTypeThreatMultiplier GridTypeMultipliers;

    [XmlElementAttribute("PowerOutputMultipliers")]
    public GridTypeThreatMultiplier GridPowerOutputMultipliers;

    [XmlElementAttribute("BoundingBoxSizeMultipliers")]
    public GridTypeThreatMultiplier BoundingBoxSizeMultipliers;

    [XmlElementAttribute("ThreatPerBlockMultipliers")]
    public GridTypeThreatMultiplier ThreatPerBlockMultipliers;

    [XmlIgnore]
    public bool ConfigLoaded = false;

        [XmlIgnore]
    public Dictionary<string, Func<string, object, bool>> EditorReference;

        public ConfigThreat()
        {

            ModVersion = MES_SessionCore.ModVersion;

            SingleBlockThreatEntries = new List<SingleBlockThreat>();
            BlockCategoryThreatEntries = new List<BlockCategoryThreat>();

            GridTypeMultipliers = new GridTypeThreatMultiplier();
            GridPowerOutputMultipliers = new GridTypeThreatMultiplier();

            BoundingBoxSizeMultipliers = new GridTypeThreatMultiplier()
            {
                SmallGridMultiplier = 0.25f,
                LargeGridMultiplier = 0.25f,
                StationMultiplier = 0.25f
            };

            ThreatPerBlockMultipliers = new GridTypeThreatMultiplier()
            {
                SmallGridMultiplier = 0.01f,
                LargeGridMultiplier = 0.01f,
                StationMultiplier = 0.01f
            };

            ConfigLoaded = false;

            EditorReference = new Dictionary<string, Func<string, object, bool>> 
            {

				
                //{"ThreatPerBlockMultiplier", (s, o) => EditorTools.SetCommandValueDouble(s, ref ThreatPerBlockMultiplier) }

            };

		}

        public ConfigThreat LoadSettings(string phase)
        {
            if (MyAPIGateway.Utilities.FileExistsInWorldStorage("Config-Threat.xml", typeof(ConfigThreat)))
            {
                try
                {
                    ConfigThreat config = null;
                    var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage("Config-Threat.xml", typeof(ConfigThreat));

                    string xmlText = reader.ReadToEnd();

                    if (xmlText != null)
                    {
                        config = MyAPIGateway.Utilities.SerializeFromXML<ConfigThreat>(xmlText);
                        if (config != null)
                        {
                            config.ConfigLoaded = true;
                            SpawnLogger.Write("Loaded Existing Settings from Config-Threat.xml. Phase: " + phase, SpawnerDebugEnum.Startup, true);
                            return config;
                        }
                    }
                    else
                    {
                        SpawnLogger.Write("ERROR loading Config-Threat.xml:Opening file returned null result. ", SpawnerDebugEnum.Startup, true);
                    }
                }
                catch (Exception exc)
                {
                    SpawnLogger.Write("ERROR loading Config-Threat.xml: " + exc, SpawnerDebugEnum.Error, true);
                }
            }

            var defaultSettings = new ConfigThreat();
            try
            {
                using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-Threat.xml", typeof(ConfigThreat)))
                    writer.Write(MyAPIGateway.Utilities.SerializeToXML(defaultSettings));
            }
            catch (Exception exc)
            {
                SpawnLogger.Write("ERROR creating Config-Threat.xml: " + exc, SpawnerDebugEnum.Error, true);
            }

            return defaultSettings;
        }


        public string SaveSettings()
        {            
            try
            {
                using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-Threat.xml", typeof(ConfigThreat)))
                    writer.Write(MyAPIGateway.Utilities.SerializeToXML(this));
                return "Settings Updated Successfully.";
            }
            catch
            {
                return "Settings Changed, But Could Not Be Saved.";
            }
        }

        public string EditFields(string receivedCommand)
        {
            var split = receivedCommand.Split('.');
            if (split.Length < 5) return "Invalid command.";
            Func<string, object, bool> reference;
            if (!EditorReference.TryGetValue(split[3], out reference)) return $"Field {split[3]} not found.";
            if (!reference.Invoke(receivedCommand, null)) return $"Invalid value for {split[3]}.";
            return SaveSettings();
        }

        


    }
}
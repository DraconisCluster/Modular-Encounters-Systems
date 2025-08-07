
using ModularEncountersSystems.Configuration;
using ModularEncountersSystems.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game;
using VRageMath;

namespace ModularEncountersSystems.Entities { 

    public class ThreatEvaluator
    {
        private float DEFAULT_THREAT = 1.0f;
        private bool config_initialized = false;
        private GridEntity _grid;
        private ConfigThreat _currentThreatSettings;

        
        public ThreatEvaluator(GridEntity grid) {
            _grid = grid;
            _currentThreatSettings = Settings.Threat;
            if (_currentThreatSettings != null) config_initialized = true;
        }

        public float evaluate()
        {

            if (!config_initialized) return DEFAULT_THREAT;
            float result = 0;

            HashSet<long> evaluatedBlockIDs = new HashSet<long>();          
            result += evaluateSingleBlocks(evaluatedBlockIDs);
            result += evaluateBlockCategories(evaluatedBlockIDs);


            var type = _grid.CubeGrid.GridSizeEnum;


            if (_grid.CubeGrid.IsStatic)
            {
                result += (float)(_grid.AllBlocks.Count * _currentThreatSettings.ThreatPerBlockMultipliers.StationMultiplier);

                result += ((float)(Vector3D.Distance(_grid.CubeGrid.WorldAABB.Min, _grid.CubeGrid.WorldAABB.Max)
               * _currentThreatSettings.BoundingBoxSizeMultipliers.StationMultiplier));

                result *= (float)_currentThreatSettings.GridTypeMultipliers.StationMultiplier;
            }

            else
            {


                if (type == MyCubeSize.Large)
                {
                    result += (float)(_grid.AllBlocks.Count * _currentThreatSettings.ThreatPerBlockMultipliers.LargeGridMultiplier);

                    result += ((float)(Vector3D.Distance(_grid.CubeGrid.WorldAABB.Min, _grid.CubeGrid.WorldAABB.Max)
                   * _currentThreatSettings.BoundingBoxSizeMultipliers.LargeGridMultiplier));

                    result *= (float)_currentThreatSettings.GridTypeMultipliers.LargeGridMultiplier;
                }
                else
                {
                    result += (float)(_grid.AllBlocks.Count * _currentThreatSettings.ThreatPerBlockMultipliers.SmallGridMultiplier);

                    result += ((float)(Vector3D.Distance(_grid.CubeGrid.WorldAABB.Min, _grid.CubeGrid.WorldAABB.Max)
                   * _currentThreatSettings.BoundingBoxSizeMultipliers.SmallGridMultiplier));

                    result *= (float)_currentThreatSettings.GridTypeMultipliers.SmallGridMultiplier;
                }

            }


            if (_grid.PowerOutput().Y > 0)
            {
                GridTypeThreatMultiplier multipliers =_currentThreatSettings.GridPowerOutputMultipliers;
                float modifier = (float)(_grid.CubeGrid.IsStatic 
                    ? (_grid.PowerOutput().Y * multipliers.StationMultiplier) 
                    : (_grid.CubeGrid.GridSizeEnum == MyCubeSize.Large) 
                        ? (_grid.PowerOutput().Y * multipliers.LargeGridMultiplier) 
                        : (_grid.PowerOutput().Y * multipliers.SmallGridMultiplier));
            }

            return result;
        }

        private float evaluateBlockCategories(HashSet<long> evaluatedBlockIDs, bool filterEvaluatedIDs = true)
        {
            if (!config_initialized)
            {
                return DEFAULT_THREAT;
            }
            float threatTotal = 0;
            var blockCategoryThreat = _currentThreatSettings.BlockCategoryThreatEntries;

            Dictionary<BlockCategoryThreat, List<float>> catSpecificThreats = new Dictionary<BlockCategoryThreat, List<float>>();
 
            foreach (var catThreat in blockCategoryThreat)
            {
                string name = catThreat.Category;
                List<BlockEntity> blocks = new List<BlockEntity>();

               
                BlockTypeEnum p;
                if(Enum.TryParse(name, true, out p))
                {

                    if (_grid.BlockListReference.TryGetValue(p, out blocks))
                    {

                        if (blocks.Count == 0) continue;
                        foreach (BlockEntity block in blocks.Where((block) => (filterEvaluatedIDs ? !evaluatedBlockIDs.Contains(block.GetEntityId()) : true)))
                        {
                            float addedScore = 0f;
                            try
                            {
                                if (block.Block.HasInventory && catThreat.FullVolumeThreat != 0)
                                {
                                    float invMod = ((float)block.Block.GetInventory().CurrentVolume / (float)block.Block.GetInventory().MaxVolume) + 1;
                                    if (!float.IsNaN(invMod))
                                    {
                                        addedScore += invMod * catThreat.FullVolumeThreat;
                                    }
                                }

                                if (!catSpecificThreats.ContainsKey(catThreat))
                                {
                                    catSpecificThreats[catThreat] = new List<float>();
                                }

                                catSpecificThreats[catThreat].Add(addedScore + catThreat.Threat);
                            }
                            catch (KeyNotFoundException ex)
                            {
                                SpawnLogger.Write($"{name}: {BlockTypeEnum.Antennas.ToString()} : : {ex.Message}", SpawnerDebugEnum.Threat);
                            }
                            catch (Exception ex)
                            {
                                SpawnLogger.Write($"{name}: {BlockTypeEnum.Antennas.ToString()} : : {ex.Message}", SpawnerDebugEnum.Threat);
                            }
                        }
                    }
                }    
            }

            foreach (var t in catSpecificThreats)
            {
                BlockCategoryThreat threatDef = t.Key;
                List<float> threatDetected = t.Value;
                if (threatDetected.Count == 0)
                {
                    continue;
                }
                else if (threatDetected.Count == 1)
                {
                    threatTotal += threatDetected.FirstOrDefault();
                }
                else if (threatDetected.Count <= threatDef.MultiplierThreshold)
                {
                    threatTotal += threatDetected.Sum();
                }
                else
                {
                    int totalLength = threatDetected.Count;
                    int numberToPenalize = totalLength - threatDef.MultiplierThreshold;

                    List<float> normalScore = threatDetected.GetRange(0, threatDef.MultiplierThreshold);
                    List<float> cumScore = threatDetected.GetRange(threatDef.MultiplierThreshold, numberToPenalize);

                    float cumTotal = (float)(cumScore.FirstOrDefault() * threatDef.Multiplier);
                    float runningTotal = normalScore.Sum();
                    for (int i = 1; i < cumScore.Count; i++)
                    {
                        cumTotal = (float)((cumTotal + cumScore[i]) * threatDef.Multiplier);
                    }
                    threatTotal += (runningTotal + cumTotal);
                }
            }
            return threatTotal;

        }

        private float evaluateSingleBlocks(HashSet<long> evaluatedBlockIDs, bool filterEvaluatedIDs = true)
        {
            if (!config_initialized)
            {
                return DEFAULT_THREAT;
            }
            float threatTotal = 0;
            var singleBlockThreat = _currentThreatSettings.SingleBlockThreatDefinitions;
            
            Dictionary<ThreatDefinition, List<float>> blockSpecificThreats = new Dictionary<ThreatDefinition, List<float>>();
            List<BlockEntity> allBlocks = this._grid.AllTerminalBlocks
                .Where( 
                    (block) => (filterEvaluatedIDs ? !evaluatedBlockIDs.Contains(block.GetEntityId()) : true)).ToList();


            foreach (var block in allBlocks)
            {
                if (block.IsClosed() || !block.Functional)
                    continue;

                var blockDefinition = block.Block.BlockDefinition;
                string blockType = blockDefinition.TypeIdString;
                string blockSubType = blockDefinition.SubtypeName;
                string fullBlockType = blockDefinition.ToString();
                ThreatDefinition threatDef = null;

                bool isBlockSpecific = singleBlockThreat.TryGetValue(fullBlockType, out threatDef);

                if (!isBlockSpecific)
                    continue;              

                evaluatedBlockIDs.Add(block.GetEntityId());
                float addedThreat = (float)threatDef.Threat;

                if (block.Block.HasInventory && threatDef.FullVolumeThreat != 0)
                {
                    float invMod = ((float)block.Block.GetInventory().CurrentVolume / (float)block.Block.GetInventory().MaxVolume) + 1;
                    if (!float.IsNaN(invMod))
                    {
                        addedThreat += (float)(invMod * threatDef.FullVolumeThreat);
                    }
                }
                blockSpecificThreats[threatDef].Add(addedThreat);
            }

            foreach (var t in blockSpecificThreats)
            {
                ThreatDefinition threatDef = t.Key;
                List<float> threatDetected = t.Value;
                if (threatDetected.Count == 0)
                {
                    continue;
                }
                else if (threatDetected.Count == 1)
                {
                    threatTotal += threatDetected.FirstOrDefault();
                }
                else if(threatDetected.Count <= threatDef.MultiplierThreshold)
                {
                    threatTotal += threatDetected.Sum();
                }
                else
                {
                    int totalLength = threatDetected.Count;
                    int numberToPenalize = totalLength - threatDef.MultiplierThreshold;

                    List<float> normalScore = threatDetected.GetRange(0, threatDef.MultiplierThreshold);
                    List<float> cumScore = threatDetected.GetRange(threatDef.MultiplierThreshold, numberToPenalize);

                    float cumTotal = (float)(cumScore.FirstOrDefault() * threatDef.Multiplier);
                    float runningTotal = normalScore.Sum();
                    for(int i = 1; i < cumScore.Count; i++) {
                        cumTotal = (float)((cumTotal + cumScore[i]) * threatDef.Multiplier);
                    }
                    threatTotal += (runningTotal + cumTotal);
                }
            }
            return threatTotal;
        }



        public static float GetTargetValueFromBlockList(List<BlockEntity> blockList, string categoryName, bool scanInventory = false)
        {

            float totalThreatResult = 0F;

            // Current Threat Config
            ConfigThreat currentThreatSettings = Settings.Threat;

            // Used to track specific blocks within the block's 'category' assigned by MES            
            Dictionary<string, List<float>> blockSpecificThreats = new Dictionary<string, List<float>>();

            // Tally for the threat limited to non-specific 'category' based blocks
            List<float> categoryThreats = new List<float>();

            ThreatDefinition categoryThreatDef = null;

            // Try to get a value for the category from the current threat definitions
            currentThreatSettings.BlockCategoryThreatDefinitions.TryGetValue(categoryName, out categoryThreatDef);


            foreach (var block in blockList)
            {

                // We don't count non-functional blocks here. They DO contribute to threat insofar as overall block count.
                if (block.IsClosed() || !block.Functional)
                    continue;

                // First, try and get the block's subtype ID. If it doesn't have one, then use the blocks main type ID.
                string blockType = String.IsNullOrEmpty(block.Block.BlockDefinition.SubtypeId)
             ? block.Block.BlockDefinition.TypeIdString
             : block.Block.BlockDefinition.SubtypeId;

                ThreatDefinition threatDef = null;

                // Before we consider category threat, let's try and get a more granular definition if it exists. Also, use it to set a flag for later.
                bool isBlockSpecific = currentThreatSettings.SingleBlockThreatDefinitions.TryGetValue(blockType, out threatDef);

                if (!isBlockSpecific)
                    threatDef = categoryThreatDef;

                // we didn't find ANY threat. Don't continue calculation.
                if (threatDef == null)
                    continue;


                float value = (float)threatDef.Threat;
                if (scanInventory
                    && block.Block.HasInventory
                    && block.Block.GetInventory().MaxVolume > 0)
                {
                    // This value will range from 0ish-1.0, representing how filled the container is in percentage.
                    // 0.54 = 54% full. 

                    float invMod = ((float)block.Block.GetInventory().CurrentVolume / (float)block.Block.GetInventory().MaxVolume) + 1;
                    if (!float.IsNaN(invMod))
                    {
                        // We add an amount of threat based on how full the container is times the potential volume modifier
                        value += (float)(invMod * threatDef.FullVolumeThreat);
                    }
                }

                // Finally. If the threat is calculated based on a specific block type, then it goes into the dictionary.
                // If not, then we are safe to add it to the category score

                if (isBlockSpecific)
                {

                    if (!blockSpecificThreats.ContainsKey(blockType))
                    {
                        // Initialize a new list if it doesn't exist
                        blockSpecificThreats[blockType] = new List<float>();
                    }
                    // And add the value for tallying later.
                    blockSpecificThreats[blockType].Add(value);
                }
                else
                {
                    // Add the category threat to the list of threat values.
                    categoryThreats.Add(value);
                }
            }

            // Now, we tally things up and apply penalties.

            // Apply a progressive penalty for category-level blocks
            if (categoryThreatDef != null
                && categoryThreats.Count > 0)
            {
                // Our penalty multiplier.
                float multiplier = (float)categoryThreatDef.Multiplier;

                // The running tally. Start with first element as the base value so we apply the penalty appropriately.
                float compoundedThreat = categoryThreats[0];
                for (int i = 1; i < categoryThreats.Count; i++)
                {
                    compoundedThreat = (compoundedThreat + categoryThreats[i]) * multiplier;
                }

                totalThreatResult += compoundedThreat;
            }

            // Apply progressive penalty to each specific block type
            foreach (var kvp in blockSpecificThreats)
            {
                string blockType = kvp.Key;
                List<float> threats = kvp.Value;

                // We need to retrieve this again because we need the multiplier. Perhaps something to improve on.
                ThreatDefinition threatD;
                if (!currentThreatSettings.SingleBlockThreatDefinitions.TryGetValue(blockType, out threatD))
                    continue;

                float multiplier = (float)threatD.Multiplier;

                float compoundedThreat = threats[0];
                for (int i = 1; i < threats.Count; i++)
                {
                    compoundedThreat = (compoundedThreat + threats[i]) * multiplier;
                }
                totalThreatResult += compoundedThreat;
            }

            // And we are done. Threatening, yes?
            return totalThreatResult;
        }
    }
}

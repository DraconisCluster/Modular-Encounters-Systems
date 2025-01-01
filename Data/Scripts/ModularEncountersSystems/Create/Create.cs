﻿using ModularEncountersSystems.Entities;
using ModularEncountersSystems.Logging;
using ModularEncountersSystems.Sync;
using ModularEncountersSystems.World;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage.Game;
using VRage.Game.ModAPI;

namespace ModularEncountersSystems.Create
{

    public class CreateMaker
    {

        public static string CreateZone(ChatMessage msg, string args)
        {
            // StringBuilder
            var sb = new StringBuilder();
            var array = args.Trim().Split('#');


            string VectorCoords = "{" + msg.PlayerPosition.ToString() + "}";

            string Name = "DefaultName";
            string Distance = "DefaultDistance";
            bool unique = false;

            // Check if array has expected length and assign values accordingly
            if (array.Length >= 2)
            {
                Name = array[0];
                Distance = array[1];
                MyVisualScriptLogicProvider.AddGPSObjectiveForAll($"{Name} {Distance}m", $"You can remove this GPS. It is just to help you.", msg.PlayerPosition, VRageMath.Color.Beige);
            }
            else
            {
                return sb.Append("Missing Arguments").ToString();
            }

            var planet = PlanetManager.GetNearestPlanet(msg.PlayerPosition);

            if (planet != null)
            {

                var up = planet.UpAtPosition(msg.PlayerPosition);
                var dist = planet.AltitudeAtPosition(msg.PlayerPosition, false);

                // Your XML string with placeholders replaced
                sb.Append($@"
	              <EntityComponent xsi:type=""MyObjectBuilder_InventoryComponentDefinition"">
                  <Id>
                    <TypeId>Inventory</TypeId>
                    <SubtypeId>Zone{Name}</SubtypeId>
                  </Id>
                  <Description>

                    [MES Zone]
		
                    [PublicName:Zone{Name}]

                    [Active:true]
                    [Persistent:true]
                    [Strict:true]

                    [Coordinates:{VectorCoords}]
                    [Radius:{Distance}]
		
		            [PlanetaryZone:true]
		            [PlanetName:{planet.Planet.Generator.Id.SubtypeName}]
		            [Direction:{up}]		
		            [HeightOffset:{dist}]		
		
		            [UseZoneAnnounce:false]
		            [ZoneEnterAnnounce:Entering {Name}]
		            [ZoneLeaveAnnounce:Leaving {Name}]		

                  </Description>

                </EntityComponent>");

            }
            else
            {
                sb.Append($@"
	              <EntityComponent xsi:type=""MyObjectBuilder_InventoryComponentDefinition"">
                  <Id>
                    <TypeId>Inventory</TypeId>
                    <SubtypeId>Zone{Name}</SubtypeId>
                  </Id>
                  <Description>

                    [MES Zone]
		
                    [PublicName:Zone{Name}]

                    [Active:true]
                    [Persistent:true]
                    [Strict:true]

                    [Coordinates:{VectorCoords}]
                    [Radius:{Distance}]
		
		            [UseZoneAnnounce:false]
		            [ZoneEnterAnnounce:Entering {Name}]
		            [ZoneLeaveAnnounce:Leaving {Name}]		

                  </Description>

                </EntityComponent>");
            }

            return sb.ToString();

        }




        public static string CreateEventArea(ChatMessage msg, string args)
        {
            // StringBuilder
            var sb = new StringBuilder();
            var array = args.Trim().Split('#');


            string VectorCoords = "{" + msg.PlayerPosition.ToString()+ "}";

            string Name = "DefaultName";
            string distance = "DefaultDistance";
            bool unique = false;
            // Check if array has expected length and assign values accordingly
            if (array.Length >= 2)
            {
                Name = array[0];
                distance = array[1];
                MyVisualScriptLogicProvider.AddGPSObjectiveForAll($"{Name} {distance}m", $"You can remove this GPS. It is just to help you.", msg.PlayerPosition, VRageMath.Color.Beige);
            }
            else
            {
                return sb.Append("Missing Arguments").ToString();
            }

           // Your XML string with placeholders replaced
            sb.Append($@"
    <!-- Persistant-->
    <EntityComponent xsi:type=""MyObjectBuilder_InventoryComponentDefinition"">
        <Id>
            <TypeId>Inventory</TypeId>
            <SubtypeId>PlayerCondition_Area_{Name}</SubtypeId>
        </Id>
        <Description>
            [MES Player Condition]

            [CheckPlayerNear:true]    
            [PlayerNearCoords:{VectorCoords}]
            [PlayerNearDistanceFromCoords:{distance}]

            [CheckPlayerTags:true]
            [ExcludedPlayerTag:Player_Triggered_Area_{Name}]

            [CheckPlayerReputation:false]
            [CheckReputationwithFaction:SPRT]
            [MinPlayerReputation:500]
            [MaxPlayerReputation:1500]
        </Description>
    </EntityComponent>

    <EntityComponent xsi:type=""MyObjectBuilder_InventoryComponentDefinition"">
        <Id>
            <TypeId>Inventory</TypeId>
            <SubtypeId>PlayerCondition_Area_{Name}_ConsiderTriggered</SubtypeId>
        </Id>
        <Description>
            [MES Player Condition]
            [CheckPlayerNear:true]    
            [PlayerNearCoords:{VectorCoords}]
            [PlayerNearDistanceFromCoords:4500]
        </Description>
    </EntityComponent>

    <EntityComponent xsi:type=""MyObjectBuilder_InventoryComponentDefinition"">
        <Id>
            <TypeId>Inventory</TypeId>
            <SubtypeId>MES_EventPersistantCondition_Area_{Name}</SubtypeId>
        </Id>
        <Description>
            [MES Event Condition]
            [CheckPlayerCondition:true]
            [PlayerConditionIds:PlayerCondition_Area_{Name}]     
        </Description>   
    </EntityComponent>


    <EntityComponent xsi:type=""MyObjectBuilder_InventoryComponentDefinition"">
        <Id>
            <TypeId>Inventory</TypeId>
            <SubtypeId>MES_Event_Area_{Name}</SubtypeId>
        </Id>
        <Description>
            [MES Event]
            [UseEvent:true]
            [Tags:EventArea{Name}]
            [UniqueEvent:false]    
            [MinCooldownMs:600000]
            [MaxCooldownMs:600001]
            [PersistantConditionIds:MES_EventPersistantCondition_Area_{Name}]


            [UseAnyPassingCondition:true]
            [ActionExecution:Condition]

            [ConditionIds:MES_EventCondition_Area_{Name}_A]
            [ActionIds:MES_EventAction_Area_{Name}_A]



        </Description>
    </EntityComponent>

    <!-- Option A -->

    <EntityComponent xsi:type=""MyObjectBuilder_InventoryComponentDefinition"">
        <Id>
            <TypeId>Inventory</TypeId>
            <SubtypeId>MES_EventCondition_Area_{Name}_A</SubtypeId>
        </Id>
        <Description>
            [MES Event Condition]
   
            [CheckPlayerCondition:true]
            [PlayerConditionIds:PlayerCondition_Area_{Name}_A]    
        </Description>   
    </EntityComponent>

    <EntityComponent xsi:type=""MyObjectBuilder_InventoryComponentDefinition"">
        <Id>
            <TypeId>Inventory</TypeId>
            <SubtypeId>PlayerCondition_Area_{Name}_A</SubtypeId>
        </Id>
        <Description>
            [MES Player Condition]

            [CheckPlayerReputation:false]
            [CheckReputationwithFaction:SPRT]
            [MinPlayerReputation:500]
            [MaxPlayerReputation:1500]

            [CheckPlayerTags:false]
            [IncludedPlayerTag:]
            [ExcludedPlayerTag:]
        </Description>
    </EntityComponent>


    <!-- Option A -->
    <EntityComponent xsi:type=""MyObjectBuilder_InventoryComponentDefinition"">
        <Id>
            <TypeId>Inventory</TypeId>
            <SubtypeId>MES_EventAction_Area_{Name}_A</SubtypeId>
        </Id>
        <Description>
            [MES Event Action]

            [AddTagstoPlayers:true]
            [AddTagsPlayerConditionIds:PlayerCondition_Area_{Name}_ConsiderTriggered]
            [AddTags:Player_Triggered_Area_{Name}]

            [UseChatBroadcast:true]
            [ChatData:MES_EventChat_Area_{Name}_A]

        </Description>
    </EntityComponent>

    <EntityComponent xsi:type=""MyObjectBuilder_InventoryComponentDefinition"">
        <Id>
            <TypeId>Inventory</TypeId>
            <SubtypeId>MES_EventChat_Area_{Name}_A</SubtypeId>
        </Id> 
        <Description>
            [RivalAI Chat]
            [UseChat:true]
            [StartsReady:true]
            [Chance:100]
            [MaxChats:-1]
            [BroadcastRandomly:true]
            [IgnoreAntennaRequirement:True]
            [IgnoredAntennaRangeOverride:1]
            [SendToAllOnlinePlayers:false]
            [SendToSpecificPlayers:true]
            [PlayerConditionIds:PlayerCondition_Area_{Name}_ConsiderTriggered]
            [Color:Green]
            [Author:MES]
            [ChatMessages:{Name} A triggered]
            [BroadcastChatType:Chat]
            [ChatAudio:]  
        </Description>
    </EntityComponent>");
            


        return sb.ToString();
        }
    }
}

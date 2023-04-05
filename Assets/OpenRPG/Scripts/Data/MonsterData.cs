using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VGDevs;

namespace ORC
{
    [CreateAssetMenu(fileName = nameof(MonsterData), menuName = kBaseScriptableDataPath + nameof(MonsterData))]
    public class MonsterData : ScriptableData
    {
        public override void ParseSource(string json)
        {
            if (FifthEdition == null)
                return;

            JObject monster = JsonConvert.DeserializeObject<JObject>(json);
            if (monster == null)
                return;

            string content = "";
            
            string title = monster.Value<string>("name");
            Title = title;
            content += $"{BuiltStyle_Title(title)}\n";

            // Check if the monster is a copy and extract the base monster data
            JObject baseMonsterToCopy = null;
            if (monster.ContainsKey("_copy") && (baseMonsterToCopy = (JObject) monster["_copy"]) != null)
            {
                JObject baseMonster = GetToCopyMonsterData(baseMonsterToCopy);
                JArray alignmentArray = baseMonster.Value<JArray>("alignment");
                List<string> stringList = alignmentArray?.Select(token => (string)token).ToList();
                Alignment alignment = stringList?.Count > 0 ? stringList.AsAlignment() : Alignment.Undefined;
                
                JArray sizeArray = baseMonster.Value<JArray>("size");
                List<string> sizeList = sizeArray?.Select(token => (string)token).ToList();
                CreatureSize creatureSize = sizeList?.Count > 0 ? sizeList.AsCreatureSize() : CreatureSize.Undefined;
                
                content += $"{BuiltStyle_SubTitle($"{creatureSize} Creature, {alignment.ToFlagString()}")}\n";
                content += $"{BuiltStyle_Separator()}\n";
                
                var speedToken = ToSpeedString((JObject)baseMonster["speed"]);
                content += $"{BuiltStyle_RedProperties(speedToken)}\n";
            }
            
            FifthEdition.Content = content;
        }
        
        public static string ToSpeedString(JObject speedObj)
        {
            var speeds = new List<string>();
            foreach (var property in speedObj.Properties())
            {
                var speed = property.Value.Value<int>();
                var speedName = property.Name;
                var speedStr = $"{(speedName.Equals("walk") ? "" : speedName)}: {speed}ft";
                speeds.Add(speedStr);
            }

            return $"<b>Speed:</b> {string.Join(", ", speeds)}";
        }

        public JObject GetToCopyMonsterData(JObject monsterToCopy)
        {
            string baseMonsterName = monsterToCopy.Value<string>("name");
            string baseMonsterSource = monsterToCopy.Value<string>("source");
            JObject foundCopy = FindItemInCollection(GameResources.OrcData.Monsters, kDungeonsAndDragons5thEditionKey, baseMonsterSource, baseMonsterName.AsSlug());

            return foundCopy;
            
        }
    }
}
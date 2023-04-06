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
                Alignment alignment = ExtractAlignment(baseMonster);
                CreatureSize creatureSize = ExtractCreatureSize(baseMonster);
                CreatureAttributes creatureAttributes = ExtractCreatureAttributes(baseMonster);
                List<string> skills = ExtractSkills(baseMonster);
                
                content += $"{BuiltStyle_SubTitle($"{creatureSize} Creature, Typically {alignment.ToFlagString()}")}\n";
                content += $"{BuiltStyle_Separator()}\n";
                
                var speedToken = ToSpeedString((JObject)baseMonster["speed"]);
                content += $"{BuiltStyle_RedProperties(speedToken)}\n";
                content += $"{BuiltStyle_RedProperties(string.Join(", ", skills))}\n";
                content += $"{BuiltStyle_RedProperties($"Attributes: {creatureAttributes}")}\n";
            }
            
            FifthEdition.Content = content;
        }

        private List<string> ExtractSkills(JObject creature)
        {
            var toReturn = new List<string>();

            if (!creature.ContainsKey("skill"))
                return toReturn;

            var skills = (JObject) creature["skill"];
            
            foreach (var skill in skills.Properties())
            {
                var bonus = skill.Value.Value<string>();
                var skillName = skill.Name;
                toReturn.Add($"{skillName.Capitalize()} {bonus}");
            }
            
            return toReturn;
        }

        private static Alignment ExtractAlignment(JObject data)
        {
            if (!data.ContainsKey("alignment"))
                return Alignment.Undefined;
            
            List<string> alignments = data.Value<JArray>("alignment").AsListOfString();
            return alignments?.Count > 0 ? alignments.AsAlignment() : Alignment.Undefined;
        }

        private static CreatureSize ExtractCreatureSize(JObject data)
        {
            if (!data.ContainsKey("size"))
                return CreatureSize.Undefined;
            
            List<string> sizes = data.Value<JArray>("size").AsListOfString();
            return sizes?.Count > 0 ? sizes.AsCreatureSize() : CreatureSize.Undefined;
        }

        private static CreatureAttributes ExtractCreatureAttributes(JObject data)
        {
            CreatureAttributes toReturn = new CreatureAttributes();
            if (data.ContainsKey("str"))
                toReturn.STR = data.Value<int>("str");
            if (data.ContainsKey("dex"))
                toReturn.DEX = data.Value<int>("dex");
            if (data.ContainsKey("con"))
                toReturn.CON = data.Value<int>("con");
            if (data.ContainsKey("int"))
                toReturn.INT = data.Value<int>("int");
            if (data.ContainsKey("wis"))
                toReturn.WIS = data.Value<int>("wis");
            if (data.ContainsKey("cha"))
                toReturn.CHA = data.Value<int>("cha");
            return toReturn;
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
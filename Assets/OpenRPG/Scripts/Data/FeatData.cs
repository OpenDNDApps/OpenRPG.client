using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VGDevs;

namespace ORC
{
    [CreateAssetMenu(fileName = nameof(FeatData), menuName = kBaseScriptableDataPath + nameof(FeatData))]
    public class FeatData : ScriptableData
    {
        public SourceBookData Sources;
        public string SourcePages;
        public List<FeatPrerequisite> Prerequisites = new List<FeatPrerequisite>();

        public override void ParseSource(string json)
        {
            JObject book = JsonConvert.DeserializeObject<JObject>(json);

            if (book == null)
                return;

            string title = book.Value<string>("name");
            string source = book.Value<string>("source").AsSlug();
            string page = book.Value<string>("page");
            
            if(source.StartsWith("ua"))
                source = "ua";

            var found = GameResources.OrcData.SourceBooks.Find(item => item.name.Equals(source.AsSlug()));
            if (found == null || string.IsNullOrEmpty(page) || string.IsNullOrEmpty(source))
                return;
            
            Sources = found;
            SourcePages = page;
            
            Prerequisites.Clear();

            try
            {
                string prerequisiteLevel = (string)book.SelectToken("prerequisite[0].level");
                if (!string.IsNullOrEmpty(prerequisiteLevel))
                {
                    Prerequisites.Add(new FeatPrerequisite { Type = FeatPrerequisiteType.Level, Value = prerequisiteLevel });
                }
            }
            catch (Exception) { /* Ignore */ }

            try
            {
                string prerequisiteLevel = (string)book.SelectToken("prerequisite[0].level?.level");
                if (!string.IsNullOrEmpty(prerequisiteLevel))
                {
                    Prerequisites.Add(new FeatPrerequisite { Type = FeatPrerequisiteType.Level, Value = prerequisiteLevel });
                }
            }
            catch (Exception) { /* Ignore */ }

            try
            {
                JArray prerequisiteRaces = (JArray)book.SelectToken("prerequisite[0].race");
                foreach (var raceToken in prerequisiteRaces)
                {
                    JObject race = (JObject) raceToken;
                    if (race.ContainsKey("name"))
                        Prerequisites.Add(new FeatPrerequisite { Type = FeatPrerequisiteType.Race, Value = race.Value<string>("name") });
                    if (race.ContainsKey("subrace"))
                        Prerequisites.Add(new FeatPrerequisite { Type = FeatPrerequisiteType.Race, Value = race.Value<string>("subrace") });
                }
            }
            catch (Exception) { /* Ignore */ }

            try
            {
                JArray prerequisiteFeats = (JArray)book.SelectToken("prerequisite[0].feat");
                foreach (var jToken in prerequisiteFeats)
                {
                    if (jToken.First == null) 
                        continue;
                    
                    string feat = (jToken.First.Value<string>())?.Split("|")[0].AsSlug();
                    Prerequisites.Add(new FeatPrerequisite() { Type = FeatPrerequisiteType.Feat, Value = feat });
                }
            }
            catch (Exception) { /* Ignore */ }
            
            try
            {
                JObject prerequisite = (JObject)book.SelectToken("prerequisite[0]");
                if (prerequisite != null)
                {
                    if (prerequisite.ContainsKey("proficiency"))
                    {
                        Prerequisites.Add(new FeatPrerequisite() { Type = FeatPrerequisiteType.Proficiency, Value = prerequisite.Value<string>("proficiency") });
                    }
                    if(prerequisite.ContainsKey("ability"))
                    {
                        Prerequisites.Add(new FeatPrerequisite() { Type = FeatPrerequisiteType.AbilityScore, Value = prerequisite.Value<string>("ability") });
                    }
                    if (prerequisite.ContainsKey("skill"))
                    {
                        Prerequisites.Add(new FeatPrerequisite() { Type = FeatPrerequisiteType.Skill, Value = prerequisite.Value<string>("skill") });
                    }
                    if (prerequisite.ContainsKey("spell"))
                    {
                        Prerequisites.Add(new FeatPrerequisite() { Type = FeatPrerequisiteType.Spell, Value = prerequisite.Value<string>("spell") });
                    }
                    if (prerequisite.ContainsKey("other"))
                    {
                        Prerequisites.Add(new FeatPrerequisite() { Type = FeatPrerequisiteType.Other, Value = prerequisite.Value<string>("other") });
                    }
                }
            }
            catch (Exception) { /* Ignore */ }
            
            Title = title;
        }
        
        [Serializable]
        public struct FeatPrerequisite
        {
            public FeatPrerequisiteType Type;
            public string Value;
        }
        
        public enum FeatPrerequisiteType
        {
            Level,
            Proficiency,
            AbilityScore,
            Race,
            SubRace,
            Feat,
            Skill,
            Spell,
            Other
        }
    }

    public partial class OrcDataCollection
    {
        
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ORC
{
    [CreateAssetMenu(fileName = nameof(SkillData), menuName = kBaseScriptableDataPath + nameof(SkillData))]
    public class SkillData : ScriptableData
    {
        public override void ParseSource(string json)
        {
            JObject skill = JsonConvert.DeserializeObject<JObject>(json);

            if (skill == null)
                return;

            string title = skill.Value<string>("name");
            
            Title = title;
        }
    }
}
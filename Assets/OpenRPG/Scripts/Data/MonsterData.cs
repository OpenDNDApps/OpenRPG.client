using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

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

            string content = "<style=\"paper\">";
            
            string title = monster.Value<string>("name");
            Title = title;
            content += $"<h2><b>{title}</b></h2>\n";
            content += $"<separator></separator>\n";
            
            FifthEdition.Content = content + "\n</style>";
        }
    }
}
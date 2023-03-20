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
            JObject monster = JsonConvert.DeserializeObject<JObject>(json);

            if (monster == null)
                return;

            string title = monster.Value<string>("name");
            
            Title = title;
        }
    }
}
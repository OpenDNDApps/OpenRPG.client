using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ORC
{
    [CreateAssetMenu(fileName = nameof(CharacterRaceData), menuName = kBaseScriptableDataPath + nameof(CharacterRaceData))]
    public class CharacterRaceData : ScriptableData
    {
        public override void ParseSource(string json)
        {
            JObject race = JsonConvert.DeserializeObject<JObject>(json);

            if (race == null)
                return;

            string title = race.Value<string>("name");
            
            Title = title;
        }
    }
}
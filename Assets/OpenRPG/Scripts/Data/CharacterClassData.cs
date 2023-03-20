using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ORC
{
    [CreateAssetMenu(fileName = nameof(CharacterClassData), menuName = kBaseScriptableDataPath + nameof(CharacterClassData))]
    public class CharacterClassData : ScriptableData
    {
        public override void ParseSource(string json)
        {
            JObject charClass = JsonConvert.DeserializeObject<JObject>(json);

            if (charClass == null)
                return;

            string title = charClass.Value<string>("name");
            
            Title = title;
        }
    }
}
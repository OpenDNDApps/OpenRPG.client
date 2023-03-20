using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ORC
{
    [CreateAssetMenu(fileName = nameof(CharacterBackgroundData), menuName = kBaseScriptableDataPath + nameof(CharacterBackgroundData))]
    public class CharacterBackgroundData : ScriptableData
    {
        public override void ParseSource(string json)
        {
            JObject charBackground = JsonConvert.DeserializeObject<JObject>(json);

            if (charBackground == null)
                return;

            string title = charBackground.Value<string>("name");
            
            Title = title;
        }
    }
}

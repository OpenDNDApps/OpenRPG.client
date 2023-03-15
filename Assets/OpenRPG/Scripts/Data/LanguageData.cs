using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace OpenRPG
{
    [CreateAssetMenu(fileName = nameof(LanguageData), menuName = kBaseScriptableDataPath + nameof(LanguageData))]
    public class LanguageData : ScriptableData
    {
        public override void ParseSource(string json)
        {
            JObject language = JsonConvert.DeserializeObject<JObject>(json);

            if (language == null)
                return;

            string title = language.Value<string>("name");
            
            Title = title;
        }
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace OpenRPG
{
    [CreateAssetMenu(fileName = nameof(SpellData), menuName = kBaseScriptableDataPath + nameof(SpellData))]
    public class SpellData : ScriptableData
    {
        public override void ParseSource(string json)
        {
            JObject spell = JsonConvert.DeserializeObject<JObject>(json);

            if (spell == null)
                return;

            string title = spell.Value<string>("name");
            
            Title = title;
        }
    }
}
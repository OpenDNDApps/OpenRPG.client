using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace OpenRPG
{
    [CreateAssetMenu(fileName = nameof(ItemData), menuName = kBaseScriptableDataPath + nameof(ItemData))]
    public class ItemData : ScriptableData
    {
        public override void ParseSource(string json)
        {
            JObject item = JsonConvert.DeserializeObject<JObject>(json);

            if (item == null)
                return;

            string title = item.Value<string>("name");
            
            Title = title;
        }
    }
}
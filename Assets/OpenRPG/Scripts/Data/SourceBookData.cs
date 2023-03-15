using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VGDevs;

namespace OpenRPG
{
    [CreateAssetMenu(fileName = nameof(FeatData), menuName = kBaseScriptableDataPath + nameof(FeatData))]
    public class SourceBookData : ScriptableData
    {
        public string CoverURL;
        public string Published;
        public string Author;

        public override void ParseSource(string source)
        {
            JObject book = JsonConvert.DeserializeObject<JObject>(source);

            if (book == null)
                return;

            string title = book.Value<string>("name");
            string coverUrl = book.Value<string>("coverUrl");
            string published = book.Value<string>("published");
            string author = book.Value<string>("author");
            
            Title = title;
            CoverURL = coverUrl;
            Published = published;
            Author = author;
        }
    }
}
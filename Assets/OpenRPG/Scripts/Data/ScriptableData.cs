using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VGDevs;

namespace OpenRPG
{
    public class ScriptableData : BaseScriptableObject
    {
        public string Title;
        public string AsJson;
        public string DataHash;
        
        public const string kBaseScriptableDataPath = "OpenRPG/";
        
        public virtual void ParseSource(string source) {
            Debug.Log("Parsed: " + source);
        }
        
        #if UNITY_EDITOR
        public static void ParseSourceFile<T>(string filePath, string idKey, string rootKey, string creationPath, ref List<T> existingData) where T : ScriptableData
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"File not found: '{filePath}'");
                return;
            }
            
            JObject root = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath));
            JArray books = (JArray) root?[rootKey];
            
            if (books == null)
                return;

            int skipped = 0;
            int created = 0;
            int added = 0;
            foreach (JToken bookToken in books)
            {
                var book = (JObject) bookToken;

                string title = book.Value<string>("name");
                string id = book.Value<string>(idKey).AsSlug();
                if (string.IsNullOrEmpty(title))
                {
                    Debug.Log($"Skipped: '{title.AsSlug()}'");
                    continue;
                }

                string asJson = book.ToString(Formatting.None);
                string hash = asJson.AsMD5Hash();

                T newObject = null;
                bool shouldCreateNewFile = false;
                T found = existingData.Find(item => item.name.Equals(id)) as T;
                if (found != null)
                {
                    if (found.DataHash.Equals(hash))
                    {
                        skipped++;
                        continue;
                    }

                    newObject = found;
                    newObject.AsJson = asJson;
                    newObject.DataHash = hash;
                    Debug.Log($"<color=orange><b>Updated:</b> '{title.AsSlug()}'</color>");
                }
                else
                {
                    shouldCreateNewFile = true;
                    newObject = CreateInstance<T>();
                    newObject.name = id;
                    newObject.AsJson = asJson;
                    newObject.DataHash = hash;
                }

                if (shouldCreateNewFile)
                {
                    created++;
                    UnityEditor.AssetDatabase.CreateAsset(newObject, $"{creationPath}/{id}.asset");
                }
                
                newObject.ParseSource(asJson);
                
                if (existingData.AddUnique(newObject))
                {
                    added++;
                }

                UnityEditor.EditorUtility.SetDirty(newObject);
            }

            if (skipped > 0)
            {
                Debug.Log($"<color=yellow><b>{skipped} Skipped:</b></color>");
            }

            if (created > 0)
            {
                Debug.Log($"<color=green><b>{created} Created:</b></color>");
            }
            if (added > 0)
            {
                Debug.Log($"<color=green><b>{added} Added:</b></color>");
            }
        }
        #endif
    }

    [Flags]
    public enum Alignment
    {
        Undefined = 0,
        Lawful = 1 << 1,
        Chaotic = 1 << 2,
        Neutral = 1 << 3,
        Good = 1 << 4,
        Evil = 1 << 5,
    }

    [Flags]
    public enum CreatureSize
    {
        Undefined = 0,
        Diminutive = 1 << 1,
        Tiny = 1 << 2,
        Small = 1 << 3,
        Medium = 1 << 4,
        Large = 1 << 5,
        Huge = 1 << 6,
        Gargantuan = 1 << 7,
        Colossal = 1 << 8
    }
    
}

#if UNITY_EDITOR
    
namespace OpenRPG
{
    using UnityEditor;
    using System.IO;
    
    [CustomEditor(typeof(ScriptableData), true)]
    public class ScriptableDataEditor : Editor
    {
        protected ScriptableData m_target;
        private const string kDataFilesFolder = "Assets/StreamingAssets/DataFiles/";
        
        private void OnEnable()
        {
            m_target = (ScriptableData)target;
        }

        public override void OnInspectorGUI() {
            m_target = (ScriptableData)target;

            if (GUILayout.Button("Parse from JSON")) 
            {
                if (m_target.AsJson.Length > 0) 
                {
                    m_target.ParseSource(m_target.AsJson);
                }
            }
            
            base.OnInspectorGUI();
        }
    }
}
    
#endif

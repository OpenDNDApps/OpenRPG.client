using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ORC
{
    public class ScriptableData : OrcScriptableObject
    {
        public string ID => this.name;
        public string Title;
        public string AsJson;
        public string DataHash;
        
        [Space(4f)]
        public List<EditionRawScriptableDataPair> DataByEdition = new List<EditionRawScriptableDataPair>();
        public EditionRawScriptableDataPair FifthEdition => DataByEdition.Find(data => data.Edition.Equals(kDungeonsAndDragons5thEditionKey));
        public EditionRawScriptableDataPair Pf1Edition => DataByEdition.Find(data => data.Edition.Equals(kPathfinder1stEditionKey));
        public EditionRawScriptableDataPair Pf2Edition => DataByEdition.Find(data => data.Edition.Equals(kPathfinder2ndEditionKey));


        [Space(4f)] public List<PageStyleConfiguration> PageStyleConfigurations = new List<PageStyleConfiguration>();
        
        public void ManualValidate()
        {
            DataByEdition.RemoveAll(data => data.Edition.Equals("5e"));
            if (!DataByEdition.Exists(data => data.Edition.Equals(kDungeonsAndDragons5thEditionKey)))
            {
                DataByEdition.Add(new EditionRawScriptableDataPair
                {
                    Edition = kDungeonsAndDragons5thEditionKey
                });
            }
            
            var find = DataByEdition.Find(data => data.Edition.Equals(kDungeonsAndDragons5thEditionKey));
            find.Title = Title;
            find.AsJson = AsJson;
            find.DataHash = DataHash;
        }

        public virtual void ParseSource(string source)
        {
            var find = DataByEdition.Find(data => data.Edition.Equals(kDungeonsAndDragons5thEditionKey));
            if (find != null)
            {
                find.Content = source;
            }

            Debug.Log($"Parsed: {source}");
        }
        
        protected virtual JObject FindItemInCollection<T>(List<T> collection, string editionKey, string sourceID, string itemID) where T : ScriptableData
        {
            foreach (var item in collection)
            {
                if (!item.name.Equals(itemID.AsSlug()))
                    continue;
                
                var findByEdition = item.DataByEdition.Find(data => data.Edition.Equals(editionKey));
                if (findByEdition == null)
                    continue;

                if (findByEdition.SourceBook != null && !findByEdition.SourceBook.ID.Equals(sourceID))
                    continue;
                
                return JsonConvert.DeserializeObject<JObject>(findByEdition.AsJson);
            }

            return null;
        }

        protected virtual string BuiltStyle_Title(string title)
        {
            return $"<style=\"PaperPage_Title\">{title}</style>";
        }

        protected virtual string BuiltStyle_Separator()
        {
            return $"<style=\"PaperPage_Separator\"></style>";
        }

        protected virtual string BuiltStyle_SubTitle(string subTitle)
        {
            return $"<style=\"PaperPage_SubTitle\">{subTitle}</style>";
        }
        
        protected virtual string BuiltStyle_RedProperties(string property)
        {
            return $"<style=\"PaperPage_RedProperties\">{property}</style>";
        }

        [Serializable]
        public class PageStyleConfiguration
        {
            public string Title;
            public PageStyle Style;
            public PageTransparency PageTransparency;
            public Sprite Background;
        }
        
        [Serializable]
        public class EditionRawScriptableDataPair
        {
            public string Edition;
            [Space(2f)]
            public string Title;
            [ResizableTextArea]
            public string Content;
            [ResizableTextArea]
            public string StatsBlock;
            public bool IsSRD;
            public SourceBookData SourceBook;
            [Header("Data")]
            public string AsJson;
            public string DataHash;
        }

        [Serializable]
        public enum OrcDataType
        {
            Unknown,
            Backgrounds,
            Books,
            Classes,
            Feats,
            Items,
            Languages,
            Monsters,
            Races,
            Spells
        }
    }
}

#if UNITY_EDITOR
    
namespace ORC
{
    using UnityEditor;
    
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
            
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

            if (GUILayout.Button("Validate")) 
            {
                m_target.ManualValidate();
            }

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            
            base.OnInspectorGUI();
        }
    }
}
    
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ORC
{
    public class ScriptableData : OrcScriptableObject
    {
        public string ID => this.name;
        public string Title;
        public string AsJson;
        public string DataHash;
        
        [Header("----")]
        public List<EditionRawScriptableDataPair> DataByEdition = new List<EditionRawScriptableDataPair>();

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

            Debug.Log("Parsed: " + source);
        }
        
        [Serializable]
        public class EditionRawScriptableDataPair
        {
            public string Edition;
            [Space(2f)]
            public string Title;
            [TextArea(15, 20)]
            public string Content;
            [Header("Data")]
            public string AsJson;
            public string DataHash;
        }
    }
}

#if UNITY_EDITOR
    
namespace ORC
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

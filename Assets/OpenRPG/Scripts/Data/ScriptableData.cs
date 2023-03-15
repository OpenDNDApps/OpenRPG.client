using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace OpenRPG
{
    public class ScriptableData : OrcScriptableObject
    {
        public string Title;
        public string AsJson;
        public string DataHash;
        
        public virtual void ParseSource(string source) {
            Debug.Log("Parsed: " + source);
        }
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
    
    public static class ScriptableDataExtensions
    {
        public static T GetByID<T>(this List<T> list, string id) where T : ScriptableData
        {
            return list.Find(item => item.name.Equals(id));
        }
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

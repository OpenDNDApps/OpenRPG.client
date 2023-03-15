using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VGDevs;

namespace OpenRPG
{
    [CreateAssetMenu(fileName = GameResources.kRPGDataFileName, menuName = kBaseScriptableDataPath + GameResources.kRPGDataFileName)]
    public partial class RPGDataCollection : ScriptableData
    {
        public List<SourceFilesPair> SourceFiles = new List<SourceFilesPair>();
        
        public List<SourceBookData> SourceBooks = new List<SourceBookData>();
        public List<FeatData> Feats = new List<FeatData>();
        
        public virtual void ParseFeats(string filePath, string creationFolderPath) => ParseSourceFile(filePath, "name", "feat", creationFolderPath, ref Feats);
        public virtual void ParseBooks(string filePath, string creationFolderPath) => ParseSourceFile(filePath, "id", "book", creationFolderPath, ref SourceBooks);
    }

    [Serializable]
    public struct SourceFilesPair
    {
        public string Title;
        public string FilePath;
        public string TargetFolderPath;
        public string ParsingMethodName;
    }
}

#if UNITY_EDITOR

namespace OpenRPG
{
    using UnityEditor;
    
    [CustomEditor(typeof(RPGDataCollection), true)]
    public class RPGDataCollectionEditor : Editor
    {
        protected RPGDataCollection m_target;
        private const string kDataFilesFolder = "Assets/StreamingAssets/DataFiles/";
        
        private void OnEnable()
        {
            m_target = (RPGDataCollection)target;
        }

        public override void OnInspectorGUI() {
            m_target = (RPGDataCollection)target;

            foreach (SourceFilesPair pair in m_target.SourceFiles)
            {
                if (string.IsNullOrEmpty(pair.FilePath)) 
                    continue;
                
                if (GUILayout.Button($"Parse {pair.Title}"))
                {
                    if (AssetDatabase.IsValidFolder(pair.TargetFolderPath))
                    {
                        m_target.GetType().GetMethod(pair.ParsingMethodName)?.Invoke(m_target, new object[] {pair.FilePath, pair.TargetFolderPath});
                    }
                    else
                    {
                        Debug.LogError($"Invalid target folder path: '{pair.TargetFolderPath}'");
                    }
                }

                GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            }

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            
            base.OnInspectorGUI();
        }
    }
}

#endif

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

namespace ORC
{
    [CreateAssetMenu(fileName = GameResources.kOrcDataFileName, menuName = kBaseScriptableDataPath + GameResources.kOrcDataFileName)]
    public partial class OrcDataCollection : OrcScriptableObject
    {
        public List<SourceFilesPair> SourceFiles = new List<SourceFilesPair>();
        
        public List<CharacterBackgroundData> Backgrounds = new List<CharacterBackgroundData>();
        public List<SourceBookData> SourceBooks = new List<SourceBookData>();
        public List<CharacterClassData> Classes = new List<CharacterClassData>();
        public List<FeatData> Feats = new List<FeatData>();
        public List<ItemData> Items = new List<ItemData>();
        public List<LanguageData> Languages = new List<LanguageData>();
        public List<MonsterData> Monsters = new List<MonsterData>();
        public List<CharacterRaceData> Races = new List<CharacterRaceData>();
        public List<SkillData> Skills = new List<SkillData>();
        public List<SpellData> Spells = new List<SpellData>();

        public virtual void ParseFeats(string filePath, string creationFolderPath) => ParseSourceFile(filePath, "name", "feat", creationFolderPath, ref Feats);
        public virtual void ParseBooks(string filePath, string creationFolderPath) => ParseSourceFile(filePath, "id", "book", creationFolderPath, ref SourceBooks);
        public virtual void ParseSpells(string filePath, string creationFolderPath) => ParseSourceFolder(filePath, "name", "spell", creationFolderPath, ref Spells);
        public virtual void ParseMonsters(string filePath, string creationFolderPath) => ParseSourceFolder(filePath, "name", "monster", creationFolderPath, ref Monsters);
        public virtual void ParseBackgrounds(string filePath, string creationFolderPath) => ParseSourceFile(filePath, "name", "background", creationFolderPath, ref Backgrounds);
        public virtual void ParseClasses(string filePath, string creationFolderPath) => ParseSourceFolder(filePath, "name", "class", creationFolderPath, ref Classes);
        public virtual void ParseRaces(string filePath, string creationFolderPath) => ParseSourceFile(filePath, "name", "race", creationFolderPath, ref Races);
        public virtual void ParseItems(string filePath, string creationFolderPath) => ParseSourceFile(filePath, "name", "item", creationFolderPath, ref Items);
        public virtual void ParseSkills(string filePath, string creationFolderPath) => ParseSourceFile(filePath, "name", "skill", creationFolderPath, ref Skills);
        public virtual void ParseLanguages(string filePath, string creationFolderPath) => ParseSourceFile(filePath, "name", "language", creationFolderPath, ref Languages);
        
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

        public static void ParseSourceFolder<T>(string folderPath, string idKey, string rootKey, string creationPath, ref List<T> existingData) where T : ScriptableData
        {
            string[] filePaths = Directory.GetFiles(folderPath);

            foreach (string filePath in filePaths)
            {
                if(!filePath.EndsWith(".json"))
                    continue;
                ParseSourceFile(filePath, idKey, rootKey, creationPath, ref existingData);
            }
        }
        #endif
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

namespace ORC
{
    using UnityEditor;
    
    [CustomEditor(typeof(OrcDataCollection), true)]
    public class RPGDataCollectionEditor : Editor
    {
        protected OrcDataCollection m_target;
        private const string kDataFilesFolder = "Assets/StreamingAssets/DataFiles/";
        
        private void OnEnable()
        {
            m_target = (OrcDataCollection)target;
        }

        public override void OnInspectorGUI() {
            m_target = (OrcDataCollection)target;

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

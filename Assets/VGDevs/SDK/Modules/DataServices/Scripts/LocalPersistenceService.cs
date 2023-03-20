using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VGDevs
{
    public class LocalPersistenceService : IPersistenceService, IDisposable
    {
        private const int kSetsBeforeFlush = 1;

        private readonly LocalPersistenceSerializerSettings m_serializerSettings = new LocalPersistenceSerializerSettings();
        private Counter m_flushCounter = null;

        public void Initialize()
        {
            m_flushCounter = new Counter(kSetsBeforeFlush, Flush, true);
            Application.quitting += Flush;
        }

        private readonly Dictionary<string, IPersistentElement> m_loadedStateElements = new Dictionary<string, IPersistentElement>();

        private string GetSavePathForElement(string element) => Path.Combine(Application.persistentDataPath, element + ".json");
        
        private string GetRoot(params string[] keys) => string.Join(".", keys);

        private string Serialize<T>(T toSerialize)
        {
            if (toSerialize is IPersistentElement persistentElement)
            {
                persistentElement.OnBeforeSerialize();
            }
            return JsonConvert.SerializeObject(toSerialize, m_serializerSettings);
        }
        private T Deserialize<T>(string toDeserialize)
        {
            var toReturn = JsonConvert.DeserializeObject<T>(toDeserialize, m_serializerSettings);
            return toReturn;
        }

        public T Get<T>(T toOverride, params string[] keys) where T : IPersistentElement
        {
            string root = GetRoot(keys);

            string jsonData = string.Empty;
            if (m_loadedStateElements.ContainsKey(root))
            {
                jsonData = Serialize(m_loadedStateElements[root]);
            }

            string filePath = GetSavePathForElement(root);
            if(string.IsNullOrEmpty(jsonData) && !File.Exists(filePath))
            {
                return default;
            }

            jsonData = File.ReadAllText(GetSavePathForElement(root));
            JObject overrideData = Deserialize<JObject>(jsonData);
            JsonUtility.FromJsonOverwrite(overrideData.ToString(), toOverride);
            toOverride.OnAfterDeserialize();

            return toOverride;
        }
        
        public T Get<T>(params string[] keys) where T : IPersistentElement
        {
            string root = GetRoot(keys);

            if (m_loadedStateElements.ContainsKey(root))
            {
                return (T) m_loadedStateElements[root];
            }

            string elementFilePath = GetSavePathForElement(root);
            if (!File.Exists(elementFilePath))
            {
                return default(T);
            }

            try
            {
                string jsonData = File.ReadAllText(Path.Combine(Application.persistentDataPath, root + ".json"));
                T elementToLoad = Deserialize<T>(jsonData);
                elementToLoad.OnAfterDeserialize();

                m_loadedStateElements.Add(root, elementToLoad);

                return elementToLoad;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Set<T>(T element, params string[] path) where T : IPersistentElement
        {
            string root = GetRoot(path);

            m_loadedStateElements[root] = element;

            m_flushCounter.Add();
        }

        public void Flush()
        {
            foreach (string element in m_loadedStateElements.Keys)
            {
                Flush(element);
            }
        }

        public void Flush(params string[] path)
        {
            string root = path.Length == 1 ? path[0] : GetRoot(path);
            string fullPath = GetSavePathForElement(root);
            string state = Serialize(m_loadedStateElements[root]);
            StreamWriter writer = new StreamWriter(fullPath);
            writer.Write(state);
            writer.Close();

            Debug.Log($"Saved state {fullPath}");
        }

        private void ReleaseUnmanagedResources()
        {
            Application.quitting -= Flush;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~LocalPersistenceService()
        {
            ReleaseUnmanagedResources();
        }
    }
}
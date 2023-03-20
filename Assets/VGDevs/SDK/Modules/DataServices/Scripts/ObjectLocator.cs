using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VGDevs
{
    [Serializable]
    public class ObjectLocator<TBase> : IPersistentElement
    {   
        [JsonProperty] private Dictionary<Type, TBase> m_stateInstances = new Dictionary<Type, TBase>();

        public ObjectLocator(string id) => PersistenceID = id;

        public TConcrete GetObject<TConcrete>() where TConcrete : TBase, new()
        {
            Type stateType = typeof(TConcrete);
            if (m_stateInstances.ContainsKey(stateType))
            {
                return (TConcrete)m_stateInstances[stateType];
            }

            TConcrete stateInstance = new TConcrete();
            m_stateInstances.Add(stateType, stateInstance);

            return stateInstance;
        }

        public string PersistenceID { get; }
        public void OnAfterDeserialize() { }

        public void OnBeforeSerialize() { }
    }
}
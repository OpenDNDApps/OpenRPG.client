using System;
using System.Collections.Generic;
using UnityEngine;

namespace VGDevs
{
    [CreateAssetMenu(fileName = GameResources.kUserFileName, menuName = GameResources.kCreateMenuPrefixNameResources + GameResources.kUserFileName)]
    public class UserResourcesCollection : BaseResourcesCollection
    {
        [SerializeField] private UserRuntime m_userRuntime;

        public List<UserDataRecordsKeys> InitialRecords = new List<UserDataRecordsKeys>();
        
        public UserRuntime UserRuntime => m_userRuntime;
        
        private void OnValidate()
        {
            if (m_userRuntime != null)
            {
                AddLoadablePrefabs(m_userRuntime.gameObject);
            }
        }

        [Serializable]
        public struct UserDataRecordsKeys
        {
            public string Key;
            public string Value;
        }
    }

}
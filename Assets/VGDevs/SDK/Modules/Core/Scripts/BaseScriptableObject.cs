using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGDevs
{
    public class BaseScriptableObject : ScriptableObject
    {
        
        #if UNITY_EDITOR
        [ContextMenu("Ping in Project Folder")]
        private void SelectInProjectFolder()
        {
            UnityEditor.EditorGUIUtility.PingObject(this);
        }
        #endif
    }
}

using UnityEngine;

namespace VGDevs
{
    public static partial class GameResources
    {
        public static UIRuntime UIRuntime => UIRuntime.Instance;
        
        public const string kUIFileName = "UIGameResources";
        
        private static GameUIResourcesCollection m_ui;
        public static GameUIResourcesCollection UI => GetGameResource(ref m_ui, kUIFileName);
    }
}

#if UNITY_EDITOR
#region Editor top menu methods
namespace VGDevs
{
    using UnityEditor;
    public static partial class CoreEditor
    {
        [MenuItem(kModuleMenuPath + "Select UI")]
        private static void SelectGameUIResources()
        {
            Selection.activeObject = GameResources.UI;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
    }
}
#endregion
#endif

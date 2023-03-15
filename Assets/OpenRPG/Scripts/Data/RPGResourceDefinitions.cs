using OpenRPG;

namespace VGDevs
{
    public static partial class GameResources
    {
        public const string kRPGDataFileName = "RPGData";
        
        private static RPGDataCollection m_rpgData;
        public static RPGDataCollection RPGData => GetGameResource(ref m_rpgData, kRPGDataFileName);
    }
}

#if UNITY_EDITOR
#region Editor top menu methods
namespace VGDevs
{
    using UnityEditor;
    public static partial class CoreEditor
    {
        public const string kOpenRPGMenuPath = "OpenRPG/";
        
        [MenuItem(kOpenRPGMenuPath + "Select RPG Data")]
        private static void SelectRPGData()
        {
            Selection.activeObject = GameResources.RPGData;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
    }
}
#endregion
#endif
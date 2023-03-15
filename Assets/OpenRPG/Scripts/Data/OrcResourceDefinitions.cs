using OpenRPG;

namespace VGDevs
{
    public static partial class GameResources
    {
        public const string kOrcDataFileName = "OrcData";
        
        private static OrcDataCollection m_orcData;
        public static OrcDataCollection OrcData => GetGameResource(ref m_orcData, kOrcDataFileName);
    }
}

#if UNITY_EDITOR
#region Editor top menu methods
namespace VGDevs
{
    using UnityEditor;
    public static partial class CoreEditor
    {
        public const string kOpenRPGMenuPath = "ORC/";
        
        [MenuItem(kOpenRPGMenuPath + "Select ORC Data")]
        private static void SelectRPGData()
        {
            Selection.activeObject = GameResources.OrcData;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
    }
}
#endregion
#endif
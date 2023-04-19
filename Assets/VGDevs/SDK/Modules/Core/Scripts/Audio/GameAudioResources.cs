namespace VGDevs
{
    public static partial class GameResources
    {
        public static AudioRuntime AudioRuntime => AudioRuntime.Instance;
        
        public const string kGameAudioFileName = "GameAudioResources";
        
        private static GameAudioResourcesCollection m_audio;
        public static GameAudioResourcesCollection Audio => GetGameResource(ref m_audio, kGameAudioFileName);
    }
}

#if UNITY_EDITOR
#region Editor top menu methods
namespace VGDevs
{
    using UnityEditor;
    public static partial class CoreEditor
    {
        [MenuItem(kModuleMenuPath + "Select Audio")]
        private static void SelectGameAudioResources()
        {
            Selection.activeObject = GameResources.Audio;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
    }
}
#endregion
#endif
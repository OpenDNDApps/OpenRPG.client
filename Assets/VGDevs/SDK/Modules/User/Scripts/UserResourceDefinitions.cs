namespace VGDevs
{
    public static partial class GameResources
    {
        public static UserRuntime UserRuntime => UserRuntime.Instance;
        
        public const string kUserFileName = "UserResources";
        
        private static UserResourcesCollection m_user;
        public static UserResourcesCollection User => GetGameResource(ref m_user, kUserFileName);
        
    }
}

#if UNITY_EDITOR
#region Editor top menu methods
namespace VGDevs
{
    using UnityEditor;
    public static partial class CoreEditor
    {
        [MenuItem(kModuleMenuPath + "Select User")]
        private static void SelectUserResources()
        {
            Selection.activeObject = GameResources.User;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
    }
}
#endregion
#endif
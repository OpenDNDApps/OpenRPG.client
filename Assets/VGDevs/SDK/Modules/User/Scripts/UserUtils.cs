using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGDevs
{
    public static class UserUtils
    {
        public static string GetDeviceID()
        {
            #if UNITY_EDITOR
            if (!string.IsNullOrEmpty(GameResources.Settings.User.DebugDeviceOverrideID))
                return GameResources.Settings.User.DebugDeviceOverrideID;
            #endif
            
            #if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            string deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");
            return deviceId;
            #elif (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
            return SystemInfo.deviceUniqueIdentifier;
            #else
            return SystemInfo.deviceUniqueIdentifier;
            #endif
        }
    }
}

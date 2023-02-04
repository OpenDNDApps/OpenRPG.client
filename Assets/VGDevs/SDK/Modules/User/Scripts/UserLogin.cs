using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace VGDevs
{
    public static class UserLogin
    {
        private static UserResourcesCollection User => GameResources.User;
        
        public static event Action<UserData> OnSilentLoginSuccess;
        public static event Action<UserData> OnFullLoginSuccess;
        public static event Action<PlayFabError> OnAPIError;

        public static bool IsClientLoggedIn => PlayFabClientAPI.IsClientLoggedIn();
        public static bool IsClientLoggedInFull => PlayFabClientAPI.IsClientLoggedIn() && IsLoggedInFull;

        private const string kLoginRememberKey = "AuthLoginRemember";
        private const string kLastAuthTypeRememberKey = "LastAuthTypeRemember";
        private const string kPlayFabRememberMeIdKey = "AuthIdPassGuid";
        
        public static bool IsLoggedInFull { get; set; }

        public static void AutoAuth()
        {
            switch (AuthType)
            {
                case UserAccount.AuthType.RememberLogin:
                    DoRememberLogin();
                break;
                case UserAccount.AuthType.Undefined:
                case UserAccount.AuthType.Silent:
                default:
                    DoSilentLogin();
                break;
            }
        }

        public static void DoSilentLogin(Action<LoginResult> callback = null)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest
            {
                AndroidDevice = SystemInfo.deviceModel,
                OS = SystemInfo.operatingSystem,
                AndroidDeviceId = UserUtils.GetDeviceID(),
            #elif (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
            PlayFabClientAPI.LoginWithIOSDeviceID(new LoginWithIOSDeviceIDRequest
            {
                DeviceModel = SystemInfo.deviceModel, 
                OS = SystemInfo.operatingSystem,
                DeviceId = UserUtils.GetDeviceID(),
            #else
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
            {
                CustomId = UserUtils.GetDeviceID(),
            #endif
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = true,
                InfoRequestParameters = GameResources.Settings.User.InfoRequestParams
            }, result => {
                HandleOnSilentLoginSuccessLogic(result);
                callback?.Invoke(result);
            }, (error) => {
                HandleOnAPIError(error);
                callback?.Invoke(null);
            });
        }

        private static void HandleOnSilentLoginSuccessLogic(LoginResult result)
        {
            UserAccount.UserData.SetFromLogin(result);

            AuthType = UserAccount.AuthType.Silent;
            
            OnSilentLoginSuccess?.Invoke(UserAccount.UserData);
        }

        public static void UnlinkSilentAuth()
        {
            DoSilentLogin(result =>
            {
                #if UNITY_ANDROID && !UNITY_EDITOR
                PlayFabClientAPI.UnlinkAndroidDeviceID(new UnlinkAndroidDeviceIDRequest() {
                    AndroidDeviceId = UserUtils.GetDeviceID()
                }, null, null);
                #elif UNITY_IPHONE || UNITY_IOS && !UNITY_EDITOR
                PlayFabClientAPI.UnlinkIOSDeviceID(new UnlinkIOSDeviceIDRequest() {
                    DeviceId = UserUtils.GetDeviceID()
                }, null, null);
                #else
                PlayFabClientAPI.UnlinkCustomID(new UnlinkCustomIDRequest() {
                    CustomId = UserUtils.GetDeviceID()
                }, null, null);
                #endif
            });
        }

        public static void DoRememberLogin()
        {
            if (!RememberMe || string.IsNullOrEmpty(RememberMeId))
            {
                HandleOnAPIError(new PlayFabError(){ ErrorMessage = "Error trying to login.", Error = PlayFabErrorCode.Unknown });
                return;
            }

            DoSilentLogin(result =>
            {
                UserAccount.UserData.SetFromLogin(result);
                
                PlayFabClientAPI.LoginWithCustomID(
                    new LoginWithCustomIDRequest
                    {
                        TitleId = PlayFabSettings.TitleId,
                        CustomId = RememberMeId,
                        CreateAccount = true,
                        InfoRequestParameters = GameResources.Settings.User.InfoRequestParams
                    },
                    HandleOnRememberLoginSuccess,
                    HandleOnAPIError
                );
            });
        }

        private static void HandleOnRememberLoginSuccess(LoginResult result)
        {
            UserAccount.UserData.SetFromLogin(result);

            AuthType = UserAccount.AuthType.RememberLogin;
            IsLoggedInFull = true;

            OnFullLoginSuccess?.Invoke(UserAccount.UserData);
        }

        public static void DoManualLogin(UserData userName)
        {
            PlayFabClientAPI.LoginWithEmailAddress(
                new LoginWithEmailAddressRequest
                {
                    TitleId = PlayFabSettings.TitleId,
                    Email = userName.Email,
                    Password = userName.Password,
                    InfoRequestParameters = GameResources.Settings.User.InfoRequestParams
                },
                HandleOnManualLoginSuccess,
                HandleOnAPIError
            );
        }

        private static void HandleOnManualLoginSuccess(LoginResult result)
        {
            UserAccount.UserData.SetFromLogin(result);

            if (RememberMe)
            {
                RememberMeId = Guid.NewGuid().ToString();

                PlayFabClientAPI.LinkCustomID(
                    new LinkCustomIDRequest
                    {
                        CustomId = RememberMeId,
                        ForceLink = GameResources.Settings.User.ForceLink
                    },
                    HandleOnLinkCustomID,
                    HandleOnAPIError
                );
            }

            OnFullLoginSuccess?.Invoke(UserAccount.UserData);
        }

        private static void HandleOnLinkCustomID(LinkCustomIDResult result)
        {
            AuthType = UserAccount.AuthType.RememberLogin;
            IsLoggedInFull = true;
        }

        public static void HandleOnAPIError(PlayFabError error)
        {
            Debug.LogError(error);
            
            error.GenerateErrorReport();
            OnAPIError?.Invoke(error);
            
            UIRuntime.TryShowGenericError(error.ErrorMessage);
        }
        
        public static UserAccount.AuthType AuthType
        {
            get => (UserAccount.AuthType)PlayerPrefs.GetInt(kLastAuthTypeRememberKey, 0);
            set => PlayerPrefs.SetInt(kLastAuthTypeRememberKey, (int) value);
        }

        public static bool RememberMe
        {
            get => PlayerPrefs.GetInt(kLoginRememberKey, 0) != 0;
            set => PlayerPrefs.SetInt(kLoginRememberKey, value ? 1 : 0);
        }

        public static string RememberMeId
        {
            // TODO: Encrypt this...
            get => PlayerPrefs.GetString(kPlayFabRememberMeIdKey, "");
            set
            {
                var guid = value ?? Guid.NewGuid().ToString();
                PlayerPrefs.SetString(kPlayFabRememberMeIdKey, guid);
            }
        }

        public static void ClearRememberMe()
        {
            PlayerPrefs.DeleteKey(kLoginRememberKey);
            PlayerPrefs.DeleteKey(kPlayFabRememberMeIdKey);
        }

        public static bool IsValidData(this UserData userData, UserAccount.AuthType authType, out string message)
        {
            message = "";
            if (authType.Equals(UserAccount.AuthType.EmailAndPassword))
            {
                if (string.IsNullOrEmpty(userData.Username) || string.IsNullOrEmpty(userData.Email))
                {
                    message = "Missing Data.";
                    return false;
                }
            }
            if (authType.Equals(UserAccount.AuthType.RegisterManual))
            {
                if (string.IsNullOrEmpty(userData.Username) || string.IsNullOrEmpty(userData.Email))
                {
                    message = "Missing Data.";
                    return false;
                }

                if (userData.Username.Length < GameResources.Settings.User.UsernameLengthMinMax.x)
                {
                    message = "Username is too short";
                    return false;
                }

                if (userData.Username.Length > GameResources.Settings.User.UsernameLengthMinMax.y)
                {
                    message = $"Username is too long: Max lenght: '{GameResources.Settings.User.UsernameLengthMinMax.y}'";
                    return false;
                }
            }

            if (userData.Password.Length < GameResources.Settings.User.PasswordLengthMinMax.x)
            {
                message = "Password is too short";
                return false;
            }

            if (userData.Password.Length > GameResources.Settings.User.PasswordLengthMinMax.y)
            {
                message = $"Password is too long: Max lenght: '{GameResources.Settings.User.PasswordLengthMinMax.y}'";
                return false;
            }

            if (!userData.Email.IsValidEmail())
            {
                message = "Invalid Email";
                return false;
            }
            
            return true;
        }
    }
}

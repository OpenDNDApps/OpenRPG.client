using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace VGDevs
{
    public static class UserRegister
    {
        public static event Action<UserData> OnRegisterSuccess;
        public static event Action<PlayFabError> OnAPIError;

        public static void DoRegisterAccount(UserData userData)
        {
            if (!userData.IsValidData(UserAccount.AuthType.RegisterManual, out string errorMessage))
            {
                HandleOnAPIError(new PlayFabError
                {
                    Error = PlayFabErrorCode.UserisNotValid,
                    ErrorMessage = errorMessage
                });
                return;
            }

            var request = new AddUsernamePasswordRequest
            {
                Username = userData.Username,
                Email = userData.Email,
                Password = userData.Password,
            };

            PlayFabClientAPI.AddUsernamePassword(
                request,
                result => { HandleOnRegisterSuccess(result, userData); },
                HandleOnAPIError
            );
        }

        private static void HandleOnRegisterSuccess(AddUsernamePasswordResult result, UserData newUserData)
        {
            UserAccount.UserData.Username = result.Username;
            UserAccount.UserData.Email = newUserData.Email;
            UserAccount.UserData.Password = newUserData.Password;
            
            if (UserLogin.RememberMe)
            {
                UserLogin.RememberMeId = Guid.NewGuid().ToString();
                
                var request = new LinkCustomIDRequest
                {
                    CustomId = UserLogin.RememberMeId,
                    ForceLink = GameResources.Settings.User.ForceLink
                };
                
                PlayFabClientAPI.LinkCustomID(request, result => { UserLogin.AuthType = UserAccount.AuthType.RememberLogin; }, null);
            }
            
            UserAccount.DoSetUserDisplayName(result.Username);

            UserLogin.IsLoggedInFull = true;
            OnRegisterSuccess?.Invoke(UserAccount.UserData);
        }

        private static void HandleOnAPIError(PlayFabError error)
        {
            Debug.LogError(error);
            
            error.GenerateErrorReport();
            OnAPIError?.Invoke(error);
            
            UIRuntime.TryShowGenericError(error.ErrorMessage);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace VGDevs
{
    public static class UserAccount
    {
        public static UserData UserData;
        
        public static event Action<UserData> OnUserDataChanged;
        public static event Action<PlayFabError> OnAPIError;
        public static bool IsReady { get; set; }

        public enum AuthType
        {
            Undefined,
            Silent,
            EmailAndPassword,
            RememberLogin,
            RegisterManual
        }

        public static void GetAccountStatistics()
        {
            var request = new GetPlayerStatisticsRequest()
            {
                StatisticNames = GameResources.Settings.User.InfoRequestParams.PlayerStatisticNames
            };
            PlayFabClientAPI.GetPlayerStatistics(request, HandleOnGetAccountCustomData, HandleOnAPIError);
        }

        private static void HandleOnGetAccountCustomData(GetPlayerStatisticsResult result)
        {
            if (!UserData.SetStatistics(result.Statistics))
                return;

            OnUserDataChanged?.Invoke(UserData);
        }

        public static void GetAccountCustomData()
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), HandleOnGetAccountCustomData, HandleOnAPIError);
        }

        private static void HandleOnGetAccountCustomData(GetUserDataResult result)
        {
            UserData.SetCustomData(result.Data);
            OnUserDataChanged?.Invoke(UserData);
        }

        public static void GetAccountCurrencyData()
        {
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), HandleOnGetAccountCurrencyData, HandleOnAPIError);
        }

        private static void HandleOnGetAccountCurrencyData(GetUserInventoryResult result)
        {
            UserData.SetVirtualCurrency(result.VirtualCurrency);
            OnUserDataChanged?.Invoke(UserData);
        }

        public static void DoFirstTimeSetup()
        {
            // TODO: Move this to Resources. Refactor stats find
            var data = new Dictionary<string, string>
            {
                { "Level", "1" }
            };
            var statsRequest = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate{ StatisticName = "Score", Value = 0 },
                    new StatisticUpdate{ StatisticName = "Kills", Value = 0 },
                    new StatisticUpdate{ StatisticName = "Deaths", Value = 0 },
                }
            };
            var dataRequest = new UpdateUserDataRequest
            {
                Data = data,
                Permission = UserDataPermission.Private
            };
            PlayFabClientAPI.UpdateUserData(dataRequest, result => { 
                HandleOnFirstTimeSetupSuccess(data, statsRequest); 
            }, HandleOnAPIError);
            PlayFabClientAPI.UpdatePlayerStatistics(statsRequest, result =>
            {
                HandleOnFirstTimeSetupSuccess(data, statsRequest);
            }, HandleOnAPIError);
        }

        private static void HandleOnFirstTimeSetupSuccess(Dictionary<string, string> customData, UpdatePlayerStatisticsRequest stats)
        {
            // TODO: Move this to Resources. Refactor stats find.
            UserData.Level = customData["Level"];
            UserData.Score = stats.Statistics.Find(stat => stat.StatisticName.Equals("Score")).Value.ToString();
            UserData.Kills = stats.Statistics.Find(stat => stat.StatisticName.Equals("Kills")).Value.ToString();
            UserData.Deaths = stats.Statistics.Find(stat => stat.StatisticName.Equals("Deaths")).Value.ToString();
            IsReady = true;
        }

        public static void DoSetUserDisplayName(string username)
        {
            UserData.Username = username;

            PlayFabClientAPI.UpdateUserTitleDisplayName(
                new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = username
                },
                (UpdateUserTitleDisplayNameResult result) =>
                {
                    UserData.DisplayName = result.DisplayName;
                    Debug.Log("UpdateUserTitleDisplayName completed.");
                    OnUserDataChanged?.Invoke(UserData);
                },
                HandleOnAPIError
            );
        }

        public static void HandleOnAPIError(PlayFabError error)
        {
            Debug.LogError(error);
            
            error.GenerateErrorReport();
            OnAPIError?.Invoke(error);
            
            UIRuntime.TryShowGenericError(error.ErrorMessage);
        }
    }
}

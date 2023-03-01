using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace VGDevs
{
    using static GameResources;
    
    public class UserRuntime : MonoSingletonSelfGenerated<UserRuntime>, IGameDependency
    {
        public bool IsReady => UserAccount.IsReady;

        private const string kDebugAuthWindow = "UIDebugAuthWindow";
        
        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();

            if (Settings.User.ShowDebugWindow)
            {
                UserLogin.OnSilentLoginSuccess += ShowDebugWindow;
            }
            
            UserLogin.AutoAuth();
        }

        private void ShowDebugWindow(UserData userData)
        {
            if (!UIRuntime.TryShowWindow(kDebugAuthWindow, out UIDebugAuthWindow debugAuthWindow))
                return;

            debugAuthWindow.Build(userData);
            debugAuthWindow.AnimatedShow();
        }
    }
        
    [Serializable]
    public struct UserData
    {
        public string ID;
        public string DisplayName;
        public string Username;
        public string Email;
        public string Password;
        public string SessionTicket;
        public string Score;
        public string Kills;
        public string Deaths;
        public string Level;
        public bool NewlyCreated;
        public int SoftCoin;
        public int HardCoin;

        public void SetFromLogin(LoginResult result)
        {
            ID = result.PlayFabId;
            SessionTicket = result.SessionTicket;
            Username = result.InfoResultPayload.AccountInfo?.Username ?? "";
            NewlyCreated = result.NewlyCreated;

            SetVirtualCurrency(result.InfoResultPayload.UserVirtualCurrency);
            
            if (NewlyCreated)
            {
                UserAccount.DoFirstTimeSetup();
            }
            else
            {
                SetCustomData(result.InfoResultPayload?.UserData);
                SetStatistics(result.InfoResultPayload?.PlayerStatistics);

                UserAccount.IsReady = true;
            }
        }

        public bool SetVirtualCurrency(Dictionary<string, int> userVirtualCurrency)
        {
            SoftCoin = userVirtualCurrency[Settings.User.SoftCoinID];
            HardCoin = userVirtualCurrency[Settings.User.HardCoinID];

            return true;
        }

        public bool SetCustomData(Dictionary<string, UserDataRecord> customData)
        {
            if (customData.ContainsKey("Level")) {
                Level = customData["Level"].Value;
            }
            return true;
        }

        public bool SetStatistics(List<StatisticValue> stats)
        {
            bool hasChanged = false;
            if (stats == null)
                return hasChanged;
            
            // var scoreStat = stats.Find(stat => stat.StatisticName != null && stat.StatisticName.Equals("Score")).Value.ToString();
            // if (!string.IsNullOrEmpty(scoreStat)) {
            //     Score = scoreStat;
            //     hasChanged = true;
            // }
            // var killStat = stats.Find(stat => stat.StatisticName != null && stat.StatisticName.Equals("Kills")).Value.ToString();
            // if (!string.IsNullOrEmpty(killStat)) {
            //     Kills = killStat;
            //     hasChanged = true;
            // }
            // var deathsStat = stats.Find(stat => stat.StatisticName != null && stat.StatisticName.Equals("Deaths")).Value.ToString();
            // if (!string.IsNullOrEmpty(deathsStat)) {
            //     Deaths = deathsStat;
            //     hasChanged = true;
            // }

            return hasChanged;
        }
    }
}
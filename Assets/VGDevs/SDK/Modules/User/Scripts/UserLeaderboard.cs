using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace VGDevs
{
    public static class UserLeaderboard
    {
        public static void GetLeaderboard(Action<GetLeaderboardResult> onComplete, Action<PlayFabError> onError)
        {
            PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest()
            {
                StatisticName = "Score",
                StartPosition = 0,
                MaxResultsCount = 10
            }, onComplete, onError);
        }
    }
}
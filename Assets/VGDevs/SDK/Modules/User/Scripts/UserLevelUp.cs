using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using VGDevs;

public class UserLevelUp : MonoBehaviour
{
    public static event Action<PlayFabError> OnAPIError;
    public static event Action<LevelUpTryStatus> OnLevelUpTryComplete;
    public static event Action<LevelUpTryStatus> OnResetCooldownTryComplete;

    public enum LevelUpTryStatus
    {
        Success,
        On_Cooldown,
        Error
    }

    public static bool IsAbleToLevelUp()
    {
        return UserAccount.UserData.SoftCoin >= GetLevelUpPrice();
    }

    public static bool IsAbleToUnlockInstant()
    {
        return UserAccount.UserData.HardCoin >= GetLevelUpPriceInstant();
    }

    public static int GetLevelUpPrice()
    {
        if (!int.TryParse(UserAccount.UserData.Level, out int level))
            level = 1;
        
        return level * 10;
    }

    public static int GetLevelUpPriceInstant()
    {
        return 10;
    }
    
    public static void TryLevelUp()
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "LevelUp",
            GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request, HandleOnCallComplete, HandleOnAPIError);
    }

    private static void HandleOnCallComplete(ExecuteCloudScriptResult result)
    {
        var actualResult = result.AsCloudScriptResult<LevelUpTryStatus>();
        OnLevelUpTryComplete?.Invoke(actualResult.type);
    }
    
    public static void TryLevelUpResetCooldown()
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "LevelUpResetCooldown",
            GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request, HandleResetCooldownOnCallComplete, HandleOnAPIError);
    }

    private static void HandleResetCooldownOnCallComplete(ExecuteCloudScriptResult result)
    {
        var actualResult = result.AsCloudScriptResult<LevelUpTryStatus>();
        if (actualResult.type.Equals(LevelUpTryStatus.Success))
        {
            TryLevelUp();
        }
        OnResetCooldownTryComplete?.Invoke(actualResult.type);
    }

    public static void HandleOnAPIError(PlayFabError error)
    {
        Debug.LogError(error);
            
        error.GenerateErrorReport();
        OnAPIError?.Invoke(error);
            
        UIRuntime.TryShowGenericError(error.ErrorMessage);
    }
}
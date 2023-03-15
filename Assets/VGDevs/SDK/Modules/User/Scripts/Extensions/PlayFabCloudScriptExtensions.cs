using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using PlayFab.Json;

namespace VGDevs
{
    public static class PlayFabCloudScriptExtensions
    {
        public static CloudScriptResult<T> AsCloudScriptResult<T>(this ExecuteCloudScriptResult result) where T : struct
        {
            var newResult = new CloudScriptResult<T>();
            T typeValue;
            
            if (result.Error != null)
            {
                newResult.message = result.Error.ToString();
                Enum.TryParse("Error", true, out typeValue);
                newResult.type = typeValue;
                return newResult;
            }

            var resultAsObject = (JsonObject) result.FunctionResult;
            resultAsObject.TryGetValue("message", out object message);
            resultAsObject.TryGetValue("error", out object error);
        
            if (!string.IsNullOrEmpty(error as string))
            {
                newResult.message = (string) error;
                Enum.TryParse(error as string, true, out typeValue);
                newResult.type = typeValue;
                return newResult;
            }

            newResult.message = message as string;
            Enum.TryParse(message as string, true, out typeValue);
            newResult.type = typeValue;
            return newResult;
        }
    }

    public class CloudScriptResult<T> where T : struct
    {
        public string message;
        public T type;
    }
}

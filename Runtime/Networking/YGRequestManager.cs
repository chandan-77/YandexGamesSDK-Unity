using System;
using System.Collections.Generic;
using UnityEngine;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Logging;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Networking
{
    public static class YGRequestManager_
    {
        private static Dictionary<string, Delegate> _callbacks = new Dictionary<string, Delegate>();
        
        public static string GenerateRequestId()
        {
            return Guid.NewGuid().ToString();
        }
        
        public static void RegisterCallback<T>(string requestId, Action<bool, T, string> callback) where T : class
        {
            if (_callbacks.ContainsKey(requestId))
            {
                YGLogger.Warning($"Request ID {requestId} already registered. Overwriting.");
            }
            
            _callbacks[requestId] = callback;
        }
        
        public static void HandleJSResponse(string jsonResponse)
        {
            try
            {
                JSResponse response = JsonUtility.FromJson<JSResponse>(jsonResponse);
                
                if (response == null)
                {
                    YGLogger.Error("Failed to parse JS response: Response is null");
                    return;
                }
                
                if (string.IsNullOrEmpty(response.requestId))
                {
                    YGLogger.Error("Failed to parse JS response: RequestId is null or empty");
                    return;
                }
                
                if (!_callbacks.TryGetValue(response.requestId, out var callbackDelegate))
                {
                    YGLogger.Warning($"No callback found for request ID: {response.requestId}");
                    return;
                }
                
                Type callbackType = callbackDelegate.GetType();
                Type[] genericArgs = callbackType.GetGenericArguments();
                
                if (genericArgs.Length > 0)
                {
                    Type dataType = genericArgs[1]; // Second type parameter of Action<bool, T, string>
                    
                    object data = null;
                    if (response.status && response.data != null)
                    {
                        string dataJson = response.data.ToString();
                        if (!string.IsNullOrEmpty(dataJson))
                        {
                            try
                            {
                                // Special case for string type
                                if (dataType == typeof(string))
                                {
                                    data = dataJson;
                                }
                                else
                                {
                                    data = JsonUtility.FromJson(dataJson, dataType);
                                }
                            }
                            catch (Exception e)
                            {
                                YGLogger.Error($"Failed to deserialize data to {dataType.Name}: {e.Message}");
                            }
                        }
                    }
                    
                    // Invoke the callback with the appropriate parameters
                    callbackDelegate.DynamicInvoke(response.status, data, response.error);
                }
                
                // Remove the callback after invocation
                _callbacks.Remove(response.requestId);
            }
            catch (Exception e)
            {
                YGLogger.Error($"Error handling JS response: {e.Message}\nJSON: {jsonResponse}");
            }
        }
    }
    
    [Serializable]
    public class JSResponse
    {
        public string requestId;
        public bool status;
        public object data;
        public string error;
    }
    
    [Serializable]
    public class JSResponse<T>
    {
        public string requestId;
        public bool status;
        public T data;
        public string error;
    }
}
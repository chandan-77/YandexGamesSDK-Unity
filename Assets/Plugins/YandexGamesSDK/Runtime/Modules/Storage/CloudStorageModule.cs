using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Networking;
using UnityEngine;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Storage
{
    public class CloudStorageModule : YGModuleBase, ICloudStorageModule
    {
        public override void Initialize()
        {
        }

        public void Save<TData>(string key, TData data, Action<bool, string> callback = null)
        {
            string requestId = YGRequestManager.GenerateRequestId();

            YGRequestManager.RegisterCallback<string>(requestId, (success, _, error) =>
            {
                callback?.Invoke(success, error);
            });

            string jsonData;

            try
            {
                jsonData = JsonConvert.SerializeObject(data);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Serialization error: {ex.Message}");
                callback?.Invoke(false, $"Serialization error: {ex.Message}");
                return;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
        SavePlayerData(requestId, key, jsonData);
#else
            Debug.Log("Cloud storage is only available in WebGL builds.");
            callback?.Invoke(false, "Cloud storage is only available in WebGL builds.");
#endif
        }

        public void Save(string key, object data, Action<bool, string> callback = null)
        {
            Save<object>(key, data, callback);
        }

        public void Load<T>(string key, Action<bool, T, string> callback = null)
        {
            string requestId = YGRequestManager.GenerateRequestId();

            // Register the callback
            YGRequestManager.RegisterCallback<T>(requestId, callback);

#if UNITY_WEBGL && !UNITY_EDITOR
         LoadPlayerData(requestId, key);
#else
            Debug.Log("Cloud storage is only available in WebGL builds.");
            callback?.Invoke(false, default(T), "Cloud storage is only available in WebGL builds.");
#endif
        }

        public void Load(string key, Action<bool, object, string> callback = null)
        {
            Load<object>(key, callback);
        }

       
        [DllImport("__Internal")]
        private static extern void SavePlayerData(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string requestId,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string key,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string data);

        [DllImport("__Internal")]
        private static extern void LoadPlayerData(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string requestId,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string key);
    }
}
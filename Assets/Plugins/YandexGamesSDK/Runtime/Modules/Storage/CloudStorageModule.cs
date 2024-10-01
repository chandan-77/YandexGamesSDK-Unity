using System;
using System.Runtime.InteropServices;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using UnityEngine;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Storage
{
    public class CloudStorageModule : YGModuleBase, ICloudStorageModule
    {
        private Action<bool, string> saveCallback;
        private Action<bool, string> loadCallback;

        [DllImport("__Internal")]
        private static extern void SavePlayerData(string key, string data);

        [DllImport("__Internal")]
        private static extern void LoadPlayerData(string key);


        public override void Initialize()
        {
        }

        public void SaveData(string key, object data, Action<bool, string> callback = null)
        {
            saveCallback = callback;

            string jsonData = JsonUtility.ToJson(data);
            
#if UNITY_WEBGL && !UNITY_EDITOR
            SavePlayerData(key, jsonData);
#else
            Debug.Log("Cloud storage is only available in WebGL builds.");
            callback?.Invoke(false, "Cloud storage is only available in WebGL builds.");
#endif
        }

        public void LoadData(string key, Action<bool, string> callback = null)
        {
            loadCallback = callback;

#if UNITY_WEBGL && !UNITY_EDITOR
            LoadPlayerData(key);
#else
            Debug.Log("Cloud storage is only available in WebGL builds.");
            callback?.Invoke(false, "Cloud storage is only available in WebGL builds.");
#endif
        }

        // Called from JavaScript when data is successfully saved
        private void OnPlayerDataSaved(string message)
        {
            bool success = message.StartsWith("true");
            string error = success ? null : message.Split('|')[1];
            saveCallback?.Invoke(success, error);
        }

        // Called from JavaScript when data is successfully loaded
        private void OnPlayerDataLoaded(string message)
        {
            bool success = !message.StartsWith("false");
            string dataOrError = success ? message : message.Split('|')[1];
            loadCallback?.Invoke(success, dataOrError);
        }
    }
}
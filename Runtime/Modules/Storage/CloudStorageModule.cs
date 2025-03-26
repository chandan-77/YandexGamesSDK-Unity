using AOT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Logging;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.LocalStorage;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Networking;
using UnityEngine;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Storage
{
    public class CloudStorageModule : YGModuleBase, ICloudStorageModule
    {
        protected Dictionary<string, string> dataCache = new Dictionary<string, string>();
        protected readonly object cacheLock = new object();
        protected Dictionary<string, long> dataVersions = new Dictionary<string, long>();
        protected bool isDirty = false;

        protected ILocalStorageModule localStorage;

        private const string DataCacheKey = "CloudStorage_DataCache";
        private const string DataVersionsKey = "CloudStorage_DataVersions";

        #region Static Callbacks
        private static Action<bool, string> s_saveCallback;
        private static Action<bool, string> s_loadAllCallback;
        private static Action<bool, object, string> s_loadCallback;
        private static CloudStorageModule s_instance;

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HandleSaveResponse(string jsonResponse)
        {
            var response = JsonConvert.DeserializeObject<JSResponse<string>>(jsonResponse);
            s_saveCallback?.Invoke(response.status, response.error);
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HandleLoadResponse(string jsonResponse)
        {
            var response = JsonConvert.DeserializeObject<JSResponse<string>>(jsonResponse);
            if (response.status && !string.IsNullOrEmpty(response.data) && response.data != "{}")
            {
                try
                {
                    var backendData = JsonConvert.DeserializeObject<Dictionary<string, VersionedData>>(response.data);
                    if (backendData != null)
                    {
                        s_instance.ResolveConflicts(backendData);
                        s_instance.SaveToLocalStorage();
                        YGLogger.Debug("Cloud data loaded and merged with local cache");
                        s_loadAllCallback?.Invoke(true, null);
                        
                        // If there's a specific load callback waiting
                        if (s_loadCallback != null)
                        {
                            s_instance.HandleLoadCallback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    YGLogger.Error($"Deserialization error: {ex.Message}");
                    s_loadAllCallback?.Invoke(false, $"Deserialization error: {ex.Message}");
                    s_loadCallback?.Invoke(false, null, $"Deserialization error: {ex.Message}");
                }
            }
            else if (response.status)
            {
                YGLogger.Debug("No data found in cloud storage");
                s_loadAllCallback?.Invoke(true, null);
                s_loadCallback?.Invoke(false, null, "No data found");
            }
            else
            {
                s_loadAllCallback?.Invoke(false, response.error);
                s_loadCallback?.Invoke(false, null, response.error);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HandleErrorResponse(string errorJson)
        {
            var response = JsonConvert.DeserializeObject<JSResponse>(errorJson);
            s_saveCallback?.Invoke(false, response.error);
            s_loadAllCallback?.Invoke(false, response.error);
            s_loadCallback?.Invoke(false, null, response.error);
        }
        #endregion

        #region DllImports
        [DllImport("__Internal")]
        private static extern void CloudStorageApi_SavePlayerData(string key, string dataJson, 
            Action<string> successCallback, Action<string> errorCallback);

        [DllImport("__Internal")]
        private static extern void CloudStorageApi_LoadPlayerData(string key, 
            Action<string> successCallback, Action<string> errorCallback);
        #endregion

        public override void Initialize()
        {
            s_instance = this;
            YGLogger.Debug("Cloud Storage Module initialized");
            
            localStorage = YandexGamesSDK.Instance.LocalStorage;
            LoadFromLocalStorage();
            LoadAllDataFromBackend(null);
        }

        public virtual void Save<T>(string key, T data, Action<bool, string> callback = null)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(data);
                string encryptedData = EncryptData(jsonData);

                lock (cacheLock)
                {
                    dataCache[key] = encryptedData;
                    dataVersions[key] = DateTime.UtcNow.Ticks;
                    isDirty = true;
                }

                SaveToLocalStorage();

                callback?.Invoke(true, null);
            }
            catch (Exception ex)
            {
                YGLogger.Error($"Serialization error for key '{key}': {ex.Message}");
                callback?.Invoke(false, $"Serialization error: {ex.Message}");
            }
        }

        public virtual void Save(string key, object data, Action<bool, string> callback = null)
        {
            Save<object>(key, data, callback);
        }

        public virtual void Load<T>(string key, Action<bool, T, string> callback)
        {
            currentLoadKey = key;
            currentLoadType = typeof(T);
            s_loadCallback = (success, obj, error) =>
            {
                callback?.Invoke(success, success ? (T)obj : default, error);
            };

            LoadAllDataFromBackend(null);
        }

        public virtual void Load(string key, Action<bool, object, string> callback = null)
        {
            Load<object>(key, callback);
        }

        public virtual void FlushData(Action<bool, string> callback = null)
        {
            if (!isDirty)
            {
                YGLogger.Info("No data to flush.");
                callback?.Invoke(true, null);
                return;
            }

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                YGLogger.Warning("No internet connection. Flush postponed.");
                callback?.Invoke(false, "No internet connection.");
                return;
            }

            Dictionary<string, VersionedData> dataToFlush;

            lock (cacheLock)
            {
                dataToFlush = new Dictionary<string, VersionedData>();
                foreach (var key in dataCache.Keys)
                {
                    dataToFlush[key] = new VersionedData
                    {
                        Data = dataCache[key],
                        Version = dataVersions.ContainsKey(key) ? dataVersions[key] : 0
                    };
                }
            }

            SaveToBackend(dataToFlush, callback);
        }

        private void SaveToBackend(Dictionary<string, VersionedData> data, Action<bool, string> callback)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            if (!YandexGamesSDK.IsInitialized)
            {
                YGLogger.Error("SDK is not initialized. Call Initialize first.");
                callback?.Invoke(false, "SDK is not initialized");
                return;
            }

            s_saveCallback = callback;
            string jsonData = JsonConvert.SerializeObject(data);
            CloudStorageApi_SavePlayerData("yg_data", jsonData, HandleSaveResponse, HandleErrorResponse);
            #else
            YGLogger.Info("Cloud storage is only available in WebGL builds.");
            callback?.Invoke(false, "Cloud storage is only available in WebGL builds.");
            #endif
        }

        private void LoadAllDataFromBackend(Action<bool, string> callback = null)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            if (!YandexGamesSDK.IsInitialized)
            {
                YGLogger.Error("SDK is not initialized. Call Initialize first.");
                callback?.Invoke(false, "SDK is not initialized");
                return;
            }

            s_loadAllCallback = callback;
            CloudStorageApi_LoadPlayerData("yg_data", HandleLoadResponse, HandleErrorResponse);
            #else
            YGLogger.Info("Cloud storage is only available in WebGL builds.");
            callback?.Invoke(false, "Cloud storage is only available in WebGL builds.");
            #endif
        }

        private void HandleLoadCallback()
        {
            if (s_loadCallback == null) return;

            try
            {
                string encryptedData = dataCache[currentLoadKey];
                string jsonData = DecryptData(encryptedData);
                var data = JsonConvert.DeserializeObject(jsonData, currentLoadType);
                s_loadCallback.Invoke(true, data, null);
            }
            catch (Exception ex)
            {
                YGLogger.Error($"Deserialization error: {ex.Message}");
                s_loadCallback.Invoke(false, null, $"Deserialization error: {ex.Message}");
            }
            finally
            {
                currentLoadKey = null;
                currentLoadType = null;
                s_loadCallback = null;
            }
        }

        private string currentLoadKey;
        private Type currentLoadType;

        private void ResolveConflicts(Dictionary<string, VersionedData> backendData)
        {
            lock (cacheLock)
            {
                foreach (var kvp in backendData)
                {
                    if (dataVersions.TryGetValue(kvp.Key, out long localVersion))
                    {
                        if (kvp.Value.Version > localVersion)
                        {
                            dataCache[kvp.Key] = kvp.Value.Data;
                            dataVersions[kvp.Key] = kvp.Value.Version;
                        }
                        // Else, keep local data
                    }
                    else
                    {
                        // Data exists in backend but not locally, add it
                        dataCache[kvp.Key] = kvp.Value.Data;
                        dataVersions[kvp.Key] = kvp.Value.Version;
                    }
                }
            }
        }

        private void SaveToLocalStorage()
        {
            lock (cacheLock)
            {
                string dataCacheJson = JsonConvert.SerializeObject(dataCache);
                string dataVersionsJson = JsonConvert.SerializeObject(dataVersions);

                string encryptedDataCache = EncryptData(dataCacheJson);
                string encryptedDataVersions = EncryptData(dataVersionsJson);

                localStorage.Save(DataCacheKey, encryptedDataCache);
                localStorage.Save(DataVersionsKey, encryptedDataVersions);
            }
        }

        private void LoadFromLocalStorage()
        {
            string encryptedDataCache = localStorage.Load(DataCacheKey);
            string encryptedDataVersions = localStorage.Load(DataVersionsKey);

            if (!string.IsNullOrEmpty(encryptedDataCache) && !string.IsNullOrEmpty(encryptedDataVersions))
            {
                try
                {
                    string dataCacheJson = DecryptData(encryptedDataCache);
                    string dataVersionsJson = DecryptData(encryptedDataVersions);

                    var loadedDataCache = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataCacheJson);
                    var loadedDataVersions = JsonConvert.DeserializeObject<Dictionary<string, long>>(dataVersionsJson);

                    lock (cacheLock)
                    {
                        dataCache = loadedDataCache ?? new Dictionary<string, string>();
                        dataVersions = loadedDataVersions ?? new Dictionary<string, long>();
                    }
                    
                    YGLogger.Debug("Loaded data from local storage");
                }
                catch (Exception ex)
                {
                    YGLogger.Error($"Error loading from local storage: {ex.Message}");
                    // Initialize with empty dictionaries
                    lock (cacheLock)
                    {
                        dataCache = new Dictionary<string, string>();
                        dataVersions = new Dictionary<string, long>();
                    }
                }
            }
            else
            {
                YGLogger.Debug("No local storage data found");
            }
        }

        private string EncryptData(string data)
        {
            // In a real implementation, you might want to encrypt the data here
            // For simplicity, we're just returning the original data
            return data;
        }

        private string DecryptData(string data)
        {
            // In a real implementation, you might want to decrypt the data here
            // For simplicity, we're just returning the original data
            return data;
        }

        [Serializable]
        private class VersionedData
        {
            public string Data;
            public long Version;
        }

        private void OnDestroy()
        {
            s_saveCallback = null;
            s_loadAllCallback = null;
            s_loadCallback = null;
            s_instance = null;
        }
    }
}
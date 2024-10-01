using System;
using System.Runtime.InteropServices;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Advertisement;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Authentication;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Leaderboard;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Storage;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Types;
using UnityEngine;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime
{
    [DefaultExecutionOrder(-100)]
    public class YandexGamesSDK : MonoBehaviour
    {
        private static YandexGamesSDK _instance;

        public static YandexGamesSDK Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<YandexGamesSDK>();

                    if (_instance == null)
                    {
                        GameObject sdkObject = new GameObject(nameof(YandexGamesSDK));
                        _instance = sdkObject.AddComponent<YandexGamesSDK>();
                    }
                }

                return _instance;
            }
        }

        [DllImport("__Internal")]
        private static extern void OnYandexGamesSDKReady();

        [DllImport("__Internal")]
        private static extern void GetServerTime();

        [DllImport("__Internal")]
        private static extern void GetEnvironment();

        private Action<bool, TimeSpan> getServerTimeCallback;
        private Action<bool, EnvironmentData> getEnvironmetCallback;


        public IAuthenticationModule Authentication { get; private set; }
        public ICloudStorageModule CloudStorage { get; private set; }
        public ILeaderboardModule Leaderboard { get; private set; }
        public IAdvertisementModule Advertisement { get; private set; }

        private bool _isInitialized = false;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }

#if !UNITY_EDITOR && !UNITY_EDITOR
        OnYandexGamesSDKReady();
#endif

            InitializeModules();
        }

        public void GetServerTime(Action<bool, TimeSpan> callback)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            getServerTimeCallback = callback;
            GetServerTime();
#else
            Debug.Log("GetServerTime is only available in WebGL builds.");
            callback(false, TimeSpan.Zero);
#endif
        }

        public void GetEnvironment(Action<bool, EnvironmentData> callback)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            getEnvironmetCallback = callback;
            GetEnvironment();
#else
            Debug.Log("GetEnvironment is only available in WebGL builds.");
            callback(false, null);
#endif
        }

        public void OnGetServerTimeSuccess(string serverTime)
        {
            Debug.Log($"Server Time Retrieved: {serverTime}");

            if (TimeSpan.TryParse(serverTime, out TimeSpan serverTimeParsed))
            {
                getServerTimeCallback?.Invoke(true, serverTimeParsed);
                return;
            }

            Debug.Log($"Failed to parse server time: {serverTime}");
        }

        public void OnGetServerTimeFailure(string error)
        {
            Debug.LogError($"Failed to get server time: {error}");

            getServerTimeCallback?.Invoke(false, TimeSpan.Zero);
        }

        public void OnGetEnvironmentSuccess(string environment)
        {
            Debug.Log($"Environment Retrieved: {environment}");

            var data = JsonUtility.FromJson<Root>(environment);

            Debug.Log($"Environment Retrieved: {JsonUtility.ToJson(data.env)}");
            getEnvironmetCallback?.Invoke(true, data.env);
        }

        public void OnGetEnvironmentFailure(string error)
        {
            Debug.LogError($"Failed to get environment: {error}");

            getEnvironmetCallback?.Invoke(false, null);
        }

        public void OnSDKInitialized(string message)
        {
            if (message == "true")
            {
                Debug.Log("Yandex SDK initialized successfully in Unity.");
                _isInitialized = true;
            }
            else
            {
                string errorMessage = message.StartsWith("false|") ? message.Substring(6) : "Unknown error";
                Debug.LogError("Yandex SDK initialization failed in Unity: " + errorMessage);
                // Handle initialization failure accordingly
            }
        }

        private void InitializeModules()
        {
            Authentication = LoadAndInitializeModule<AuthenticationModule>();
            Advertisement = LoadAndInitializeModule<AdvertisementModule>();
            Leaderboard = LoadAndInitializeModule<LeaderboardModule>();
            CloudStorage = LoadAndInitializeModule<CloudStorageModule>();
        }

        private TModule LoadAndInitializeModule<TModule>() where TModule : YGModuleBase
        {
            var module = GetComponent<TModule>() ?? gameObject.AddComponent<TModule>();

            module.Initialize();

            return module;
        }
    }
}
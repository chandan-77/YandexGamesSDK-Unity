using System.Runtime.InteropServices;
using UnityEngine;
using Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using Plugins.YandexGamesSDK.Runtime.Modules.Authentication;
using Plugins.YandexGamesSDK.Runtime.Modules.Leaderboard;
using Plugins.YandexGamesSDK.Runtime.Modules.Storage;

namespace Plugins.YandexGamesSDK.Runtime
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

        public IAuthenticationModule Authentication { get; private set; }
        public ICloudStorageModule CloudStorage { get; private set; }
        public ILeaderboardModule Leaderboard { get; private set; }

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

#if !UNITY_EDITOR
        OnYandexGamesSDKReady();
#endif

            InitializeModules();
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
            // Advertisement = LoadAndInitializeModule<AdvertisementModule>();
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
using Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using Plugins.YandexGamesSDK.Runtime.Modules.Advertisement;
using Plugins.YandexGamesSDK.Runtime.Modules.Authentication;
using Plugins.YandexGamesSDK.Runtime.Modules.Leaderboard;
using Plugins.YandexGamesSDK.Runtime.Modules.Storage;
using UnityEngine;

namespace Plugins.YandexGamesSDK.Runtime
{
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

        public IAuthenticationModule Authentication { get; private set; }
        public IAdvertisementModule Advertisement { get; private set; }
        public ILeaderboardModule Leaderboard { get; private set; }
        public ICloudStorageModule CloudStorage { get; private set; }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeModules();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeModules()
        {
            Authentication = LoadAndInitializeModule<AuthenticationModule>();
            // Advertisement = LoadAndInitializeModule<AdvertisementModule>();
            // Leaderboard = LoadAndInitializeModule<LeaderboardModule>();
            // CloudStorage = LoadAndInitializeModule<CloudStorageModule>();
        }

        private TModule LoadAndInitializeModule<TModule>() where TModule : YGModuleBase
        {
            var module = GetComponent<TModule>() ?? gameObject.AddComponent<TModule>();

            module.Initialize();

            return module;
        }
    }
}
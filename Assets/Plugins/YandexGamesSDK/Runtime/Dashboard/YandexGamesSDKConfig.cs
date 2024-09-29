using System.Collections.Generic;
using Plugins.YandexGamesSDK.Runtime.Singletons;
using UnityEngine;

namespace Plugins.YandexGamesSDK.Runtime.Dashboard
{
    [CreateAssetMenu(fileName = "YandexGamesSDKConfig", menuName = "Yandex Games SDK/Config", order = 1)]
    public class YandexGamesSDKConfig : ScriptableObjectSingleton<YandexGamesSDKConfig>
    {
        [Header("General Settings")]
        public string appID = "YOUR_GAME_ID";
        public bool useMockData = true;
        public bool isYandexPlatform = false;
        public bool verboseLogging = false;

        [Header("Mock User Profile")]
        public MockUserProfile mockUserProfile;

        [Header("Mock Leaderboard Data")]
        public List<MockLeaderboardEntry> mockLeaderboardEntries;

        // Add other settings as needed
        
        [Header("Development Settings")]
        public bool runLocalServerAfterBuild = false;

        [System.Serializable]
        public class MockUserProfile
        {
            public string id;
            public string name;
            public bool isAuthorized;
            public string avatarUrlSmall;
            public string avatarUrlMedium;
            public string avatarUrlLarge;
        }

        [System.Serializable]
        public class MockLeaderboardEntry
        {
            public string playerId;
            public string playerName;
            public int score;
        }
    }

}
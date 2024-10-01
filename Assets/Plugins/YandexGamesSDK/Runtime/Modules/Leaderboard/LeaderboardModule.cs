using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Types;
using UnityEngine;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Leaderboard
{
    public class LeaderboardModule : YGModuleBase, ILeaderboardModule
    {
        private Action<bool, string> submitScoreCallback;
        private Action<List<LeaderboardEntry>, string> getEntriesCallback;
        private Action<LeaderboardEntry, string> getPlayerEntryCallback;

        [DllImport("__Internal")]
        private static extern void SubmitScore(string leaderboardName, int score);

        [DllImport("__Internal")]
        private static extern void GetLeaderboardEntries(string leaderboardName, int offset, int limit);

        [DllImport("__Internal")]
        private static extern void GetPlayerEntry(string leaderboardName);

        public override void Initialize()
        {
        }

        public void SubmitScore(string leaderboardName, int score, Action<bool, string> callback)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        submitScoreCallback = callback;
        SubmitScore(leaderboardName, score);
#else
            Debug.LogWarning("Leaderboard functionality is only available in WebGL builds.");
            callback?.Invoke(false, "Leaderboard not supported in the Unity Editor.");
#endif
        }

        public void GetLeaderboardEntries(string leaderboardName, int offset, int limit,
            Action<List<LeaderboardEntry>, string> callback)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        getEntriesCallback = callback;
        GetLeaderboardEntries(leaderboardName, offset, limit);
#else
            Debug.LogWarning("Leaderboard functionality is only available in WebGL builds.");
            callback?.Invoke(null, "Leaderboard not supported in the Unity Editor.");
#endif
        }

        public void GetPlayerEntry(string leaderboardName, Action<LeaderboardEntry, string> callback)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        getPlayerEntryCallback = callback;
        GetPlayerEntry(leaderboardName);
#else
            Debug.LogWarning("Leaderboard functionality is only available in WebGL builds.");
            callback?.Invoke(null, "Leaderboard not supported in the Unity Editor.");
#endif
        }

        public void OnSubmitScoreSuccess()
        {
            submitScoreCallback?.Invoke(true, null);
        }

        public void OnSubmitScoreFailure(string error)
        {
            submitScoreCallback?.Invoke(false, error);
        }

        public void OnGetLeaderboardEntriesSuccess(string jsonData)
        {
            var entries = JsonUtility.FromJson<LeaderboardEntriesWrapper>(jsonData).entries;
            getEntriesCallback?.Invoke(entries, null);
        }

        public void OnGetLeaderboardEntriesFailure(string error)
        {
            getEntriesCallback?.Invoke(null, error);
        }

        public void OnGetPlayerEntrySuccess(string jsonData)
        {
            var entry = JsonUtility.FromJson<LeaderboardEntry>(jsonData);
            getPlayerEntryCallback?.Invoke(entry, null);
        }

        public void OnGetPlayerEntryFailure(string error)
        {
            getPlayerEntryCallback?.Invoke(null, error);
        }

        [Serializable]
        private class LeaderboardEntriesWrapper
        {
            public List<LeaderboardEntry> entries;
        }
    }
}
using AOT;
using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Types;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Logging;
using UnityEngine;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Networking;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Leaderboard
{
    public class LeaderboardModule : YGModuleBase, ILeaderboardModule
    {
        #region Static Callbacks
        private static Action<bool, string> s_submitScoreCallback;
        private static Action<LeaderboardEntriesResponse, string> s_entriesCallback;
        private static Action<LeaderboardEntry, string> s_playerEntryCallback;
        private static LeaderboardModule s_instance;

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HandleSubmitScoreResponse(string jsonResponse)
        {
            var response = JsonConvert.DeserializeObject<JSResponse<string>>(jsonResponse);
            if (response.status)
            {
                YGLogger.Debug("Score submitted successfully");
                s_submitScoreCallback?.Invoke(true, null);
            }
            else
            {
                YGLogger.Error($"Failed to submit score: {response.error}");
                s_submitScoreCallback?.Invoke(false, response.error);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HandleEntriesResponse(string jsonResponse)
        {
            var response = JsonConvert.DeserializeObject<JSResponse<string>>(jsonResponse);
            if (response.status && !string.IsNullOrEmpty(response.data))
            {
                try
                {
                    var leaderboardEntries = JsonConvert.DeserializeObject<LeaderboardEntriesResponse>(response.data);
                    YGLogger.Debug($"Fetched {leaderboardEntries?.entries?.Length ?? 0} leaderboard entries");
                    s_entriesCallback?.Invoke(leaderboardEntries, null);
                }
                catch (Exception ex)
                {
                    YGLogger.Error($"Deserialization error: {ex.Message}");
                    s_entriesCallback?.Invoke(null, "Failed to parse leaderboard entries.");
                }
            }
            else
            {
                YGLogger.Error($"Failed to fetch leaderboard entries: {response.error}");
                s_entriesCallback?.Invoke(null, response.error);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HandlePlayerEntryResponse(string jsonResponse)
        {
            var response = JsonConvert.DeserializeObject<JSResponse<string>>(jsonResponse);
            if (response.status)
            {
                if (response.data == "null")
                {
                    YGLogger.Debug("Player has no entry in leaderboard");
                    s_playerEntryCallback?.Invoke(null, null);
                    return;
                }

                if (!string.IsNullOrEmpty(response.data))
                {
                    try
                    {
                        var playerEntry = JsonConvert.DeserializeObject<LeaderboardEntry>(response.data);
                        YGLogger.Debug($"Fetched player entry: Rank {playerEntry.rank}, Score {playerEntry.score}");
                        s_playerEntryCallback?.Invoke(playerEntry, null);
                    }
                    catch (Exception ex)
                    {
                        YGLogger.Error($"Deserialization error: {ex.Message}");
                        s_playerEntryCallback?.Invoke(null, "Failed to parse player entry.");
                    }
                }
                else
                {
                    s_playerEntryCallback?.Invoke(null, null);
                }
            }
            else
            {
                YGLogger.Error($"Failed to fetch player entry: {response.error}");
                s_playerEntryCallback?.Invoke(null, response.error);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HandleErrorResponse(string errorJson)
        {
            var response = JsonConvert.DeserializeObject<JSResponse>(errorJson);
            s_submitScoreCallback?.Invoke(false, response.error);
            s_entriesCallback?.Invoke(null, response.error);
            s_playerEntryCallback?.Invoke(null, response.error);
        }
        #endregion

        #region DllImports
        [DllImport("__Internal")]
        private static extern void LeaderboardApi_SubmitScore(string leaderboardName, int score, string extraData, 
            Action<string> successCallback, Action<string> errorCallback);

        [DllImport("__Internal")]
        private static extern void LeaderboardApi_GetLeaderboardEntries(string leaderboardName, int quantityTop, 
            int quantityAround, bool includeUser, Action<string> successCallback, Action<string> errorCallback);

        [DllImport("__Internal")]
        private static extern void LeaderboardApi_GetPlayerEntry(string leaderboardName, 
            Action<string> successCallback, Action<string> errorCallback);
        #endregion

        public override void Initialize()
        {
            s_instance = this;
            YGLogger.Debug("Leaderboard Module initialized");
        }

        public void SubmitScore(string leaderboardName, int score, string extraData = null, Action<bool, string> callback = null)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            if (!YandexGamesSDK.IsInitialized)
            {
                YGLogger.Error("SDK is not initialized. Call Initialize first.");
                callback?.Invoke(false, "SDK is not initialized");
                return;
            }

            s_submitScoreCallback = callback;
            LeaderboardApi_SubmitScore(leaderboardName, score, extraData ?? "", 
                HandleSubmitScoreResponse, HandleErrorResponse);
            #else
            YGLogger.Debug($"Mock: Submitting score: {score} to leaderboard: {leaderboardName}");
            callback?.Invoke(false, "Submitting scores is not supported in this build.");
            #endif
        }

        /// <summary>
        /// Retrieves leaderboard entries.
        /// </summary>
        /// <param name="leaderboardName">The name of the leaderboard.</param>
        /// <param name="quantityTop">Number of top entries to fetch.</param>
        /// <param name="quantityAround">Number of entries around the player.</param>
        /// <param name="includeUser">Whether to include the user's entry.</param>
        /// <param name="callback">Callback invoked upon completion with leaderboard data.</param>
        public void GetLeaderboardEntries(string leaderboardName, int quantityTop, int quantityAround, bool includeUser, Action<LeaderboardEntriesResponse, string> callback = null)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            if (!YandexGamesSDK.IsInitialized)
            {
                YGLogger.Error("SDK is not initialized. Call Initialize first.");
                callback?.Invoke(null, "SDK is not initialized");
                return;
            }

            s_entriesCallback = callback;
            LeaderboardApi_GetLeaderboardEntries(leaderboardName, quantityTop, quantityAround, includeUser,
                HandleEntriesResponse, HandleErrorResponse);
            #else
            YGLogger.Debug($"Mock: Fetching leaderboard entries for: {leaderboardName}");
            
            // Create mock leaderboard entries for testing in editor
            var mockResponse = new LeaderboardEntriesResponse
            {
                leaderboard = new LeaderboardDescriptionResponse { name = leaderboardName },
                entries = new LeaderboardEntry[5]
            };
            
            for (int i = 0; i < 5; i++)
            {
                mockResponse.entries[i] = new LeaderboardEntry
                {
                    rank = i + 1,
                    score = 1000 - (i * 100),
                    extraData = "",
                    player = new PlayerEntry
                    {
                        id = $"mock-player-{i}",
                        name = $"Mock Player {i+1}",
                        profilePicture = new PlayerProfilePictureUrls()
                    }
                };
            }
            
            callback?.Invoke(mockResponse, null);
            #endif
        }

        public void GetPlayerEntry(string leaderboardName, Action<LeaderboardEntry, string> callback = null)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            if (!YandexGamesSDK.IsInitialized)
            {
                YGLogger.Error("SDK is not initialized. Call Initialize first.");
                callback?.Invoke(null, "SDK is not initialized");
                return;
            }

            s_playerEntryCallback = callback;
            LeaderboardApi_GetPlayerEntry(leaderboardName, HandlePlayerEntryResponse, HandleErrorResponse);
            #else
            YGLogger.Debug($"Mock: Fetching player entry for leaderboard: {leaderboardName}");
            
            // Create a mock player entry for testing in editor
            var mockEntry = new LeaderboardEntry
            {
                rank = 42,
                score = 850,
                extraData = "",
                player = new PlayerEntry
                {
                    id = "mock-player-self",
                    name = "Mock Player (You)",
                    profilePicture = new PlayerProfilePictureUrls()
                }
            };
            
            callback?.Invoke(mockEntry, null);
            #endif
        }

        private void OnDestroy()
        {
            s_submitScoreCallback = null;
            s_entriesCallback = null;
            s_playerEntryCallback = null;
            s_instance = null;
        }
    }
}

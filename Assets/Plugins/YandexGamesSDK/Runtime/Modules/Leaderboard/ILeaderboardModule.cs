using System;
using System.Collections.Generic;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Types;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Leaderboard
{
    public interface ILeaderboardModule
    {
        void SubmitScore(string leaderboardName, int score, Action<bool, string> callback);

        void GetLeaderboardEntries(string leaderboardName, int offset, int limit,
            Action<List<LeaderboardEntry>, string> callback);

        void GetPlayerEntry(string leaderboardName, Action<LeaderboardEntry, string> callback);
    }
}
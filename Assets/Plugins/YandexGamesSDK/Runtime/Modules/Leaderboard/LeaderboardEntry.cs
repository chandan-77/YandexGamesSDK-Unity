using System;

namespace Plugins.YandexGamesSDK.Runtime.Modules.Leaderboard
{
    [Serializable]
    public record LeaderboardEntry
    {
        public int rank;            // Player's rank on the leaderboard
        public string playerId;     // Unique identifier of the player
        public string name;         // Player's display name
        public int score;           // Player's score
        public string avatarUrl;    // URL to the player's avatar
    }
}
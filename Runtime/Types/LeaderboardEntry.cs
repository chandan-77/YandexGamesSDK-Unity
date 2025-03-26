using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Types
{
    [Serializable]
    public class LeaderboardEntriesResponse
    {
        [field: Preserve]
        public LeaderboardDescriptionResponse leaderboard;
        
        [field: Preserve]
        public Range[] ranges;
        
        [field: Preserve]
        public int userRank;
        
        [field: Preserve]
        public LeaderboardEntry[] entries;
    }

    [Serializable]
    public class LeaderboardDescriptionResponse
    {
        [field: Preserve]
        public string appID;
        
        [field: Preserve]
        public bool @default;
        
        [field: Preserve]
        public Description description;
        
        [field: Preserve]
        public string name;
        
        [field: Preserve]
        public Title title;
    }

    [Serializable]
    public class Description
    {
        [field: Preserve]
        public bool invert_sort_order;
        
        [field: Preserve]
        public ScoreFormat score_format;
        
        [field: Preserve]
        public string type;
    }

    [Serializable]
    public class ScoreFormat
    {
        [field: Preserve]
        public Options options;
    }

    [Serializable]
    public class Options
    {
        [field: Preserve]
        public int decimal_offset;
    }

    [Serializable]
    public class Title
    {
        [field: Preserve]
        public string en;
        
        [field: Preserve]
        public string ru;
    }

    [Serializable]
    public class Range
    {
        [field: Preserve]
        public int start;
        
        [field: Preserve]
        public int size;
    }

    [Serializable]
    public class LeaderboardEntry
    {
        [field: Preserve]
        public int score;
        
        [field: Preserve]
        public string extraData;
        
        [field: Preserve]
        public int rank;
        
        [field: Preserve]
        public PlayerEntry player;
        
        [field: Preserve]
        public string formattedScore;
    }

    [Serializable]
    public class PlayerEntry
    {
        [field: Preserve]
        public string id;
        
        [field: Preserve]
        public string name;
        
        [field: Preserve]
        public PlayerProfilePictureUrls profilePicture;
    }

    [Serializable]
    public class PlayerProfilePictureUrls
    {
        [field: Preserve]
        public string small;
        
        [field: Preserve]
        public string medium;
        
        [field: Preserve]
        public string large;
    }

    [Serializable]
    public class ScopePermissions
    {
        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("public_name")]
        public string PublicName { get; set; }
    }
    
    [Serializable]
    public class LeaderboardDataWrapper
    {
        [JsonProperty("entries")]
        public List<LeaderboardEntry> Entries { get; set; }
    }
}

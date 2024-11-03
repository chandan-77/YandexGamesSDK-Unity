---
icon: ranking-star
---

# Leaderboards

The **Leaderboards** feature in the Yandex Games SDK for Unity allows players to compete by submitting scores and viewing rankings on a shared leaderboard. This can enhance engagement and encourage players to improve their performance.

**Overview**

With Leaderboards, you can:

* **Submit Scores**: Allow players to submit their scores for ranking.
* **Retrieve Top and Surrounding Entries**: Display leaderboard entries for top scores and scores near the user’s rank.
* **Display User Ranking**: Include the user’s current rank in the leaderboard results.

**Basic Usage Examples**

Below are examples for submitting a score, retrieving leaderboard entries with both top and surrounding scores and displaying the player’s ranking.

***

#### **1. Submitting a Score**

To submit a player’s score to the leaderboard, use the leaderboard’s unique identifier (ID) and the player’s score.

```csharp
void SubmitScore()
{
    int playerScore = 120; // Example player score
    string leaderboardId = "testLeaderboard"; // Replace with your leaderboard ID

    YandexGamesSDK.Instance.Leaderboard.SubmitScore(leaderboardId, playerScore, (success, error) =>
    {
        if (success)
        {
            Debug.Log("Score submitted successfully.");
        }
        else
        {
            Debug.LogError($"Failed to submit score: {error}");
        }
    });
}
```

* **Leaderboard ID**: `"testLeaderboard"` is a placeholder; replace it with your actual leaderboard ID.
* **Score**: `playerScore` is the score that will be submitted.
* **Callback**: The callback checks if the score submission was successful.

***

#### **2. Retrieving Leaderboard Entries with Top and Surrounding Scores**

The `GetLeaderboardEntries` method allows you to retrieve both the top scores and scores around the user for a more comprehensive leaderboard view.

```csharp
void GetLeaderboardEntries()
{
    string leaderboardId = "testLeaderboard"; // Replace with your leaderboard ID
    int quantityTop = 5; // Number of top entries to retrieve
    int quantityAround = 5; // Number of entries around the user
    bool includeUser = true; // Include the user's entry

    YandexGamesSDK.Instance.Leaderboard.GetLeaderboardEntries(leaderboardId, quantityTop, quantityAround, includeUser, (entries, error) =>
    {
        if (entries != null)
        {
            Debug.Log("Top Leaderboard Entries:");
            foreach (var entry in entries.TopEntries)
            {
                Debug.Log($"Player: {entry.PlayerName}, Score: {entry.Score}, Rank: {entry.Rank}");
            }

            Debug.Log("Entries Around User:");
            foreach (var entry in entries.AroundUserEntries)
            {
                Debug.Log($"Player: {entry.PlayerName}, Score: {entry.Score}, Rank: {entry.Rank}");
            }

            if (includeUser && entries.UserEntry != null)
            {
                Debug.Log($"User's Entry - Player: {entries.UserEntry.PlayerName}, Score: {entries.UserEntry.Score}, Rank: {entries.UserEntry.Rank}");
            }
        }
        else
        {
            Debug.LogError($"Failed to retrieve leaderboard entries: {error}");
        }
    });
}
```

* **quantityTop**: Specifies the number of top entries to retrieve.
* **quantityAround**: Specifies the number of entries surrounding the user’s rank.
* **includeUser**: Determines whether the user’s own entry should be included in the results.
* **Callback**: Processes both the top entries (`TopEntries`) and the entries around the user (`AroundUserEntries`). If `includeUser` is `true`, the user’s entry is also displayed.

***

#### **3. Displaying Player’s Rank Separately**

If you only need to display the player’s current rank without retrieving other leaderboard entries, you can use `GetPlayerEntry`.

```csharp
void GetPlayerRank()
{
    string leaderboardId = "testLeaderboard"; // Replace with your leaderboard ID

    YandexGamesSDK.Instance.Leaderboard.GetPlayerEntry(leaderboardId, (entry, error) =>
    {
        if (entry != null)
        {
            Debug.Log($"Player Rank: {entry.Rank}, Score: {entry.Score}");
        }
        else
        {
            Debug.LogError($"Failed to retrieve player's leaderboard entry: {error}");
        }
    });
}
```

* **Leaderboard ID**: Use the unique identifier for the leaderboard.
* **Callback**: If the player has an entry, the callback retrieves the player’s `Rank` and `Score`.

***

#### **Additional Tips**

1. **Define Your Leaderboard Goals**: Consider your game’s design and whether to emphasize top ranks or player progression by using `quantityTop` and `quantityAround` wisely.
2. **Personalized Rankings**: Include the user's current rank in the leaderboard to boost engagement and encourage improvement.
3. **Error Handling**: Provide feedback for players if leaderboard features are unavailable (e.g., due to network issues).



> For further details on the JavaScript version of this method, refer to the [Yandex Games SDK JavaScript documentation](https://yandex.ru/dev/games/doc/en/sdk/sdk-leaderboard).

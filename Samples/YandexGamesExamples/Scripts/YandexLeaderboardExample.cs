using UnityEngine;
using UnityEngine.UI;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Types;

public class YandexLeaderboardExample : MonoBehaviour
{
    [SerializeField] private Text playerScoreText;
    [SerializeField] private Transform leaderboardEntriesContainer;
    [SerializeField] private GameObject leaderboardEntryPrefab;
    
    private const string LEADERBOARD_NAME = "highscores";
    private YandexGamesSDK sdk;
    private int currentScore = 0;
    
    private void Awake()
    {
        // Disable the script until SDK is initialized
        enabled = false;
    }
    
    private void OnEnable()
    {
        // Get SDK instance
        sdk = YandexGamesSDK.Instance;
        
        // Perform initialization logic here
        InitializeExample();
    }

    private void InitializeExample()
    {
        if (!YandexGamesInitializer.Instance.CheckSDKAvailability())
            return;
            
        // Initial score display
        UpdateScoreDisplay();
        
        // Load initial leaderboard data
        LoadLeaderboard();
    }
    
    public void AddScore(int points)
    {
        if (!YandexGamesInitializer.Instance.CheckSDKAvailability())
            return;
            
        currentScore += points;
        UpdateScoreDisplay();
        
        sdk.Leaderboard.SubmitScore(LEADERBOARD_NAME, currentScore, null, (success, error) =>
        {
            if (success)
            {
                Debug.Log($"Score {currentScore} submitted successfully!");
                LoadLeaderboard(); // Refresh leaderboard display
            }
            else
                Debug.LogError($"Failed to submit score: {error}");
        });
    }
    
    private void LoadLeaderboard()
    {
        sdk.Leaderboard.GetLeaderboardEntries(
            LEADERBOARD_NAME,
            quantityTop: 5,
            quantityAround: 2,
            includeUser: true,
            (response, error) =>
            {
                if (error == null && response?.entries != null)
                {
                    DisplayLeaderboardEntries(response.entries);
                    LoadPlayerEntry();
                }
                else
                    Debug.LogError($"Failed to load leaderboard: {error}");
            });
    }
    
    private void LoadPlayerEntry()
    {
        sdk.Leaderboard.GetPlayerEntry(LEADERBOARD_NAME, (entry, error) =>
        {
            if (error == null && entry != null)
            {
                Debug.Log($"Player rank: {entry.rank}, Score: {entry.score}");
                UpdatePlayerRankDisplay(entry);
            }
        });
    }
    
    private void DisplayLeaderboardEntries(LeaderboardEntry[] entries)
    {
        // Clear existing entries
        foreach (Transform child in leaderboardEntriesContainer)
            Destroy(child.gameObject);
            
        // Display new entries
        foreach (var entry in entries)
        {
            GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardEntriesContainer);
            
            // Assuming the prefab has these Text components
            entryObj.transform.Find("Rank").GetComponent<Text>().text = $"#{entry.rank}";
            entryObj.transform.Find("Name").GetComponent<Text>().text = entry.player.name;
            entryObj.transform.Find("Score").GetComponent<Text>().text = entry.score.ToString();
            
            // Highlight the player's entry
            if (entry.player.id == "mock-player-self") // In real build, this would match the actual player ID
            {
                entryObj.GetComponent<Image>().color = Color.yellow;
            }
        }
    }
    
    private void UpdateScoreDisplay()
    {
        playerScoreText.text = $"Score: {currentScore}";
    }
    
    private void UpdatePlayerRankDisplay(LeaderboardEntry playerEntry)
    {
        // You might want to display the player's global rank somewhere in your UI
        Debug.Log($"Global Rank: #{playerEntry.rank}");
    }
} 
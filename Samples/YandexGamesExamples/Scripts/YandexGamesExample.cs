using UnityEngine;
using UnityEngine.UI;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Types;

public class YandexGamesExample : MonoBehaviour
{
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text scoreText;
    private YandexGamesSDK sdk;
    
    private void Awake()
    {
        enabled = false;
    }
    
    private void OnEnable()
    {
        sdk = YandexGamesSDK.Instance;

        InitializeExample();
    }

    private void InitializeExample()
    {
        if (!YandexGamesInitializer.Instance.CheckSDKAvailability())
            return;
            
        AuthenticatePlayer();
    }
    
    private void AuthenticatePlayer()
    {
        sdk.Authentication.AuthenticateUser(true, (success, error) =>
        {
            if (success)
            {
                Debug.Log("Player authenticated!");
                LoadPlayerData();
            }
            else
                Debug.LogError($"Authentication failed: {error}");
        });
    }
    
    private void LoadPlayerData()
    {
        var userData = sdk.Authentication.CurrentUser;
        if (userData == null)
            return;

        playerNameText.text = $"Welcome, {userData.name}!";
    }
    
    public void SubmitScore(int score)
    {
        if (!YandexGamesInitializer.Instance.CheckSDKAvailability())
            return;
            
        scoreText.text = $"Score: {score}";

        sdk.Leaderboard.SubmitScore("mainLeaderboard", score, null, (success, error) =>
        {
            if (success)
                Debug.Log("Score submitted successfully!");
            else
                Debug.LogError($"Failed to submit score: {error}");
        });
    }
} 

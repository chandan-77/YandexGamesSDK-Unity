using UnityEngine;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime;
using Newtonsoft.Json;

public class YandexStorageExample : MonoBehaviour
{
    private YandexGamesSDK sdk;
    
    [System.Serializable]
    private class PlayerProgress
    {
        public int level;
        public int coins;
        public float playTime;
    }
    
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
            
        // Load initial progress
        LoadProgress();
    }
    
    public void SaveProgress()
    {
        if (!YandexGamesInitializer.Instance.CheckSDKAvailability())
            return;
            
        var progress = new PlayerProgress
        {
            level = 5,
            coins = 1000,
            playTime = 120.5f
        };
        
        var jsonData = JsonConvert.SerializeObject(progress);

        sdk.CloudStorage.Save("SaveData", jsonData, (success, error) =>
        {
            if (success)
                Debug.Log("Progress saved to cloud!");
            else
                Debug.LogError($"Failed to save progress: {error}");
        });
    }
    
    public void LoadProgress()
    {
        if (!YandexGamesInitializer.Instance.CheckSDKAvailability())
            return;
            
        sdk.CloudStorage.Load<string>("SaveData", (success, data, error) =>
        {
            if (success && !string.IsNullOrEmpty(data))
            {
                var progress = JsonConvert.DeserializeObject<PlayerProgress>(data);
                Debug.Log($"Loaded progress - Level: {progress.level}, Coins: {progress.coins}");
            }
            else
                Debug.LogError($"Failed to load progress: {error}");
        });
    }
} 

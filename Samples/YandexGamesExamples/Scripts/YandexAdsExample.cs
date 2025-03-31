using UnityEngine;
using UnityEngine.UI;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime;

public class YandexAdsExample : MonoBehaviour
{
    [SerializeField] private Button rewardedAdButton;
    [SerializeField] private Button interstitialAdButton;
    [SerializeField] private Text rewardText;
    
    private YandexGamesSDK sdk;

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
            
        // Setup UI elements
        if (rewardedAdButton != null)
        {
            rewardedAdButton.onClick.RemoveAllListeners();
            rewardedAdButton.onClick.AddListener(ShowRewardedAd);
        }
        
        if (interstitialAdButton != null)
        {
            interstitialAdButton.onClick.RemoveAllListeners();
            interstitialAdButton.onClick.AddListener(ShowInterstitial);
        }
        
        if (rewardText != null)
        {
            rewardText.text = "Watch ad for reward!";
        }
    }
    
    public void ShowInterstitial()
    {
        if (!YandexGamesInitializer.Instance.CheckSDKAvailability())
            return;
            
        sdk.Advertisement.ShowInterstitial((success, error) =>
        {
            if (success)
                Debug.Log("Interstitial ad shown successfully!");
            else
                Debug.LogError($"Failed to show interstitial: {error}");
        });
    }
    
    public void ShowRewardedAd()
    {
        if (!YandexGamesInitializer.Instance.CheckSDKAvailability())
            return;
            
        sdk.Advertisement.ShowRewarded((success, error) =>
        {
            if (success)
            {
                Debug.Log("Rewarded ad completed!");
                GiveReward();
            }
            else
                Debug.LogError($"Failed to show rewarded ad: {error}");
        });
    }
    
    private void GiveReward()
    {
        // Example reward implementation
        int coins = 100;
        rewardText.text = $"Rewarded! +{coins} coins";
    }
    
    private void OnDisable()
    {
        // Clean up listeners when disabled
        if (rewardedAdButton != null)
            rewardedAdButton.onClick.RemoveAllListeners();
            
        if (interstitialAdButton != null)
            interstitialAdButton.onClick.RemoveAllListeners();
    }
} 
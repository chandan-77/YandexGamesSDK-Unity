using UnityEngine;
using UnityEngine.Events;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime;

/// <summary>
/// Central initialization point for Yandex Games SDK examples.
/// Attach this script to a GameObject in your example scene.
/// </summary>
public class YandexGamesInitializer : MonoBehaviour
{
    [Header("Example Scripts References")]
    [SerializeField] private YandexGamesExample gamesExample;
    [SerializeField] private YandexAdsExample adsExample;
    [SerializeField] private YandexLeaderboardExample leaderboardExample;
    [SerializeField] private YandexPurchaseExample purchaseExample;
    [SerializeField] private YandexStorageExample storageExample;

    [Header("Events")]
    [SerializeField] private UnityEvent onSDKInitialized;
    [SerializeField] private UnityEvent onSDKInitializationFailed;

    private YandexGamesSDK sdk;
    private bool isInitialized;

    public static YandexGamesInitializer Instance { get; private set; }

    public bool IsInitialized => isInitialized;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        sdk = YandexGamesSDK.Instance;
    }

    private void Start()
    {
        StartCoroutine(InitializeSDK());
    }

    private System.Collections.IEnumerator InitializeSDK()
    {
        Debug.Log("Initializing Yandex Games SDK...");

        yield return sdk.Initialize();

        if (YandexGamesSDK.IsInitialized)
        {
            Debug.Log("Yandex Games SDK initialized successfully!");
            isInitialized = true;

            sdk.SetGameplayReady((success, error) =>
            {
                if (success)
                    Debug.Log("Game is ready to play!");
                else
                    Debug.LogError($"Failed to set game ready: {error}");
            });

            InitializeExamples();

            onSDKInitialized?.Invoke();
        }
        else
        {
            Debug.LogError("Failed to initialize Yandex Games SDK!");
            onSDKInitializationFailed?.Invoke();
        }
    }

    private void InitializeExamples()
    {
        if (gamesExample != null)
            gamesExample.enabled = true;

        if (adsExample != null)
            adsExample.enabled = true;

        if (leaderboardExample != null)
            leaderboardExample.enabled = true;

        if (purchaseExample != null)
            purchaseExample.enabled = true;

        if (storageExample != null)
            storageExample.enabled = true;
    }

    /// <summary>
    /// Helper method to check if SDK is ready before performing operations
    /// </summary>
    public bool CheckSDKAvailability()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("SDK is not initialized yet. Wait for initialization to complete.");
            return false;
        }

        if (!YandexGamesSDK.IsRunningOnYandex)
        {
            Debug.LogWarning("Not running on Yandex Games platform. Some features may not work.");
            return false;
        }

        return true;
    }
} 
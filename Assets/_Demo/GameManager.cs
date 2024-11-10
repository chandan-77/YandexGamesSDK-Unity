using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Advertisement;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public TMP_InputField loadKey;
    public TMP_InputField saveData;
    public TMP_InputField saveKey;
    
    [Serializable]
    public struct PlayerData
    {
        public string playerName;
        public string level;
    }


    public void Load()
    {
        YandexGamesSDK.Instance.CloudStorage.Load(loadKey.text, (success, data, error) =>
        {
            if (success)
            {
                Debug.Log($"Loaded player data: int={data}");
            }
            else
            {
                Debug.LogError($"Failed to load player data: {error}");
            }
        });
    }
    
    public void Save()
    {
        YandexGamesSDK.Instance.CloudStorage.Save(saveKey.text, saveData.text);
    }
    void Start()
    {
        YandexGamesSDK.Instance.Authentication.AuthenticateUser(requireSignin: true, (success, message) =>
        {
            if (!success)
            {
                Debug.LogError(message);
            }
            else
            {
                Debug.Log($"Authenticated user: {YandexGamesSDK.Instance.Authentication.GetUserProfile().name}");
            }
        });
        
        YandexGamesSDK.Instance.CloudStorage.Save<PlayerData>("salam", new PlayerData())
        ;
      
        YandexGamesSDK.Instance.GetServerTime((isFetched, time, error) => { Debug.Log("GetServerTime: " + time); });

        YandexGamesSDK.Instance.GetEnvironment((isFetched, env, errors) =>
        {
            Debug.Log("GetEnvironment-lang: " + env.i18n.lang);
        });

        YandexGamesSDK.Instance.SetGameplayReady();
        YandexGamesSDK.Instance.SetGameplayStart();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            var playerData = new PlayerData { playerName = "TestPlayer", level = "TestLevel" };

            for (int i = 0; i < 10; i++)
            {
                YandexGamesSDK.Instance.CloudStorage.Save($"primit_{i}", Random.Range(0, 19), (success, error) =>
                {
                    if (success)
                    {
                        Debug.Log("Player data saved successfully.");
                    }
                    else
                    {
                        Debug.LogError($"Failed to save player data: {error}");
                    }
                });
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            YandexGamesSDK.Instance.CloudStorage.FlushData((isf, data) =>
            {
                if (isf)
                {
                    Debug.Log("Flush data saved successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to flush data: {data}");
                }
            });
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            YandexGamesSDK.Instance.SetGameplayStop();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 0; i < 10; i++)
            {
                YandexGamesSDK.Instance.CloudStorage.Load($"primit_{i}", (success, data, error) =>
                {
                    if (success)
                    {
                        Debug.Log($"Loaded player data: int={data}");
                    }
                    else
                    {
                        Debug.LogError($"Failed to load player data: {error}");
                    }
                });
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            YandexGamesSDK.Instance.Leaderboard.GetLeaderboardEntries("testLeaderboard", 0, 10,
                false,
                (list, s) =>
                {
                    Debug.Log($"Get leaderboard entries: {JsonUtility.ToJson(list)} for {list?.Leaderboard?.Name}");
                });
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            YandexGamesSDK.Instance.Advertisement.ShowRewardedAd((success, adResponse, error) =>
            {
                if (success)
                {
                    switch (adResponse)
                    {
                        case YGAdResponse.AdOpened:
                            Debug.Log("Rewarded ad opened.");
                            break;
                        case YGAdResponse.AdGranted:
                            Debug.Log("Rewarded ad granted.");
                            // Grant reward to the player
                            break;
                        case YGAdResponse.AdClosed:
                            Debug.Log("Rewarded ad closed.");
                            break;
                        default:
                            Debug.Log($"Unknown ad response: {adResponse}");
                            break;
                    }
                }
                else
                {
                    Debug.LogError($"Failed to show rewarded ad: {error}");
                }
            });

        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            YandexGamesSDK.Instance.Advertisement.ShowInterstitialAd((success, adResponse, error) =>
            {
                if (success)
                {
                    switch (adResponse)
                    {
                        case YGAdResponse.AdOpened:
                            Debug.Log("Interstitial ad opened.");
                            break;
                        case YGAdResponse.AdClosed:
                            Debug.Log("Interstitial ad closed.");
                            break;
                        default:
                            Debug.Log($"Unknown ad response: {adResponse}");
                            break;
                    }
                }
                else
                {
                    Debug.LogError($"Failed to show interstitial ad: {error}");
                }
            });

        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            YandexGamesSDK.Instance.Advertisement.ShowBannerAd("bottom", (success, adResponse, error) =>
            {
                if (success)
                {
                    if (adResponse == YGAdResponse.AdShown)
                    {
                        Debug.Log("Banner ad shown.");
                    }
                    else
                    {
                        Debug.Log($"Unknown banner ad response: {adResponse}");
                    }
                }
                else
                {
                    Debug.LogError($"Failed to show banner ad: {error}");
                }
            });

        }
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            YandexGamesSDK.Instance.Leaderboard.SubmitScore("testLeaderboard", 100, callback: (success, error) =>
            {
                Debug.Log($"Submit score: {success}");
                Debug.Log($"error: {error}");
            });
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            YandexGamesSDK.Instance.Leaderboard.GetPlayerEntry("testLeaderboard",
                (leaderBoard, error) => { Debug.Log(JsonUtility.ToJson(leaderBoard)); });
        }
    }
}
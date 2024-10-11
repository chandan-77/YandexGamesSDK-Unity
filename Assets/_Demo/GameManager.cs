using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Advertisement;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public struct PlayerData
    {
        public string playerName;
        public string level;
    }

    void Start()
    {
        YandexGamesSDK.Instance.Authentication.AuthenticateUser(true, (isAuh, error) =>
        {
            Debug.Log("IsAuth: " + isAuh);

            if (isAuh)
            {
                Debug.Log(YandexGamesSDK.Instance.Authentication.GetUserProfile().name);
            }
        });

        YandexGamesSDK.Instance.GetServerTime((isFetched, time) => { Debug.Log("GetServerTime: " + time); });

        YandexGamesSDK.Instance.GetEnvironment((isFetched, env) => { Debug.Log("GetEnvironment: " + env.i18n.lang); });
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
                false, (list, s) => { Debug.Log($"Get leaderboard entries: {JsonUtility.ToJson(list)} for {list?.Leaderboard?.Name}"); });
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            YandexGamesSDK.Instance.Advertisement.ShowRewardedAd((success, adType, error) =>
            {
                if (success)
                {
                    switch (Enum.Parse(typeof(RewardedAdResponse), adType))
                    {
                        case RewardedAdResponse.AdGranted:
                            Debug.Log("Rewarded ad collect.");
                            break;
                        case RewardedAdResponse.AdClosed:
                            Debug.Log("Rewarded ad closed.");
                            break;
                        case RewardedAdResponse.AdOpened:
                            Debug.Log("Rewarded ad opened.");
                            break;
                        default:
                            Debug.Log($"Rewarded ad response type is invalid: {adType}");
                            break;
                    }
                }
            });

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
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Plugins.YandexGamesSDK.Runtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public struct PlayerData
    {
        public string playerName;
        public string level;
    }
    void Start()
    {
        YandexGamesSDK.Instance.Authentication.AuthenticateUser((isAuh, error) =>
        {
            Debug.Log("IsAuth: " + isAuh);

            if (isAuh)
            {
                Debug.Log(YandexGamesSDK.Instance.Authentication.GetUserProfile().name);
            }
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            YandexGamesSDK.Instance.CloudStorage.SaveData("testData", new PlayerData()
            {
                level = "1",
                playerName = "SavedPlayer"
            }, delegate { Debug.Log("Player saved."); });
        }
        
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            YandexGamesSDK.Instance.CloudStorage.LoadData("testData",
                (a, b) => { Debug.Log($"Loading data: {a}, {b}"); });
        }
    }
}
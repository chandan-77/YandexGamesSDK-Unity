using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Plugins.YandexGamesSDK.Runtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     YandexGamesSDK.Instance.CloudStorage.SaveData("testData", new
        //     {
        //         platform = "Android",
        //         platform_version = "1.0.0",
        //         player_language = "en_US",
        //         player_sound_vol = "1",
        //     }, delegate { Debug.Log("Player saved."); });
        // }
        //
        //
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     YandexGamesSDK.Instance.CloudStorage.LoadData("testData",
        //         (a, b) => { Debug.Log($"Loading data: {a}, {b}"); });
        // }
    }
}
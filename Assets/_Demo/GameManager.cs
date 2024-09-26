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
           
       });
       
       YandexGamesSDK.Instance.Advertisement.ShowRewardedAd();
       
    }

    [DllImport("__Internal")]
    private static extern int add(int a, int b);

    [DllImport("__Internal")]
    private static extern int subtract(int a, int b);

    void Staart()
    {
        // Call the JS functions from Unity
        int resultAdd = add(5, 3);
        int resultSubtract = subtract(10, 4);

        Debug.Log($"Add: {resultAdd}");
        Debug.Log($"Subtract: {resultSubtract}");
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
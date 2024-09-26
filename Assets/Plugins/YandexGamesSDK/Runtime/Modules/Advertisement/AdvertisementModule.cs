using System;
using System.Runtime.InteropServices;
using Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using UnityEngine;

namespace Plugins.YandexGamesSDK.Runtime.Modules.Advertisement
{
    public class AdvertisementModule : YGModuleBase, IAdvertisementModule
    {
        private Action<bool, string> interstitialCallback;
        private Action<bool, string> rewardedCallback;
        private Action<bool, string> bannerCallback;

        public override void Initialize()
        {
        }

        // Interstitial Ad
        [DllImport("__Internal")]
        private static extern void ShowInterstitialAd(string gameObjectName);

        // Rewarded Ad
        [DllImport("__Internal")]
        private static extern void ShowRewardedAd(string gameObjectName);

        // Banner Ad
        [DllImport("__Internal")]
        private static extern void ShowBannerAd(string gameObjectName, string position);

        [DllImport("__Internal")]
        private static extern void HideBannerAd(string gameObjectName);


        public void ShowInterstitialAd(Action<bool, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            interstitialCallback = callback;
            ShowInterstitialAd(gameObject.name);
#else
            Debug.Log("Interstitial ads are only available in WebGL builds.");
            callback?.Invoke(false, "Interstitial ads are only available in WebGL builds.");
#endif
        }


        public void ShowRewardedAd(Action<bool, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            rewardedCallback = callback;
            ShowRewardedAd(gameObject.name);
#else
            Debug.Log("Rewarded ads are only available in WebGL builds.");
            callback?.Invoke(false, "Rewarded ads are only available in WebGL builds.");
#endif
        }

        public void ShowBannerAd(string position, Action<bool, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            bannerCallback = callback;
            ShowBannerAd(gameObject.name, position);
#else
            Debug.Log("Banner ads are only available in WebGL builds.");
            callback?.Invoke(false, "Banner ads are only available in WebGL builds.");
#endif
        }

        public void HideBannerAd(Action<bool, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            bannerCallback = callback;
            HideBannerAd(gameObject.name);
#else
            Debug.Log("Banner ads are only available in WebGL builds.");
            callback?.Invoke(false, "Banner ads are only available in WebGL builds.");
#endif
        }

        // Callbacks from JavaScript
        private void OnInterstitialAdClosed(string message)
        {
            bool success = message.StartsWith("true");
            string error = success ? null : message.Split('|')[1];
            interstitialCallback?.Invoke(success, error);
        }

        private void OnRewardedAdCompleted(string message)
        {
            bool success = message.StartsWith("true");
            string error = success ? null : message.Split('|')[1];
            rewardedCallback?.Invoke(success, error);
        }

        private void OnBannerAdLoaded(string message)
        {
            bool success = message.StartsWith("true");
            string error = success ? null : message.Split('|')[1];
            bannerCallback?.Invoke(success, error);
        }

        private void OnBannerAdHidden(string message)
        {
            bool success = message.StartsWith("true");
            string error = success ? null : message.Split('|')[1];
            bannerCallback?.Invoke(success, error);
        }
    }
}
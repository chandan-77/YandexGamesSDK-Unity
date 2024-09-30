using System;
using System.Runtime.InteropServices;
using Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using UnityEngine;

namespace Plugins.YandexGamesSDK.Runtime.Modules.Advertisement
{
    public class AdvertisementModule : YGModuleBase, IAdvertisementModule
    {
        private Action<bool, string> bannerCallback;
        
        private Action<bool, string> rewardedCallback;
        private Action rewardedOnOpenCallback;
        private Action rewardedOnRewardCallback;
        private Action rewardedOnCloseCallback;
        
        private Action<bool, string> interstitialCallback;
        private Action interstitialOnOpenCallback;
        private Action interstitialOnOfflineCallback;
        private Action interstitialOnCloseCallback;


        public override void Initialize()
        {
        }

        // Interstitial Ad
        [DllImport("__Internal")]
        private static extern void ShowInterstitialAd();

        // Rewarded Ad
        [DllImport("__Internal")]
        private static extern void ShowRewardedAd();

        // Banner Ad
        [DllImport("__Internal")]
        private static extern void ShowBannerAd(string position);

        [DllImport("__Internal")]
        private static extern void HideBannerAd();


        public void ShowInterstitialAd(Action<bool, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            interstitialCallback = callback;
            ShowInterstitialAd();
#else
            Debug.Log("Interstitial ads are only available in WebGL builds.");
            callback?.Invoke(false, "Interstitial ads are only available in WebGL builds.");
#endif
        }


        public void ShowRewardedAd(Action<bool, string> callback = null,
            Action onOpen = null, Action onReward = null, Action onClose = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
 rewardedCallback = callback;
            rewardedOnRewardCallback = onReward;
            rewardedOnOpenCallback = onOpen;
            rewardedOnCloseCallback = onClose;
            rewardedCallback = callback;
            ShowRewardedAd();
#else
            Debug.Log("Rewarded ads are only available in WebGL builds.");
            callback?.Invoke(false, "Rewarded ads are only available in WebGL builds.");
#endif
        }

        public void ShowBannerAd(string position, Action<bool, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            bannerCallback = callback;
            ShowBannerAd(position);
#else
            Debug.Log("Banner ads are only available in WebGL builds.");
            callback?.Invoke(false, "Banner ads are only available in WebGL builds.");
#endif
        }

        public void HideBannerAd(Action<bool, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            bannerCallback = callback;
            HideBannerAd();
#else
            Debug.Log("Banner ads are only available in WebGL builds.");
            callback?.Invoke(false, "Banner ads are only available in WebGL builds.");
#endif
        }

        // Callbacks from JavaScript
        private void OnInterstitialAdCompleted(string message)
        {
            bool success = message.StartsWith("true");
            string error = success ? null : message.Split('|')[1];
            interstitialCallback?.Invoke(success, error);
        }

        private void OnInterstitialAdOpen(string message)
        {
            interstitialOnOpenCallback?.Invoke();
        }
        private void OnInterstitialAdClose(string message)
        {
            interstitialOnCloseCallback?.Invoke();
        }
        
        private void OnInterstitialAdOffline(string message)
        {
            interstitialOnOfflineCallback?.Invoke();
        }

        private void OnRewardedAdCompleted(string message)
        {
            bool success = message.StartsWith("true");
            string error = success ? null : message.Split('|')[1];
            rewardedCallback?.Invoke(success, error);
        }

        private void OnRewardedAdOpen(string message)
        {
            rewardedOnOpenCallback?.Invoke();
        }
        private void OnRewardedAdClose(string message)
        {
            rewardedOnCloseCallback?.Invoke();
        }
        
        private void OnRewardedAdReward(string message)
        {
            rewardedOnRewardCallback?.Invoke();
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
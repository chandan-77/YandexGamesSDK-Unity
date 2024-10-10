using System;
using System.Runtime.InteropServices;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Networking;
using UnityEngine;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Advertisement
{
    public enum RewardedAdResponse
    {
        AdOpened,
        AdGranted,
        AdClosed
    }

    public class AdvertisementModule : YGModuleBase, IAdvertisementModule
    {
        public override void Initialize()
        {
        }

        [DllImport("__Internal")]
        private static extern void ShowInterstitialAd(string requestId);

        [DllImport("__Internal")]
        private static extern void ShowRewardedAd(string requestId);

        [DllImport("__Internal")]
        private static extern void ShowBannerAd(string requestId, string position);

        [DllImport("__Internal")]
        private static extern void HideBannerAd(string requestId);

        public void ShowInterstitialAd(Action<bool, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            string requestId = YGRequestManager.GenerateRequestId();
            YGRequestManager.RegisterCallback<string>(requestId, (success, data, error) =>
            {
                callback?.Invoke(success, error);
            });

            ShowInterstitialAd(requestId);
#else
            Debug.Log("Interstitial ads are only available in WebGL builds.");
            callback?.Invoke(false, "Interstitial ads are only available in WebGL builds.");
#endif
        }

        public void ShowRewardedAd(Action<bool, string, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            string requestId = YGRequestManager.GenerateRequestId();
            YGRequestManager.RegisterCallback<string>(requestId, (success, data, error) =>
            {
                callback?.Invoke(success, data, error);
            });

            ShowRewardedAd(requestId);
#else
            Debug.Log("Rewarded ads are only available in WebGL builds.");
            callback?.Invoke(false, null,"Rewarded ads are only available in WebGL builds.");
#endif
        }

        public void ShowBannerAd(string position, Action<bool, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            string requestId = YGRequestManager.GenerateRequestId();
            YGRequestManager.RegisterCallback<string>(requestId, (success, data, error) =>
            {
                callback?.Invoke(success, error);
            });

            ShowBannerAd(requestId, position);
#else
            Debug.Log("Banner ads are only available in WebGL builds.");
            callback?.Invoke(false, "Banner ads are only available in WebGL builds.");
#endif
        }

        public void HideBannerAd(Action<bool, string> callback = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            string requestId = YGRequestManager.GenerateRequestId();
            YGRequestManager.RegisterCallback<string>(requestId, (success, data, error) =>
            {
                callback?.Invoke(success, error);
            });

            HideBannerAd(requestId);
#else
            Debug.Log("Banner ads are only available in WebGL builds.");
            callback?.Invoke(false, "Banner ads are only available in WebGL builds.");
#endif
        }
    }
}
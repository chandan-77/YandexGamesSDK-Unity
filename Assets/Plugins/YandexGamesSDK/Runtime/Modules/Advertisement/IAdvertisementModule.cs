using System;

namespace Plugins.YandexGamesSDK.Runtime.Modules.Advertisement
{
    public interface IAdvertisementModule
    {
        void ShowInterstitialAd(Action<bool, string> callback = null);
        void ShowRewardedAd(Action<bool, string> callback = null);
        void ShowBannerAd(string position, Action<bool, string> callback = null);
        void HideBannerAd(Action<bool, string> callback = null);
    }
}
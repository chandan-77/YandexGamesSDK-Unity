import { YandexGames } from "YandexGamesSDK";

export class AdvertisementModule {
  /**
   * Show an interstitial ad.
   */
  static async showInterstitialAd(): Promise<void> {
    try {
      const sdk: YandexGames.SDK = await window.yandexSDK;

      await sdk.adv.showFullscreenAdv({
        callbacks: {
          onClose: (wasShown: boolean) => {
            if (wasShown) {
              unityInstance.SendMessage('YandexGamesSDK', 'OnInterstitialAdCompleted', 'true|Interstitial ad was shown successfully.');
            } else {
              unityInstance.SendMessage('YandexGamesSDK', 'OnInterstitialAdClose', 'Interstitial ad was not shown due to frequency limitation.');
            }
          },
          onOpen: () => {
            console.log('Interstitial ad opened.');

            unityInstance.SendMessage('YandexGamesSDK', 'OnRewardedAdOpen', `Interstitial ad opened.`);
          },
          onError: (error: any) => {
            console.error('Error while showing interstitial ad:', error);
            unityInstance.SendMessage('YandexGamesSDK', 'OnInterstitialAdCompleted', `false|${error.message || 'Failed to show interstitial ad.'}`);

          },
          onOffline: () => {
            console.warn('Network connection lost, unable to show interstitial ad.');
            unityInstance.SendMessage('YandexGamesSDK', 'OnInterstitialAdOffline', 'Network connection lost.');
          }
        }
      });
    } catch (error: any) {
      console.error('Failed to call interstitial ad:', error);
      unityInstance.SendMessage('YandexGamesSDK', 'OnInterstitialAdFailure', error.message || 'Failed to call interstitial ad.');
    }
  }

  /**
   * Show a rewarded ad.
   */
  static async showRewardedAd(): Promise<void> {
    try {
      const sdk: YandexGames.SDK = await window.yandexSDK;

      await sdk.adv.showRewardedVideo({
        callbacks: {
          onOpen: () => {
            console.log('Rewarded video ad opened.');
            unityInstance.SendMessage('YandexGamesSDK', 'OnRewardedAdOpen', 'Rewarded video ad opened.');
          },
          onRewarded: () => {
            console.log('Reward granted.');
            unityInstance.SendMessage('YandexGamesSDK', 'OnRewardedAdCompleted', `true|Reward granted.`);
            unityInstance.SendMessage('YandexGamesSDK', 'OnRewardedAdReward', 'Rewarded video ad was viewed successfully.');
          },
          onClose: () => {
            console.log('Rewarded video ad closed.');
            unityInstance.SendMessage('YandexGamesSDK', 'OnRewardedAdClose', 'Rewarded video ad closed.');
          },
          onError: (error: any) => {
            console.error('Error while showing rewarded ad:', error);
            unityInstance.SendMessage('YandexGamesSDK', 'OnRewardedAdCompleted', `false|${error.message || 'Failed to show rewarded video ad.'}`);
          }
        }
      });
    } catch (error: any) {
      console.error('Failed to call rewarded ad:', error);
      unityInstance.SendMessage('YandexGamesSDK', 'OnRewardedAdCompleted', `false|${error.message || 'Failed to show rewarded video ad.'}`);
    }
  }

  /**
   * Show a sticky banner ad.
   * @param position The position where the banner ad should be displayed (e.g., "top", "bottom", "right").
   */
  static async showBannerAd(position: string): Promise<void> {
    try {

      const sdk: YandexGames.SDK = await window.yandexSDK;
      // Show a sticky banner ad using the Yandex SDK
      await sdk.adv.showBannerAdv();
      unityInstance.SendMessage('YandexGamesSDK', 'OnBannerAdLoaded', `true|${position}`);
    } catch (error: any) {
      console.error('Failed to show banner ad:', error);
      unityInstance.SendMessage('YandexGamesSDK', 'OnBannerAdLoaded', `false|${error.message || 'Failed to show banner ad.'}`);
    }
  }

  /**
   * Hide the sticky banner ad.
   */
  static async hideBannerAd(): Promise<void> {
    try {
      const sdk: YandexGames.SDK = await window.yandexSDK;

      // Hide the sticky banner ad using the Yandex SDK
      await sdk.adv.hideBannerAdv();
      unityInstance.SendMessage('YandexGamesSDK', 'OnBannerAdHidden', 'true');
    } catch (error: any) {
      console.error('Failed to hide banner ad:', error);
      unityInstance.SendMessage('YandexGamesSDK', 'OnBannerAdHidden', `false|${error.message}`);
    }
  }

  /**
   * Get the current banner ad status.
   */
  static async getBannerAdStatus(): Promise<void> {
    try {
      const sdk: YandexGames.SDK = await window.yandexSDK;

      const { stickyAdvIsShowing, reason } = await sdk.adv.getBannerAdvStatus();

      if (stickyAdvIsShowing) {
        console.log('Sticky banner is currently being shown.');
        unityInstance.SendMessage('YandexGamesSDK', 'OnBannerAdStatus', 'Sticky banner is currently being shown.');
      } else if (reason) {
        console.log(`Sticky banner is not shown. Reason: ${reason}`);
        unityInstance.SendMessage('YandexGamesSDK', 'OnBannerAdFailure', `Sticky banner is not shown. Reason: ${reason}`);
      } else {
        unityInstance.SendMessage('YandexGamesSDK', 'OnBannerAdHidden', 'Sticky banner is not currently being shown.');
      }
    } catch (error: any) {
      console.error('Failed to get banner ad status:', error);
      unityInstance.SendMessage('YandexGamesSDK', 'OnBannerAdFailure', error.message || 'Failed to get banner ad status.');
    }
  }
}

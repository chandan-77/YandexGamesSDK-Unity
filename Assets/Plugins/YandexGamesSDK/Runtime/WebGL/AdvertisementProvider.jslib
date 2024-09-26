mergeInto(LibraryManager.library, {
  ShowInterstitialAd: function (gameObjectNamePtr) {
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    if (typeof ysdk !== 'undefined') {
      ysdk.adv.showFullscreenAdv({
        callbacks: {
          onClose: function () {
            SendMessage(gameObjectName, 'OnInterstitialAdClosed', 'true');
          },
          onError: function (error) {
            SendMessage(gameObjectName, 'OnInterstitialAdClosed', 'false|' + error);
          }
        }
      });
    } else {
      SendMessage(gameObjectName, 'OnInterstitialAdClosed', 'false|YSDK not initialized');
    }
  },

  ShowRewardedAd: function (gameObjectNamePtr) {
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    if (typeof ysdk !== 'undefined') {
      ysdk.adv.showRewardedVideo({
        callbacks: {
          onOpen: function () {
            SendMessage(gameObjectName, 'OnRewardedAdOpened', '');
          },
          onRewarded: function () {
            SendMessage(gameObjectName, 'OnRewardedAdRewarded', '');
          },
          onClose: function (wasShown) {
            SendMessage(gameObjectName, 'OnRewardedAdClosed', wasShown ? 'true' : 'false');
          },
          onError: function (error) {
            SendMessage(gameObjectName, 'OnRewardedAdError', error);
          }
        }
      });
    } else {
      SendMessage(gameObjectName, 'OnRewardedAdError', 'YSDK not initialized');
    }
  },

  ShowBannerAd: function (gameObjectNamePtr, positionPtr) {
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    var position = UTF8ToString(positionPtr); // 'top' or 'bottom'

    if (typeof ysdk !== 'undefined') {
      ysdk.adv.showBannerAdv({
        callbacks: {
          onLoad: function () {
            SendMessage(gameObjectName, 'OnBannerAdLoaded', 'true');
          },
          onError: function (error) {
            SendMessage(gameObjectName, 'OnBannerAdLoaded', 'false|' + error);
          }
        },
        params: {
          position: position
        }
      });
    } else {
      SendMessage(gameObjectName, 'OnBannerAdLoaded', 'false|YSDK not initialized');
    }
  },

  HideBannerAd: function (gameObjectNamePtr) {
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    if (typeof ysdk !== 'undefined') {
      ysdk.adv.hideBannerAdv();
      SendMessage(gameObjectName, 'OnBannerAdHidden', 'true');
    } else {
      SendMessage(gameObjectName, 'OnBannerAdHidden', 'false|YSDK not initialized');
    }
  }
});

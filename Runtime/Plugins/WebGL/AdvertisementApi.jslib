const advertisementApiLibrary = {
  
  $advertisementApi: {
    isInitialized: false,
    sdk: undefined,

    throwIfSdkNotInitialized: function() {
      if (!advertisementApi.sdk && typeof yandexGamesPlugin !== 'undefined') {
        advertisementApi.sdk = yandexGamesPlugin.sdk;
        advertisementApi.isInitialized = yandexGamesPlugin.isInitialized;
      }
      
      if (!advertisementApi.sdk) {
        throw new Error('SDK is not initialized. Make sure YandexGamesSDK is initialized first.');
      }
    },

    // Interstitial Ad Methods
    showInterstitialAd: function(successCallbackPtr, errorCallbackPtr) {
      try {
        advertisementApi.throwIfSdkNotInitialized();
        
        advertisementApi.sdk.adv.showFullscreenAdv({
          callbacks: {
            onOpen: function() {
              yandexGamesPlugin.sendResponse(
                successCallbackPtr,
                errorCallbackPtr,
                { response: "AdOpened" },
                null
              );
            },
            onClose: function(wasShown) {
              yandexGamesPlugin.sendResponse(
                successCallbackPtr,
                errorCallbackPtr,
                { response: "AdClosed", wasShown: wasShown },
                null
              );
            },
            onError: function(error) {
              yandexGamesPlugin.sendResponse(
                successCallbackPtr,
                errorCallbackPtr,
                null,
                error instanceof Error ? error.message : error
              );
            },
            onOffline: function() {
              yandexGamesPlugin.sendResponse(
                successCallbackPtr,
                errorCallbackPtr,
                null,
                "Device is offline"
              );
            }
          }
        });
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error instanceof Error ? error.message : "Unknown error showing interstitial ad"
        );
      }
    },

    // Rewarded Ad Methods
    showRewardedAd: function(successCallbackPtr, errorCallbackPtr) {
      try {
        advertisementApi.throwIfSdkNotInitialized();
        
        advertisementApi.sdk.adv.showRewardedVideo({
          callbacks: {
            onOpen: function() {
              yandexGamesPlugin.sendResponse(
                successCallbackPtr,
                errorCallbackPtr,
                { response: "AdOpened" },
                null
              );
            },
            onRewarded: function() {
              yandexGamesPlugin.sendResponse(
                successCallbackPtr,
                errorCallbackPtr,
                { response: "AdRewarded" },
                null
              );
            },
            onClose: function() {
              yandexGamesPlugin.sendResponse(
                successCallbackPtr,
                errorCallbackPtr,
                { response: "AdClosed" },
                null
              );
            },
            onError: function(error) {
              yandexGamesPlugin.sendResponse(
                successCallbackPtr,
                errorCallbackPtr,
                null,
                error instanceof Error ? error.message : error
              );
            }
          }
        });
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error instanceof Error ? error.message : "Unknown error showing rewarded ad"
        );
      }
    },

    // Sticky (Banner) Ad Methods
    showBannerAd: function(position, successCallbackPtr, errorCallbackPtr) {
      try {
        advertisementApi.throwIfSdkNotInitialized();
        
        advertisementApi.sdk.adv.showBannerAdv({
          position: position || 'bottom'
        });
        
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          { response: "BannerShown" },
          null
        );
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error instanceof Error ? error.message : "Unknown error showing banner ad"
        );
      }
    },

    hideBannerAd: function(successCallbackPtr, errorCallbackPtr) {
      try {
        advertisementApi.throwIfSdkNotInitialized();
        
        advertisementApi.sdk.adv.hideBannerAdv();
        
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          { response: "BannerHidden" },
          null
        );
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error instanceof Error ? error.message : "Unknown error hiding banner ad"
        );
      }
    }
  },

  // External C# calls
  AdvertisementApi_ShowInterstitialAd: function(successCallbackPtr, errorCallbackPtr) {
    advertisementApi.showInterstitialAd(successCallbackPtr, errorCallbackPtr);
  },

  AdvertisementApi_ShowRewardedAd: function(successCallbackPtr, errorCallbackPtr) {
    advertisementApi.showRewardedAd(successCallbackPtr, errorCallbackPtr);
  },

  AdvertisementApi_ShowBannerAd: function(positionPtr, successCallbackPtr, errorCallbackPtr) {
    const position = UTF8ToString(positionPtr);
    advertisementApi.showBannerAd(position, successCallbackPtr, errorCallbackPtr);
  },

  AdvertisementApi_HideBannerAd: function(successCallbackPtr, errorCallbackPtr) {
    advertisementApi.hideBannerAd(successCallbackPtr, errorCallbackPtr);
  }
};

autoAddDeps(advertisementApiLibrary, '$advertisementApi');
mergeInto(LibraryManager.library, advertisementApiLibrary); 
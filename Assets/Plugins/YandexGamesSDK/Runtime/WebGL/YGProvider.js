mergeInto(LibraryManager.library, {
    initializeGame: function () {
        InitializeGame(); 
    },

    showInterstitialAd: function () {
        ShowInterstitialAd(); 
    },

    showRewardedAd: function (id) {
        ShowRewardedAd(id);
    },
    
    showReviewPrompt: function () {
        ShowReviewPrompt();
    },
    
    showGenericPrompt: function () {
        ShowGenericPrompt();
    },
    
    toggleStickyAdActivity: function (activity) {
        ToggleStickyAdActivity(activity);
    },
    
    openURLInNewTab: function (url) {
        window.open(UTF8ToString(url), "_blank");
    },
    
    startGameplay: function () {
        if (ysdk && ysdk.features?.GameplayAPI) {
            ysdk.features.GameplayAPI.start();
        } else {
            console.error('Gameplay start failed. The SDK is not initialized or GameplayAPI is undefined.');
        }
    },
    
    stopGameplay: function () {
        if (ysdk && ysdk.features?.GameplayAPI) {
            ysdk.features.GameplayAPI.stop();
        } else {
            console.error('Gameplay stop failed. The SDK is not initialized or GameplayAPI is undefined.');
        }
    },
    
    getServerTime: function () {
        if (ysdk) {
            var serverTime = ysdk.serverTime().toString();
            var lengthBytes = lengthBytesUTF8(serverTime) + 1;
            var buffer = _malloc(lengthBytes);
            stringToUTF8(serverTime, buffer, lengthBytes);
            return buffer;
        }
        return 0;
    },
    
    setFullscreenMode: function (fullscreen) {
        if (ysdk && ysdk.screen?.fullscreen) {
            if (fullscreen) {
                if (ysdk.screen.fullscreen.status !== 'on') {
                    ysdk.screen.fullscreen.request();
                }
            } else {
                if (ysdk.screen.fullscreen.status !== 'off') {
                    ysdk.screen.fullscreen.exit();
                }
            }
        }
    },
    
    isFullscreenMode: function () {
        if (ysdk && ysdk.screen?.fullscreen) {
            return ysdk.screen.fullscreen.status === 'on';
        }
        return false;
    }
});

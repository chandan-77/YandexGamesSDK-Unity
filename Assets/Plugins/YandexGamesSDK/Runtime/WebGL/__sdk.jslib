mergeInto(LibraryManager.library, {

        AuthenticateUser: function() {
          if (typeof window.YandexSDKExports.AuthenticateUser === 'function') {
            
            window.YandexSDKExports.AuthenticateUser();
          } else {
            console.error('AuthenticateUser is not defined on window.YandexSDKExports.');
          }
        },

        SavePlayerData: function(keyPtr, dataPtr, flushPtr) {
          if (typeof window.YandexSDKExports.SavePlayerData === 'function') {
            var key = UTF8ToString(keyPtr), data = UTF8ToString(dataPtr), flush = UTF8ToString(flushPtr);
            window.YandexSDKExports.SavePlayerData(key, data, flush);
          } else {
            console.error('SavePlayerData is not defined on window.YandexSDKExports.');
          }
        },

        LoadPlayerData: function(keyPtr) {
          if (typeof window.YandexSDKExports.LoadPlayerData === 'function') {
            var key = UTF8ToString(keyPtr);
            window.YandexSDKExports.LoadPlayerData(key);
          } else {
            console.error('LoadPlayerData is not defined on window.YandexSDKExports.');
          }
        },

        CheckForInitialization: function() {
          if (typeof window.YandexSDKExports.CheckForInitialization === 'function') {
            
            window.YandexSDKExports.CheckForInitialization();
          } else {
            console.error('CheckForInitialization is not defined on window.YandexSDKExports.');
          }
        },

        OnYandexGamesSDKReady: function() {
          if (typeof window.YandexSDKExports.OnYandexGamesSDKReady === 'function') {
            
            window.YandexSDKExports.OnYandexGamesSDKReady();
          } else {
            console.error('OnYandexGamesSDKReady is not defined on window.YandexSDKExports.');
          }
        },
});
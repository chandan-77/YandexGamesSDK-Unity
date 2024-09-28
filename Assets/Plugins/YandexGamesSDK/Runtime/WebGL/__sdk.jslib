mergeInto(LibraryManager.library, {

        AuthenticateUser: function() {
          if (typeof window.YandexSDKExports.AuthenticateUser === 'function') {
            
            window.YandexSDKExports.AuthenticateUser();
          } else {
            console.error('AuthenticateUser is not defined on window.YandexSDKExports.');
          }
        },

        SavePlayerData: function(keyPtr, dataPtr) {
          if (typeof window.YandexSDKExports.SavePlayerData === 'function') {
            var key = UTF8ToString(keyPtr), data = UTF8ToString(dataPtr);
            window.YandexSDKExports.SavePlayerData(key, data);
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
});
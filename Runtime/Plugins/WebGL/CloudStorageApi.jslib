const cloudStorageApiLibrary = {
  
  $cloudStorageApi: {
    isInitialized: false,
    sdk: undefined,
    playerAccount: undefined,

    throwIfSdkNotInitialized: function() {
      if (!cloudStorageApi.sdk && typeof yandexGamesPlugin !== 'undefined') {
        cloudStorageApi.sdk = yandexGamesPlugin.sdk;
        cloudStorageApi.isInitialized = yandexGamesPlugin.isInitialized;
        cloudStorageApi.playerAccount = typeof authenticationApi !== 'undefined' ? 
                                         authenticationApi.playerAccount : undefined;
      }
      
      if (!cloudStorageApi.sdk) {
        throw new Error('SDK is not initialized. Make sure YandexGamesSDK is initialized first.');
      }
      
      if (!cloudStorageApi.playerAccount) {
        return cloudStorageApi.sdk.getPlayer({ scopes: false }).then(function(player) {
          cloudStorageApi.playerAccount = player;
          return Promise.resolve();
        });
      }
      
      return Promise.resolve();
    },

    savePlayerData: function(key, dataJson, successCallbackPtr, errorCallbackPtr) {
      try {
        cloudStorageApi.throwIfSdkNotInitialized().then(function() {
          if (!cloudStorageApi.playerAccount) {
            yandexGamesPlugin.sendResponse(
              successCallbackPtr,
              errorCallbackPtr,
              null,
              "Player is not authorized. Authentication required for cloud storage."
            );
            return;
          }
          
          let data;
          try {
            data = JSON.parse(dataJson);
          } catch (e) {
            yandexGamesPlugin.sendResponse(
              successCallbackPtr,
              errorCallbackPtr,
              null,
              "Failed to parse data JSON: " + e.message
            );
            return;
          }
          
          const saveData = {};
          saveData[key] = data;
          
          cloudStorageApi.playerAccount.setData(saveData, true).then(function() {
            yandexGamesPlugin.sendResponse(
              successCallbackPtr,
              errorCallbackPtr,
              "true",
              null
            );
          }).catch(function(error) {
            yandexGamesPlugin.sendResponse(
              successCallbackPtr,
              errorCallbackPtr,
              null,
              error instanceof Error ? error.message : "Failed to save player data"
            );
          });
        }).catch(function(error) {
          yandexGamesPlugin.sendResponse(
            successCallbackPtr,
            errorCallbackPtr,
            null,
            error instanceof Error ? error.message : "Failed to initialize player account"
          );
        });
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error instanceof Error ? error.message : "Unknown error saving player data"
        );
      }
    },
    
    loadPlayerData: function(key, successCallbackPtr, errorCallbackPtr) {
      try {
        cloudStorageApi.throwIfSdkNotInitialized().then(function() {
          if (!cloudStorageApi.playerAccount) {
            yandexGamesPlugin.sendResponse(
              successCallbackPtr,
              errorCallbackPtr,
              null,
              "Player is not authorized. Authentication required for cloud storage."
            );
            return;
          }
          
          cloudStorageApi.playerAccount.getData().then(function(data) {
            if (data && data[key]) {
              yandexGamesPlugin.sendResponse(
                successCallbackPtr,
                errorCallbackPtr,
                JSON.stringify(data[key]),
                null
              );
            } else {
              yandexGamesPlugin.sendResponse(
                successCallbackPtr,
                errorCallbackPtr,
                "{}",
                null
              );
            }
          }).catch(function(error) {
            yandexGamesPlugin.sendResponse(
              successCallbackPtr,
              errorCallbackPtr,
              null,
              error instanceof Error ? error.message : "Failed to load player data"
            );
          });
        }).catch(function(error) {
          yandexGamesPlugin.sendResponse(
            successCallbackPtr,
            errorCallbackPtr,
            null,
            error instanceof Error ? error.message : "Failed to initialize player account"
          );
        });
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error instanceof Error ? error.message : "Unknown error loading player data"
        );
      }
    }
  },

  // External C# calls
  CloudStorageApi_SavePlayerData: function(keyPtr, dataJsonPtr, successCallbackPtr, errorCallbackPtr) {
    const key = UTF8ToString(keyPtr);
    const dataJson = UTF8ToString(dataJsonPtr);
    cloudStorageApi.savePlayerData(key, dataJson, successCallbackPtr, errorCallbackPtr);
  },
  
  CloudStorageApi_LoadPlayerData: function(keyPtr, successCallbackPtr, errorCallbackPtr) {
    const key = UTF8ToString(keyPtr);
    cloudStorageApi.loadPlayerData(key, successCallbackPtr, errorCallbackPtr);
  }
};

autoAddDeps(cloudStorageApiLibrary, '$cloudStorageApi');
mergeInto(LibraryManager.library, cloudStorageApiLibrary); 
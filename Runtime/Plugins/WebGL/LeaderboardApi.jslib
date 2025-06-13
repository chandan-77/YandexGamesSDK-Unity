const leaderboardApiLibrary = {
  
  $leaderboardApi: {
    isInitialized: false,
    sdk: undefined,
    leaderboard: undefined,

    throwIfSdkNotInitialized: function() {
      if (!leaderboardApi.sdk && typeof yandexGamesPlugin !== 'undefined') {
        leaderboardApi.sdk = yandexGamesPlugin.sdk;
        leaderboardApi.isInitialized = yandexGamesPlugin.isInitialized;
        
        // Initialize leaderboard if SDK is available but leaderboard isn't
        if (leaderboardApi.sdk && !leaderboardApi.leaderboard) {
            leaderboardApi.leaderboard = leaderboardApi.sdk.leaderboards;
        }
      }
      
      if (!leaderboardApi.sdk) {
        throw new Error('SDK is not initialized. Make sure YandexGamesSDK is initialized first.');
      }
      
      if (!leaderboardApi.leaderboard) {
        throw new Error('Leaderboard is not initialized. Make sure SDK is initialized properly.');
      }
    },

    submitScore: function(leaderboardName, score, extraData, successCallbackPtr, errorCallbackPtr) {
      try {
        leaderboardApi.throwIfSdkNotInitialized();
        
        // Set the score on the leaderboard
        leaderboardApi.leaderboard.setLeaderboardScore(leaderboardName, score, extraData)
          .then(function() {
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
              error instanceof Error ? error.message : "Failed to submit score"
            );
          });
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error instanceof Error ? error.message : "Unknown error submitting score"
        );
      }
    },
    
    getLeaderboardEntries: function(leaderboardName, quantityTop, quantityAround, includeUser, 
        successCallbackPtr, errorCallbackPtr) {
      try {
        leaderboardApi.throwIfSdkNotInitialized();
        
        // Get entries from the leaderboard
        leaderboardApi.leaderboard.getLeaderboardEntries(leaderboardName, {
          includeUser: includeUser,
          quantityAround: quantityAround,
          quantityTop: quantityTop
        }).then(function(response) {
          // Process entries to add player profile pictures
          if (response && response.entries) {
            response.entries.forEach(function(entry) {
              if (entry.player) {
                entry.player.profilePicture = {
                  small: entry.player.getAvatarSrc({ size: "small" }) || "",
                  medium: entry.player.getAvatarSrc({ size: "medium" }) || "",
                  large: entry.player.getAvatarSrc({ size: "large" }) || ""
                };
              }
            });
          }
          
          yandexGamesPlugin.sendResponse(
            successCallbackPtr,
            errorCallbackPtr,
            JSON.stringify(response),
            null
          );
        }).catch(function(error) {
          yandexGamesPlugin.sendResponse(
            successCallbackPtr,
            errorCallbackPtr,
            null,
            error instanceof Error ? error.message : "Failed to get leaderboard entries"
          );
        });
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error instanceof Error ? error.message : "Unknown error getting leaderboard entries"
        );
      }
    },
    
    getPlayerEntry: function(leaderboardName, successCallbackPtr, errorCallbackPtr) {
      try {
        leaderboardApi.throwIfSdkNotInitialized();
        
        // Get the player's entry
        leaderboardApi.leaderboard.getLeaderboardPlayerEntry(leaderboardName)
          .then(function(response) {
            if (response && response.player) {
              response.player.profilePicture = {
                small: response.player.getAvatarSrc({ size: "small" }) || "",
                medium: response.player.getAvatarSrc({ size: "medium" }) || "",
                large: response.player.getAvatarSrc({ size: "large" }) || ""
              };
            }
          
            yandexGamesPlugin.sendResponse(
              successCallbackPtr,
              errorCallbackPtr,
              JSON.stringify(response),
              null
            );
          }).catch(function(error) {
            // Special case for player not in leaderboard
            if (error && error.code === 'LEADERBOARD_PLAYER_NOT_PRESENT') {
              yandexGamesPlugin.sendResponse(
                successCallbackPtr,
                errorCallbackPtr,
                "null",
                null
              );
            } else {
              yandexGamesPlugin.sendResponse(
                successCallbackPtr,
                errorCallbackPtr,
                null,
                error instanceof Error ? error.message : "Failed to get player entry"
              );
            }
          });
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error instanceof Error ? error.message : "Unknown error getting player entry"
        );
      }
    }
  },

  // External C# calls
  LeaderboardApi_SubmitScore: function(leaderboardNamePtr, score, extraDataPtr, 
      successCallbackPtr, errorCallbackPtr) {
    const leaderboardName = UTF8ToString(leaderboardNamePtr);
    const extraData = UTF8ToString(extraDataPtr);
    leaderboardApi.submitScore(leaderboardName, score, 
      extraData.length > 0 ? extraData : undefined,
      successCallbackPtr, errorCallbackPtr);
  },
  
  LeaderboardApi_GetLeaderboardEntries: function(leaderboardNamePtr, quantityTop, 
      quantityAround, includeUser, successCallbackPtr, errorCallbackPtr) {
    const leaderboardName = UTF8ToString(leaderboardNamePtr);
    leaderboardApi.getLeaderboardEntries(leaderboardName, quantityTop, 
      quantityAround, !!includeUser, successCallbackPtr, errorCallbackPtr);
  },
  
  LeaderboardApi_GetPlayerEntry: function(leaderboardNamePtr, successCallbackPtr, errorCallbackPtr) {
    const leaderboardName = UTF8ToString(leaderboardNamePtr);
    leaderboardApi.getPlayerEntry(leaderboardName, successCallbackPtr, errorCallbackPtr);
  }
};

autoAddDeps(leaderboardApiLibrary, '$leaderboardApi');
mergeInto(LibraryManager.library, leaderboardApiLibrary); 

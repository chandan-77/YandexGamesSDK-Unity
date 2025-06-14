const yandexGamesPluginLibrary = {
  
  $yandexGamesPlugin: {
    isInitialized: false,
    sdk: undefined,
    isInitializeCalled: false,

    // Helper function for string allocation
    allocateUnmanagedString: function(string) {
      const stringBufferSize = lengthBytesUTF8(string) + 1;
      const stringBufferPtr = _malloc(stringBufferSize);
      stringToUTF8(string, stringBufferPtr, stringBufferSize);
      return stringBufferPtr;
    },

    // Helper function to send response via dynCall
    sendResponse: function(successCallbackPtr, errorCallbackPtr, data, error) {
      const response = {
        status: !error,
        data: data,
        error: error ? (error instanceof Error ? error.message : error) : null
      };

      const jsonPtr = yandexGamesPlugin.allocateUnmanagedString(JSON.stringify(response));
      if (error) {
        {{{ makeDynCall('vi', 'errorCallbackPtr') }}} (jsonPtr);
        // see https://docs.unity3d.com/6000.0/Documentation/Manual/web-interacting-browser-deprecated.html#dyncall
        // dynCall('vi', errorCallbackPtr, [jsonPtr]);
      } else {
        {{{ makeDynCall('vi', 'successCallbackPtr') }}} (jsonPtr);
        // see https://docs.unity3d.com/6000.0/Documentation/Manual/web-interacting-browser-deprecated.html#dyncall
        // dynCall('vi', successCallbackPtr, [jsonPtr]);
      }
      _free(jsonPtr);
    },

    initialize: function(successCallbackPtr, errorCallbackPtr) {
      if (yandexGamesPlugin.isInitializeCalled) {
        return;
      }
      yandexGamesPlugin.isInitializeCalled = true;

      function postInit(sdk, reused) {
        yandexGamesPlugin.sdk = sdk;
        yandexGamesPlugin.isInitialized = true;
        window.ysdk = sdk;

        // Initialize other modules conditionally
        if (typeof advertisementApi !== 'undefined' && !advertisementApi.isInitialized) {
          advertisementApi.sdk = sdk;
          advertisementApi.isInitialized = true;
        }

        if (typeof purchaseApi !== 'undefined' && !purchaseApi.isInitialized) {
          purchaseApi.sdk = sdk;
          purchaseApi.initializePayments().catch(function(error) {
            console.error("Failed to initialize payments:", error);
          });
        }

        if (typeof authenticationApi !== 'undefined' && !authenticationApi.isInitialized) {
          authenticationApi.sdk = sdk;
          authenticationApi.isInitialized = true;

          sdk.getPlayer({ scopes: false }).then(function(player) {
            authenticationApi.playerAccount = player;
            if (player.isAuthorized()) {
              authenticationApi.isAuthorized = true;
            }
          }).catch(function(error) {
            console.error("Failed to initialize player account:", error);
          });
        }

        if (typeof cloudStorageApi !== 'undefined' && !cloudStorageApi.isInitialized) {
          cloudStorageApi.sdk = sdk;
          cloudStorageApi.isInitialized = true;

          if (!cloudStorageApi.playerAccount) {
            sdk.getPlayer({ scopes: false }).then(function(player) {
              cloudStorageApi.playerAccount = player;
            }).catch(function(error) {
              console.error("Failed to initialize player account for cloud storage:", error);
            });
          }
        }

        if (typeof leaderboardApi !== 'undefined' && !leaderboardApi.isInitialized) {
          leaderboardApi.sdk = sdk;
          leaderboardApi.isInitialized = true;
          leaderboardApi.leaderboard = sdk.leaderboards;
        }

        yandexGamesPlugin.sendResponse(successCallbackPtr, errorCallbackPtr, { initialized: true, reused: reused }, null);
      }

      // 1. SDK already initialized externally
      if (window.ysdk && typeof window.ysdk.environment === 'object') {
        postInit(window.ysdk, true);
        return;
      }

      // 2. SDK script already loaded but not yet initialized
      if (typeof window.YaGames === 'function') {
        window.YaGames.init().then(function(sdk) {
          postInit(sdk, false);
        }).catch(function(error) {
          yandexGamesPlugin.sendResponse(successCallbackPtr, errorCallbackPtr, null, error);
        });
        return;
      }

      // 3. Load and initialize SDK
      const sdkScript = document.createElement('script');
      sdkScript.src = yandexGamesPlugin.getSdkScriptSrc();
      document.head.appendChild(sdkScript);

      sdkScript.onload = function() {
        window.YaGames.init().then(function(sdk) {
          postInit(sdk, false);
        }).catch(function(error) {
          yandexGamesPlugin.sendResponse(successCallbackPtr, errorCallbackPtr, null, error);
        });
      };

      sdkScript.onerror = function() {
        yandexGamesPlugin.sendResponse(successCallbackPtr, errorCallbackPtr, null,
            "Failed to load Yandex Games SDK script"
        );
      };
    },
    
    throwIfSdkNotInitialized: function() {
      if (!yandexGamesPlugin.isInitialized) {
        throw new Error('SDK is not initialized. Initialize SDK first using YandexGamesSDK.Initialize()');
      }
    },
    
    getEnvironment: function(successCallbackPtr, errorCallbackPtr) {
      try {
        yandexGamesPlugin.throwIfSdkNotInitialized();
        
        const environmentData = {
          app: {
            id: yandexGamesPlugin.sdk.environment.app.id
          },
          browser: {
            lang: yandexGamesPlugin.sdk.environment.browser.lang
          },
          i18n: {
            lang: yandexGamesPlugin.sdk.environment.i18n.lang,
            tld: yandexGamesPlugin.sdk.environment.i18n.tld
          },
          payload: yandexGamesPlugin.sdk.environment.payload
        };
        
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          environmentData,
          null
        );
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error
        );
      }
    },
    
    getServerTime: function(successCallbackPtr, errorCallbackPtr) {
      try {
        const now = new Date().toISOString();
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          now,
          null
        );
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error
        );
      }
    },
    
    setGameplayReady: function(successCallbackPtr, errorCallbackPtr) {
      try {
        yandexGamesPlugin.throwIfSdkNotInitialized();
        yandexGamesPlugin.sdk.features.LoadingAPI.ready();
        
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          true,
          null
        );
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error
        );
      }
    },
    
    setGameplayStart: function(successCallbackPtr, errorCallbackPtr) {
      try {
        yandexGamesPlugin.throwIfSdkNotInitialized();

        yandexGamesPlugin.sdk.features.GameplayAPI.start();

        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          true,
          null
        );
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error
        );
      }
    },
    
    setGameplayStop: function(successCallbackPtr, errorCallbackPtr) {
      try {
        yandexGamesPlugin.throwIfSdkNotInitialized();
        yandexGamesPlugin.sdk.features.GameplayAPI.stop();
        
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          true,
          null
        );
      } catch (error) {
        yandexGamesPlugin.sendResponse(
          successCallbackPtr,
          errorCallbackPtr,
          null,
          error
        );
      }
    },
    
    getDeviceType: function() {
      try {
        yandexGamesPlugin.throwIfSdkNotInitialized();
        const deviceType = yandexGamesPlugin.sdk.deviceInfo.type;
        
        switch (deviceType) {
          case 'desktop':
            return 0;
          case 'mobile':
            return 1;
          case 'tablet':
            return 2;
          case 'tv':
            return 3;
          default:
            console.error('Unexpected ysdk.deviceInfo response from Yandex. Assuming desktop. deviceType = ' + JSON.stringify(deviceType));
            return 0; // Default to Desktop
        }
      } catch (error) {
        console.error("Error getting device type:", error);
        return 0; // Default to Desktop on error
      }
    },

    isRunningOnYandex: function () {
      try {
        const hostname = window.location.hostname;
        const referrer = document.referrer;
        const userAgent = navigator.userAgent;
        const hash = window.location.hash;

        const hasYandexHost = hostname.includes('yandex') || hostname.includes('playhop');
        const hasYandexReferrer = referrer && referrer.includes('yandex');
        const hasYandexUA = userAgent.includes('YaGames'); // fallback for PWA/WebView
        const hasYandexHashOrigin = hash && decodeURIComponent(hash).includes('origin=https://yandex.ru');

        const isTopLevel = window.top === window;

        // Return true if:
        // - running directly on Yandex domain (not iframe)
        // - referrer is from Yandex (iframe case)
        // - user agent suggests Yandex container (PWA/WebView)
        // - Yandex origin passed in hash (e.g. Safari Private mode)
        return (
            (isTopLevel && hasYandexHost) ||
            hasYandexReferrer ||
            hasYandexUA ||
            hasYandexHashOrigin
        );
      } catch (e) {
        // Fallback: use referrer or hash
        const hash = window.location.hash;
        return (
            (document.referrer && document.referrer.includes('yandex')) ||
            (hash && decodeURIComponent(hash).includes('origin=https://yandex.ru'))
        );
      }
    },

    getSdkScriptSrc: function() {
      try {
        const isTopLevel = window.top === window; // Check if the page is not embedded in an iframe
        const hostname = window.location.hostname;

        const isYandexHost = hostname.includes('yandex') || hostname.includes('playhop');

        // If running as a top-level page on Yandex-hosted domain, use relative path
        if (isTopLevel && isYandexHost) {
          return '/sdk.js';
        }

        // Otherwise (iframe, external hosting, test environment), use full CDN path
        return 'https://sdk.games.s3.yandex.net/sdk.js';
      } catch (e) {
        // If window.top access is blocked (cross-origin iframe), default to full CDN path
        return 'https://sdk.games.s3.yandex.net/sdk.js';
      }
    },

    isInitializedGetter: function() {
      return yandexGamesPlugin.isInitialized ? 1 : 0;
    }
  },

  // C# external methods
  YandexGamesPlugin_Initialize: function(successCallbackPtr, errorCallbackPtr) {
    yandexGamesPlugin.initialize(successCallbackPtr, errorCallbackPtr);
  },

  YandexGamesPlugin_GetEnvironment: function(successCallbackPtr, errorCallbackPtr) {
    yandexGamesPlugin.getEnvironment(successCallbackPtr, errorCallbackPtr);
  },

  YandexGamesPlugin_GetServerTime: function(successCallbackPtr, errorCallbackPtr) {
    yandexGamesPlugin.getServerTime(successCallbackPtr, errorCallbackPtr);
  },
  
  YandexGamesPlugin_SetGameplayReady: function(successCallbackPtr, errorCallbackPtr) {
    yandexGamesPlugin.setGameplayReady(successCallbackPtr, errorCallbackPtr);
  },
  
  YandexGamesPlugin_SetGameplayStart: function(successCallbackPtr, errorCallbackPtr) {
    yandexGamesPlugin.setGameplayStart(successCallbackPtr, errorCallbackPtr);
  },
  
  YandexGamesPlugin_SetGameplayStop: function(successCallbackPtr, errorCallbackPtr) {
    yandexGamesPlugin.setGameplayStop(successCallbackPtr, errorCallbackPtr);
  },
  
  YandexGamesPlugin_IsRunningOnYandex: function() {
    return yandexGamesPlugin.isRunningOnYandex() ? 1 : 0;
  },
  
  YandexGamesPlugin_IsInitialized: function() {
    return yandexGamesPlugin.isInitializedGetter();
  },
  
  YandexGamesPlugin_GetDeviceType: function() {
    return yandexGamesPlugin.getDeviceType();
  }
};

autoAddDeps(yandexGamesPluginLibrary, '$yandexGamesPlugin');
mergeInto(LibraryManager.library, yandexGamesPluginLibrary); 

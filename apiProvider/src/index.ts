import { YandexGames } from 'YandexGamesSDK';
import { AuthModule } from './modules/auth/authModule';
import { Utils } from './system/utils';
import { StorageModule } from './modules/storage/storageModule';

window.AuthenticateUser = AuthModule.authenticateUser;
window.SavePlayerData = StorageModule.savePlayerData;
window.LoadPlayerData = StorageModule.loadPlayerData;

window.onload = function () {
  if (Utils.isLocalhost()) {
    initializeUnity();
    return;
  }

  loadYandexSDK();
};

function loadYandexSDK() {
  const script = document.createElement('script');
  script.src = 'https://yandex.ru/games/sdk/v2'; // Yandex SDK URL

  script.onload = onYandexSDKLoaded;
  script.onerror = onYandexSDKLoadFailed;

  document.head.appendChild(script);
}

function onYandexSDKLoaded() {
  if (Utils.isLocalhost()) {
    initializeUnity();
    return;
  }

  YaGames.init()
    .then(function (sdk: YandexGames.SDK) {
      window.sdk = sdk; 

      initializeUnity();
      sendUnityMessage('OnYSDKLoaded', 'YSDK loaded successfully');
      console.log("Yandex SDK loaded successfully");
    })
    .catch(function (error: any) {
      console.error("Yandex SDK failed to load:", error.message);

      initializeUnity();
      sendUnityMessage('OnYSDKLoadFailed', error.message);
    });
}

function onYandexSDKLoadFailed() {
  console.error("Failed to load Yandex SDK script");
  initializeUnity();
}

// Initialize Unity WebGL instance
function initializeUnity() {
  const buildUrl = "Build";
  const loaderUrl = `${buildUrl}/{{{ LOADER_FILENAME }}}`;

  const config = {
    dataUrl: `${buildUrl}/{{{ DATA_FILENAME }}}`,
    frameworkUrl: `${buildUrl}/{{{ FRAMEWORK_FILENAME }}}`,
    codeUrl: `${buildUrl}/{{{ CODE_FILENAME }}}`,
    streamingAssetsUrl: "StreamingAssets",
    companyName: "{{{ COMPANY_NAME }}}",
    productName: "{{{ PRODUCT_NAME }}}",
    productVersion: "{{{ PRODUCT_VERSION }}}"
  };

  const canvas = document.querySelector("#unity-canvas");
  const fullscreenButton = document.querySelector("#unity-fullscreen-button");

  // Load Unity loader script
  loadScript(loaderUrl, () => {
    createUnityInstance(canvas, config, updateLoadingProgress)
      .then((unityInstance: any) => {
        console.log("Unity WebGL instance created successfully");
        window.unityInstance = unityInstance; // Store instance globally
      })
      .catch((error: any) => {
        console.error("Failed to load Unity game:", error);
        alert("Failed to load Unity game: " + error);
      });
  });
}

// Load external script helper
function loadScript(url: string, onload: () => void) {
  const script = document.createElement('script');
  script.src = url;
  script.onload = onload;
  script.onerror = () => alert("Failed to load Unity loader script.");
  document.body.appendChild(script);
}

// Unity loading progress callback
function updateLoadingProgress(progress: number) {
  // console.log(`Loading Unity progress: ${progress * 100}%`);
}

// Send message to Unity
function sendUnityMessage(functionName: string, message: string) {
  if (window.unityInstance) {
    window.unityInstance.SendMessage('YandexGamesSDK', functionName, message);
  }
}

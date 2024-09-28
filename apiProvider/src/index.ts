import { AuthModule } from './modules/auth/authModule';
import { StorageModule } from './modules/storage/storageModule';
import { Utils } from './system/utils';

window.YandexSDKExports = {
  AuthenticateUser: AuthModule.authenticateUser,
  SavePlayerData: StorageModule.savePlayerData,
  LoadPlayerData: StorageModule.loadPlayerData,
};

window.addEventListener('load', () => {
  if (Utils.isLocalhost()) {
    initializeUnity();
  } else {
    loadYandexSDK();
  }
});

function loadYandexSDK() {
  const script = document.createElement('script');
  script.src = 'https://yandex.ru/games/sdk/v2'; // Yandex SDK URL
  script.onload = onYandexSDKLoaded;
  script.onerror = onYandexSDKLoadFailed;
  document.head.appendChild(script);
}

function onYandexSDKLoaded() {
  YaGames.init()
    .then((sdk: any) => {
      window.yandexSDK = sdk;
      console.log('Yandex SDK loaded successfully');
      sendUnityMessage('OnYSDKLoaded', 'YSDK loaded successfully');
      initializeUnity();
    })
    .catch((error: any) => {
      console.error('Yandex SDK failed to load:', error.message);
      sendUnityMessage('OnYSDKLoadFailed', error.message);
      initializeUnity();
    });
}

function onYandexSDKLoadFailed() {
  console.error('Failed to load Yandex SDK script');
  initializeUnity();
}

// Initialize Unity WebGL instance
function initializeUnity() {
  const buildUrl = 'Build';
  const loaderUrl = `${buildUrl}/{{{ LOADER_FILENAME }}}`;

  const config = {
    dataUrl: `${buildUrl}/{{{ DATA_FILENAME }}}`,
    frameworkUrl: `${buildUrl}/{{{ FRAMEWORK_FILENAME }}}`,
    codeUrl: `${buildUrl}/{{{ CODE_FILENAME }}}`,
    streamingAssetsUrl: 'StreamingAssets',
    companyName: '{{{ COMPANY_NAME }}}',
    productName: '{{{ PRODUCT_NAME }}}',
    productVersion: '{{{ PRODUCT_VERSION }}}',
  };

  const canvas = document.querySelector('#unity-canvas') as HTMLCanvasElement;

  // Load Unity loader script
  loadScript(loaderUrl, () => {
    createUnityInstance(canvas, config, updateLoadingProgress)
      .then((unityInstance: any) => {
        console.log('Unity WebGL instance created successfully');
        window.unityInstance = unityInstance; // Store instance globally
      })
      .catch((error: any) => {
        console.error('Failed to load Unity game:', error);
        alert('Failed to load Unity game: ' + error);
      });
  });
}

// Load external script helper
function loadScript(url: string, onload: () => void) {
  const script = document.createElement('script');
  script.src = url;
  script.onload = onload;
  script.onerror = () => {
    console.error('Failed to load script:', url);
    alert('Failed to load Unity loader script.');
  };
  document.body.appendChild(script);
}

// Unity loading progress callback
function updateLoadingProgress(progress: number) {
  // Optional: Update a loading UI if necessary
  // console.log(`Loading Unity progress: ${progress * 100}%`);
}

// Send message to Unity
function sendUnityMessage(functionName: string, message: string) {
  if (window.unityInstance) {
    window.unityInstance.SendMessage('YandexGamesSDK', functionName, message);
  } else {
    console.warn('Unity instance is not initialized.');
  }
}

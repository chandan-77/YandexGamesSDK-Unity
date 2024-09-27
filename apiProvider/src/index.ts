import { AuthModule } from './modules/auth/authModule';
import { Utils } from './system/utils';

window.AuthenticateUser = AuthModule.AuthenticateUser;
window.isLocalHost = Utils.isLocalhost;

// Initialize Yandex SDK and Unity WebGL instance after the page is loaded
window.onload = function () {
  // Load Yandex SDK

  if (window.isLocalHost()) {
    initializeUnity();
    return;
  }
  const script = document.createElement('script');
  script.src = 'https://yandex.ru/games/sdk/v2'; // Load Yandex SDK

  script.onload = function () {
    if (!window.isLocalHost()) {
      YaGames.init()
        .then(function (ysdk: any) {
          // Store the SDK globally for use in your AuthModule
          window.ysdk = ysdk;

          initializeUnity();

          unityInstance.SendMessage('YandexGamesSDK', 'OnYSDKLoaded', 'YSDK loaded successfully');

          console.log("Yandex SDK loaded");
        })
        .catch(function (error: any) {
          console.error("Yandex SDK failed to load:", error.message);

          initializeUnity();

          unityInstance.SendMessage('YandexGamesSDK', 'OnYSDKLoadFailed', error.message);
        });

    }


  };

  script.onerror = function () {
    console.error("Failed to load Yandex SDK script");

    initializeUnity();
  };

  document.head.appendChild(script);
};

function initializeUnity() {
  var buildUrl = "Build"; // Path to the Unity WebGL build folder
  var loaderUrl = buildUrl + "/{{{ LOADER_FILENAME }}}"; // Replace with actual loader filename

  // Unity configuration
  var config = {
    dataUrl: buildUrl + "/{{{ DATA_FILENAME }}}", // Unity data file
    frameworkUrl: buildUrl + "/{{{ FRAMEWORK_FILENAME }}}", // Unity framework file
    codeUrl: buildUrl + "/{{{ CODE_FILENAME }}}", // Unity WebAssembly (WASM) file
    streamingAssetsUrl: "StreamingAssets", // Path for streaming assets
    companyName: "{{{ COMPANY_NAME }}}", // Unity project company name
    productName: "{{{ PRODUCT_NAME }}}", // Unity project product name
    productVersion: "{{{ PRODUCT_VERSION }}}", // Unity project version
  };

  // DOM elements
  const canvas = document.querySelector("#unity-canvas"); // Unity canvas
  const loadingCover = document.querySelector("#loading-cover"); // Loading cover
  const fullscreenButton = document.querySelector("#unity-fullscreen-button"); // Fullscreen button

  // Check if fullscreen is supported
  const canFullscreen = (function () {
    for (const key of [
      'exitFullscreen',
      'webkitExitFullscreen',
      'webkitCancelFullScreen',
      'mozCancelFullScreen',
      'msExitFullscreen'
    ]) {
      if (key in document) {
        return true;
      }
    }
    return false;
  }());

  // Mobile optimizations (optional)
  if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
  }

  // Load Unity loader script
  const script = document.createElement("script");
  script.src = loaderUrl;
  script.onload = () => {
    createUnityInstance(canvas, config, (progress: any) => {
      // Update progress (optional)
      console.log(`Loading Unity progress: ${progress * 100}%`);
    }).then((unityInstance: any) => {
      console.log("Unity WebGL instance created successfully");
      window.unityInstance = unityInstance; // Store instance globally for further use

    }).catch((error: any) => {
      alert("Failed to load Unity game: " + error);
      console.error("Unity game loading failed:", error);
    });
  };

  script.onerror = () => {
    alert("Failed to load Unity loader script.");
  };

  // Append Unity loader script to the document
  document.body.appendChild(script);
}

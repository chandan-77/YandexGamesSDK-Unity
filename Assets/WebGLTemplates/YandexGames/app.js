// const hideFullScreenButton = "{{{ HIDE_FULL_SCREEN_BUTTON }}}";
// const buildUrl = "Build";
// const loaderUrl = buildUrl + "/{{{ LOADER_FILENAME }}}";
// const config = {
//     dataUrl: buildUrl + "/{{{ DATA_FILENAME }}}",
//     frameworkUrl: buildUrl + "/{{{ FRAMEWORK_FILENAME }}}",
//     codeUrl: buildUrl + "/{{{ CODE_FILENAME }}}",
//     #if MEMORY_FILENAME
//     memoryUrl: buildUrl + "/{{{ MEMORY_FILENAME }}}",
//     #endif
//     #if SYMBOLS_FILENAME
//     symbolsUrl: buildUrl + "/{{{ SYMBOLS_FILENAME }}}",
//     #endif
//     streamingAssetsUrl: "StreamingAssets",
//     companyName: "{{{ COMPANY_NAME }}}",
//     productName: "{{{ PRODUCT_NAME }}}",
//     productVersion: "{{{ PRODUCT_VERSION }}}",
// };
//
// const container = document.querySelector("#unity-container");
// const canvas = document.querySelector("#unity-canvas");
// const loadingCover = document.querySelector("#loading-cover");
// //   const progressBarEmpty = document.querySelector("#unity-progress-bar-empty");
// //   const progressBarFull = document.querySelector("#unity-progress-bar-full");
// const fullscreenButton = document.querySelector("#unity-fullscreen-button");
// const spinner = document.querySelector('.spinner');
//
// const canFullscreen = (function() {
//     for (const key of [
//         'exitFullscreen',
//         'webkitExitFullscreen',
//         'webkitCancelFullScreen',
//         'mozCancelFullScreen',
//         'msExitFullscreen',
//     ]) {
//         if (key in document) {
//             return true;
//         }
//     }
//     return false;
// }());
//
// if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
//     container.className = "unity-mobile";
//     config.devicePixelRatio = 1;
// }
// #if BACKGROUND_FILENAME
// canvas.style.background = "url('" + buildUrl + "/{{{ BACKGROUND_FILENAME.replace(/'/g, '%27') }}}') center / cover";
// #endif
// loadingCover.style.display = "";
//
// const script = document.createElement("script");
// script.src = loaderUrl;
// script.onload = () => {
//     createUnityInstance(canvas, config, (progress) => {
//         //   spinner.style.display = "none";
//         //   progressBarEmpty.style.display = "";
//         //   progressBarFull.style.width = `${100 * progress}%`;
//     }).then((unityInstance) => {
//         loadingCover.style.display = "none";
//         if (canFullscreen) {
//             if (!hideFullScreenButton) {
//                 fullscreenButton.style.display = "";
//             }
//             fullscreenButton.onclick = () => {
//                 unityInstance.SetFullscreen(1);
//             };
//         }
//     }).catch((message) => {
//         alert(message);
//     });
// };
// document.body.appendChild(script);
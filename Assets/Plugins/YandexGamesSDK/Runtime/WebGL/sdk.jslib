/******/ (() => { // webpackBootstrap
/******/ 	// runtime can't be in strict mode because a global variable is assign and maybe created.
var __webpack_exports__ = {};
// This entry need to be wrapped in an IIFE because it need to be in strict mode.
(() => {
"use strict";

;// ./src/modules/authModule.ts
var AuthModule = /** @class */ (function () {
    function AuthModule() {
    }
    AuthModule.AuthenticateUser = function () {
        if (typeof window.ysdk !== 'undefined') {
            window.ysdk.getPlayer()
                .then(function (player) {
                var profile = {
                    id: player.getUniqueID(),
                    name: player.getName(),
                    avatarUrlSmall: player.getPhoto('small'),
                    avatarUrlMedium: player.getPhoto('medium'),
                    avatarUrlLarge: player.getPhoto('large'),
                };
                SendMessage('YandexGamesSDK', 'OnAuthenticationSuccess', JSON.stringify(profile));
            })
                .catch(function (error) {
                SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', error.message || 'Error during authentication');
            });
        }
        else {
            SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', 'Yandex SDK not initialized.');
        }
    };
    return AuthModule;
}());


;// ./src/index.ts

mergeInto(LibraryManager.library, {
    AuthenticateUser: AuthModule.AuthenticateUser,
});

})();

var __webpack_export_target__ = (l = typeof l === "undefined" ? {} : l);
for(var i in __webpack_exports__) __webpack_export_target__[i] = __webpack_exports__[i];
if(__webpack_exports__.__esModule) Object.defineProperty(__webpack_export_target__, "__esModule", { value: true });
/******/ })()
;
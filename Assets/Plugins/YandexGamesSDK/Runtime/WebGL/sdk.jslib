var ApiProvider;
/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
var __webpack_exports__ = {};

;// ./src/modules/authModule.ts
var __awaiter = (undefined && undefined.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (undefined && undefined.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g = Object.create((typeof Iterator === "function" ? Iterator : Object).prototype);
    return g.next = verb(0), g["throw"] = verb(1), g["return"] = verb(2), typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var AuthModule = /** @class */ (function () {
    function AuthModule() {
    }
    AuthModule.AuthenticateUser = function () {
        return __awaiter(this, void 0, void 0, function () {
            var player, profile, error_1;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!(typeof window.ysdk !== 'undefined')) return [3 /*break*/, 5];
                        _a.label = 1;
                    case 1:
                        _a.trys.push([1, 3, , 4]);
                        return [4 /*yield*/, window.ysdk.getPlayer()];
                    case 2:
                        player = _a.sent();
                        profile = {
                            id: player.getUniqueID(),
                            name: player.getName(),
                            avatarUrlSmall: player.getPhoto('small'),
                            avatarUrlMedium: player.getPhoto('medium'),
                            avatarUrlLarge: player.getPhoto('large'),
                        };
                        // Send success message back to Unity
                        window.SendMessage('YandexGamesSDK', 'OnAuthenticationSuccess', JSON.stringify(profile));
                        return [3 /*break*/, 4];
                    case 3:
                        error_1 = _a.sent();
                        // Send failure message back to Unity
                        window.SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', error_1.message);
                        return [3 /*break*/, 4];
                    case 4: return [3 /*break*/, 6];
                    case 5:
                        // SDK is not initialized
                        window.SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', 'Yandex SDK not initialized.');
                        _a.label = 6;
                    case 6: return [2 /*return*/];
                }
            });
        });
    };
    return AuthModule;
}());


;// ./src/index.ts

var loadYandexSDK = function () {
    return new Promise(function (resolve, reject) {
        if (typeof window.ysdk === 'undefined') {
            // Create a script element
            var script = document.createElement('script');
            script.src = 'https://yandex.ru/games/sdk/v2'; // Yandex SDK URL
            script.async = true;
            // On successful loading of the SDK
            script.onload = function () {
                window.YaGames.init()
                    .then(function (ysdk) {
                    window.ysdk = ysdk;
                    console.log('Yandex SDK initialized successfully');
                    resolve();
                })
                    .catch(function (error) {
                    console.error('Failed to initialize Yandex SDK:', error);
                    reject(error);
                });
            };
            // On script loading error
            script.onerror = function () {
                reject(new Error('Failed to load Yandex SDK script'));
            };
            // Add the script tag to the document
            document.head.appendChild(script);
        }
        else {
            resolve();
        }
    });
};
// Load the Yandex SDK and merge the functions into the Unity library
loadYandexSDK()
    .then(function () {
    if (typeof mergeInto !== 'undefined' && typeof LibraryManager !== 'undefined') {
        mergeInto(LibraryManager.library, {
            AuthenticateUser: AuthModule.AuthenticateUser, // Attach the AuthModule's method after YSDK is loaded
        });
        console.log('AuthenticateUser function added to the Unity library.');
    }
})
    .catch(function (error) {
    console.error('Error loading Yandex SDK:', error);
});

ApiProvider = __webpack_exports__;
/******/ })()
;
import { AuthModule } from './modules/auth/authModule';
import { YandexGamesSDK } from './modules/YandexGamesSDK';
import { StorageModule } from './modules/storage/storageModule';

window.YandexSDKVersion = YANDEX_SDK_VERSION;


window.YandexSDKExports = {
  AuthenticateUser: AuthModule.authenticateUser,
  SavePlayerData: StorageModule.savePlayerData,
  LoadPlayerData: StorageModule.loadPlayerData,
  CheckForInitialization: YandexGamesSDK.checkForInitialization,
  OnYandexGamesSDKReady: YandexGamesSDK.OnYandexGamesSDKReady
};

window.addEventListener('load', () => {
  YandexGamesSDK.initialize();
});

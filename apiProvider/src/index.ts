import { AuthModule } from './modules/auth/authModule';
import { YandexGamesSDK } from './modules/YandexGamesSDK';
import { StorageModule } from './modules/storage/storageModule';
import { LeaderboardModule } from './modules/leaderboard/leaderboardModule';

window.YandexSDKVersion = YANDEX_SDK_VERSION;


window.YandexSDKExports = {
  AuthenticateUser: AuthModule.authenticateUser,
  SavePlayerData: StorageModule.savePlayerData,
  LoadPlayerData: StorageModule.loadPlayerData,
  CheckForInitialization: YandexGamesSDK.checkForInitialization,
  OnYandexGamesSDKReady: YandexGamesSDK.OnYandexGamesSDKReady,
  SubmitScore: LeaderboardModule.submitScore,
  GetLeaderboardEntries: LeaderboardModule.getLeaderboardEntries,
  GetPlayerEntry: LeaderboardModule.getPlayerEntry,
};


window.addEventListener('load', () => {
  YandexGamesSDK.initialize();
});

import { AuthModule } from './modules/auth/authModule';
import { YandexGamesSDK } from './modules/YandexGamesSDK';
import { StorageModule } from './modules/storage/storageModule';
import { LeaderboardModule } from './modules/leaderboard/leaderboardModule';
import { GameplayModule } from './modules/gameplay/gameplayModule';
import { AdvertisementModule } from './modules/ads/advertisementModule';
import { IAPModule } from './modules/iap/iapModule';

window.YandexSDKVersion = YANDEX_SDK_VERSION;

window.SendResponseToUnity = function SendResponseToUnity(requestId: string, status: boolean, data: any, error: any) {
  const response = {
    requestId: requestId,
    status: status,
    data: data != null ? data : null,
    error: error != null ? error : status ? null : 'Unknown error'
  };
  const jsonResponse = JSON.stringify(response);
  unityInstance.SendMessage('YandexGamesSDK', 'OnJSResponse', jsonResponse);
}


window.YandexSDKExports = {
  // Authentication
  AuthenticateUser: AuthModule.authenticateUser,

  // Storage
  SavePlayerData: StorageModule.savePlayerData,
  LoadPlayerData: StorageModule.loadPlayerData,

  // Initialization Checks
  CheckForInitialization: YandexGamesSDK.checkForInitialization,
  OnYandexGamesSDKReady: YandexGamesSDK.OnYandexGamesSDKReady,

  // Server Time and Environment
  GetServerTime: YandexGamesSDK.getServerTime,
  GetEnvironment: YandexGamesSDK.getEnvironment,

  // Leaderboards
  SubmitScore: LeaderboardModule.submitScore,
  GetLeaderboardEntries: LeaderboardModule.getLeaderboardEntries,
  GetPlayerEntry: LeaderboardModule.getPlayerEntry,

  // Gameplay Markup
  SetGameplayReady: GameplayModule.setGameplayReady,
  SetGameplayStart: GameplayModule.setGameplayStart,
  SetGameplayStop: GameplayModule.setGameplayStop,

  // Advertisements
  HideBannerAd: AdvertisementModule.hideBannerAd,
  ShowBannerAd: AdvertisementModule.showBannerAd,
  ShowInterstitialAd: AdvertisementModule.showInterstitialAd,
  ShowRewardedAd: AdvertisementModule.showRewardedAd,

  // IAP
  InitializePayments: IAPModule.initializePayments,
  PurchaseProduct: IAPModule.purchaseProduct,
  GetPurchases: IAPModule.getPurchases,
  ConsumePurchase: IAPModule.consumePurchase,
  GetCatalog: IAPModule.getCatalog,
};



window.addEventListener('load', () => {
  YandexGamesSDK.initialize();
});

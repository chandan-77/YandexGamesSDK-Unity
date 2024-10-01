mergeInto(LibraryManager.library, {

  AuthenticateUser: function(requireSigninPtr) {
    if (typeof window.YandexSDKExports.AuthenticateUser === 'function') {
      var requireSignin = requireSigninPtr;
      window.YandexSDKExports.AuthenticateUser(requireSignin);
    } else {
      console.error('AuthenticateUser is not defined on window.YandexSDKExports.');
    }
  },

  SavePlayerData: function(keyPtr, dataPtr, flushPtr) {
    if (typeof window.YandexSDKExports.SavePlayerData === 'function') {
      var key = UTF8ToString(keyPtr);
var data = UTF8ToString(dataPtr);
var flush = flushPtr;
      window.YandexSDKExports.SavePlayerData(key, data, flush);
    } else {
      console.error('SavePlayerData is not defined on window.YandexSDKExports.');
    }
  },

  LoadPlayerData: function(keyPtr) {
    if (typeof window.YandexSDKExports.LoadPlayerData === 'function') {
      var key = UTF8ToString(keyPtr);
      window.YandexSDKExports.LoadPlayerData(key);
    } else {
      console.error('LoadPlayerData is not defined on window.YandexSDKExports.');
    }
  },

  CheckForInitialization: function() {
    if (typeof window.YandexSDKExports.CheckForInitialization === 'function') {
      
      window.YandexSDKExports.CheckForInitialization();
    } else {
      console.error('CheckForInitialization is not defined on window.YandexSDKExports.');
    }
  },

  OnYandexGamesSDKReady: function() {
    if (typeof window.YandexSDKExports.OnYandexGamesSDKReady === 'function') {
      
      window.YandexSDKExports.OnYandexGamesSDKReady();
    } else {
      console.error('OnYandexGamesSDKReady is not defined on window.YandexSDKExports.');
    }
  },

  GetServerTime: function() {
    if (typeof window.YandexSDKExports.GetServerTime === 'function') {
      
      window.YandexSDKExports.GetServerTime();
    } else {
      console.error('GetServerTime is not defined on window.YandexSDKExports.');
    }
  },

  GetEnvironment: function() {
    if (typeof window.YandexSDKExports.GetEnvironment === 'function') {
      
      window.YandexSDKExports.GetEnvironment();
    } else {
      console.error('GetEnvironment is not defined on window.YandexSDKExports.');
    }
  },

  SubmitScore: function(leaderboardNamePtr, scorePtr) {
    if (typeof window.YandexSDKExports.SubmitScore === 'function') {
      var leaderboardName = UTF8ToString(leaderboardNamePtr);
var score = scorePtr;
      window.YandexSDKExports.SubmitScore(leaderboardName, score);
    } else {
      console.error('SubmitScore is not defined on window.YandexSDKExports.');
    }
  },

  GetLeaderboardEntries: function(leaderboardNamePtr, offsetPtr, limitPtr) {
    if (typeof window.YandexSDKExports.GetLeaderboardEntries === 'function') {
      var leaderboardName = UTF8ToString(leaderboardNamePtr);
var offset = offsetPtr;
var limit = limitPtr;
      window.YandexSDKExports.GetLeaderboardEntries(leaderboardName, offset, limit);
    } else {
      console.error('GetLeaderboardEntries is not defined on window.YandexSDKExports.');
    }
  },

  GetPlayerEntry: function(leaderboardNamePtr) {
    if (typeof window.YandexSDKExports.GetPlayerEntry === 'function') {
      var leaderboardName = UTF8ToString(leaderboardNamePtr);
      window.YandexSDKExports.GetPlayerEntry(leaderboardName);
    } else {
      console.error('GetPlayerEntry is not defined on window.YandexSDKExports.');
    }
  },

  SetGameplayReady: function() {
    if (typeof window.YandexSDKExports.SetGameplayReady === 'function') {
      
      window.YandexSDKExports.SetGameplayReady();
    } else {
      console.error('SetGameplayReady is not defined on window.YandexSDKExports.');
    }
  },

  SetGameplayStart: function() {
    if (typeof window.YandexSDKExports.SetGameplayStart === 'function') {
      
      window.YandexSDKExports.SetGameplayStart();
    } else {
      console.error('SetGameplayStart is not defined on window.YandexSDKExports.');
    }
  },

  SetGameplayStop: function() {
    if (typeof window.YandexSDKExports.SetGameplayStop === 'function') {
      
      window.YandexSDKExports.SetGameplayStop();
    } else {
      console.error('SetGameplayStop is not defined on window.YandexSDKExports.');
    }
  },

  HideBannerAd: function() {
    if (typeof window.YandexSDKExports.HideBannerAd === 'function') {
      
      window.YandexSDKExports.HideBannerAd();
    } else {
      console.error('HideBannerAd is not defined on window.YandexSDKExports.');
    }
  },

  ShowBannerAd: function(positionPtr) {
    if (typeof window.YandexSDKExports.ShowBannerAd === 'function') {
      var position = UTF8ToString(positionPtr);
      window.YandexSDKExports.ShowBannerAd(position);
    } else {
      console.error('ShowBannerAd is not defined on window.YandexSDKExports.');
    }
  },

  ShowInterstitialAd: function() {
    if (typeof window.YandexSDKExports.ShowInterstitialAd === 'function') {
      
      window.YandexSDKExports.ShowInterstitialAd();
    } else {
      console.error('ShowInterstitialAd is not defined on window.YandexSDKExports.');
    }
  },

  ShowRewardedAd: function() {
    if (typeof window.YandexSDKExports.ShowRewardedAd === 'function') {
      
      window.YandexSDKExports.ShowRewardedAd();
    } else {
      console.error('ShowRewardedAd is not defined on window.YandexSDKExports.');
    }
  },
});
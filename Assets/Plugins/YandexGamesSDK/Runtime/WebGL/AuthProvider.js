mergeInto(LibraryManager.library, {
  AuthenticateUser: function () {
    if (typeof ysdk !== 'undefined') {
      ysdk.getPlayer().then(function(player) {
        var profile = {
          id: player.getUniqueID(),
          name: player.getName(),
          avatarUrlSmall: player.getPhoto('small'),
          avatarUrlMedium: player.getPhoto('medium'),
          avatarUrlLarge: player.getPhoto('large'),
          // Add other properties if needed
        };
        SendMessage('YandexGamesSDK', 'OnAuthenticationSuccess', JSON.stringify(profile));
      }).catch(function(error) {
        SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', error.message);
      });
    } else {
      SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', 'Yandex SDK not initialized.');
    }
  }
});

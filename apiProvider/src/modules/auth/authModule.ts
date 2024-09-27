export class AuthModule {
  static AuthenticateUser(): void {
    if (!window.isLocalHost()) {
      window.ysdk.getPlayer()
        .then((player: any) => {
          const profile = {
            id: player.getUniqueID(),
            name: player.getName(),
            avatarUrlSmall: player.getPhoto('small'),
            avatarUrlMedium: player.getPhoto('medium'),
            avatarUrlLarge: player.getPhoto('large'),
          };

          unityInstance.SendMessage('YandexGamesSDK', 'OnAuthenticationSuccess', JSON.stringify(profile));
        })
        .catch((error: any) => {
          unityInstance.SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', error.message || 'Error during authentication');
        });
    } else {
      const profile = {
        id: "10223",
        name: "Turan",
      };

      unityInstance.SendMessage('YandexGamesSDK', 'OnAuthenticationSuccess', JSON.stringify(profile));

    }

  }
}

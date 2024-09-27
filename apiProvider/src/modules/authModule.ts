
export class AuthModule {
  static AuthenticateUser(): void {
    if (typeof window.ysdk !== 'undefined') {
      window.ysdk.getPlayer()
        .then((player: any) => {
          const profile = {
            id: player.getUniqueID(),
            name: player.getName(),
            avatarUrlSmall: player.getPhoto('small'),
            avatarUrlMedium: player.getPhoto('medium'),
            avatarUrlLarge: player.getPhoto('large'),
          };

          SendMessage('YandexGamesSDK', 'OnAuthenticationSuccess', JSON.stringify(profile));
        })
        .catch((error: any) => {
          SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', error.message || 'Error during authentication');
        });
    } else {
      SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', 'Yandex SDK not initialized.');
    }
  }
}

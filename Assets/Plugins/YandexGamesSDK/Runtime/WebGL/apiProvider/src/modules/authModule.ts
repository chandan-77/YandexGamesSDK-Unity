export class AuthModule {
    static async AuthenticateUser(): Promise<void> {
      if (typeof window.ysdk !== 'undefined') {
        try {
          const player = await window.ysdk.getPlayer();
          
          const profile = {
            id: player.getUniqueID(),
            name: player.getName(),
            avatarUrlSmall: player.getPhoto('small'),
            avatarUrlMedium: player.getPhoto('medium'),
            avatarUrlLarge: player.getPhoto('large'),
          };
  
          // Send success message back to Unity
          window.SendMessage('YandexGamesSDK', 'OnAuthenticationSuccess', JSON.stringify(profile));
        } catch (error:any) {
          // Send failure message back to Unity
          window.SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', error.message);
        }
      } else {
        // SDK is not initialized
        window.SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', 'Yandex SDK not initialized.');
      }
    }
  }
  
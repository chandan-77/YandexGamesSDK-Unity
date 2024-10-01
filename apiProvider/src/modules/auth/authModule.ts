import { YandexGames } from "YandexGamesSDK";

export class AuthModule {
  static async authenticateUser(requireSignin: boolean): Promise<void> {
    try {
      console.log("req sig is " + requireSignin);
      if (!window.yandexSDK) {
        throw new Error("Yandex SDK is not available on the window object.");
      }

      const sdk: YandexGames.SDK = await window.yandexSDK;
      const player: YandexGames.Player = await sdk.getPlayer();

      if ( Boolean(requireSignin) && player.getMode() === 'lite') {
        await sdk.auth.openAuthDialog();
      }

      const isAuthorized = player.getUniqueID() !== '';
      console.log("Player is authorized:", isAuthorized);

      const profile = {
        id: player.getUniqueID(),
        name: player.getName(),
        avatarUrlSmall: player.getPhoto('small'),
        avatarUrlMedium: player.getPhoto('medium'),
        avatarUrlLarge: player.getPhoto('large'),
        isAuthorized: isAuthorized
      };

      unityInstance.SendMessage('YandexGamesSDK', 'OnAuthenticationSuccess', JSON.stringify(profile));
      console.log("Authentication success message sent to Unity.");

    } catch (error: any) {
      // Enhanced error handling
      console.error("Error during user authentication:", error.message || error);
      unityInstance.SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', error.message || 'An unknown error occurred during authentication.');
    }
  }

}

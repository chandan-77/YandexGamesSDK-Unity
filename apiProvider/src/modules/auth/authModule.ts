import { YandexGames } from "YandexGamesSDK";

export class AuthModule {
  static async authenticateUser(requestId: string, requireSignin: boolean) {
    try {
      console.log("Authentication requested. Require sign-in:", requireSignin);

      if (!window.yandexSDK) {
        throw new Error("Yandex SDK is not available on the window object.");
      }

      const sdk = await window.yandexSDK;
      const player = await sdk.getPlayer();

      if (Boolean(requireSignin) && player.getMode() === 'lite') {
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

      window.SendResponseToUnity(requestId, true, JSON.stringify(profile), null);
      console.log("Authentication success message sent to Unity.");
    } catch (error: any) {

      console.error("Error during user authentication:", error.message || error);
      window.SendResponseToUnity(requestId, false, null, error.message || 'An unknown error occurred during authentication.');
    }
  }
}


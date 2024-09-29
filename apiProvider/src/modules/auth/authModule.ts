import { YandexGames } from "YandexGamesSDK";
import { Utils } from "../../system/utils";

export class AuthModule {
  static async authenticateUser(): Promise<void> {
    if (!Utils.isLocalhost()) {
      try {
        const player: YandexGames.Player = await window.yandexSDK.getPlayer();

        const isAuthorized = player.getUniqueID() !== '';

        const profile = {
          id: player.getUniqueID(),
          name: player.getName(),
          avatarUrlSmall: player.getPhoto('small'),
          avatarUrlMedium: player.getPhoto('medium'),
          avatarUrlLarge: player.getPhoto('large'),
          isAuthorized: isAuthorized
        };

        unityInstance.SendMessage('YandexGamesSDK', 'OnAuthenticationSuccess', JSON.stringify(profile));

      } catch (error: any) {
        unityInstance.SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', error.message);
      }

    } else {
      const profile = {
        id: "10223",
        name: "Turan",
        isAuthorized: true // Set to true or false depending on your testing needs
      };

      unityInstance.SendMessage('YandexGamesSDK', 'OnAuthenticationSuccess', JSON.stringify(profile));
    }
  }
}

import { YandexGames } from "YandexGamesSDK";
import { Utils } from "../../system/utils";

export class AuthModule {
  static async authenticateUser(): Promise<void> {
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
  }
}

import { Utils } from "../../system/utils";

export class AuthModule {
  static async authenticateUser(): Promise<void> {
    if (!Utils.isLocalhost()) {
      try {
        var player: PlayerProfile = await window.sdk.getPlayer();
        const profile = {
          id: player.getUniqueID(),
          name: player.getName(),
          avatarUrlSmall: player.getPhoto('small'),
          avatarUrlMedium: player.getPhoto('medium'),
          avatarUrlLarge: player.getPhoto('large'),
        };

        unityInstance.SendMessage('YandexGamesSDK', 'OnAuthenticationSuccess', JSON.stringify(profile));

      } catch (error: any) {
        unityInstance.SendMessage('YandexGamesSDK', 'OnAuthenticationFailure', error.message);

      }

    } else {
      const profile = {
        id: "10223",
        name: "Turan",
      };

      unityInstance.SendMessage('YandexGamesSDK', 'OnAuthenticationSuccess', JSON.stringify(profile));

    }

  }
}

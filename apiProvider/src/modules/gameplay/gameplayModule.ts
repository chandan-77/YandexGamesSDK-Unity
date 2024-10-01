import { YandexGames } from "YandexGamesSDK";

export class GameplayModule {
    static async setGameplayReady(): Promise<void> {
        try {
            const sdk: YandexGames.SDK = await window.yandexSDK;

            sdk.features.LoadingAPI?.ready()
        } catch (error: any) {
            console.error("Failed to send LoadingAPI ready: ", error.message);
        }
    }

    static async setGameplayStart(): Promise<void> {
        try {
            const sdk: YandexGames.SDK = await window.yandexSDK;

            sdk.features.GameplayAPI.start()
        } catch (error: any) {
            console.error("Failed to send GameplayAPI start: ", error.message);
        }
    }

    static async setGameplayStop(): Promise<void> {
        try {
            const sdk: YandexGames.SDK = await window.yandexSDK;

            sdk.features.GameplayAPI.stop()
        } catch (error: any) {
            console.error("Failed to send GameplayAPI stop: ", error.message);
        }
    }
}

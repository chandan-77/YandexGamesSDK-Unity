import { YandexGames } from "YandexGamesSDK";

export class GameplayModule {
    /**
     * Notify that the game has finished loading and is ready.
     * @param {string} requestId - The unique request ID.
     */
    static async setGameplayReady(requestId: string): Promise<void> {
        try {
            const sdk: YandexGames.SDK = await window.yandexSDK;

            await sdk.features.LoadingAPI.ready();

            // Send success response
            window.SendResponseToUnity(requestId, true, null, null);
            console.log("Gameplay ready sent successfully to Unity.");
        } catch (error: any) {
            console.error("Failed to send LoadingAPI ready:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to send LoadingAPI ready.');
        }
    }

    /**
     * Notify that gameplay has started or resumed.
     * @param {string} requestId - The unique request ID.
     */
    static async setGameplayStart(requestId: string): Promise<void> {
        try {
            const sdk: YandexGames.SDK = await window.yandexSDK;

            await sdk.features.GameplayAPI.start();

            // Send success response
            window.SendResponseToUnity(requestId, true, null, null);
            console.log("Gameplay start sent successfully to Unity.");
        } catch (error: any) {
            console.error("Failed to send GameplayAPI start:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to send GameplayAPI start.');
        }
    }

    /**
     * Notify that gameplay has paused or stopped.
     * @param {string} requestId - The unique request ID.
     */
    static async setGameplayStop(requestId: string): Promise<void> {
        try {
            const sdk: YandexGames.SDK = await window.yandexSDK;

            await sdk.features.GameplayAPI.stop();

            // Send success response
            window.SendResponseToUnity(requestId, true, null, null);
            console.log("Gameplay stop sent successfully to Unity.");
        } catch (error: any) {
            console.error("Failed to send GameplayAPI stop:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to send GameplayAPI stop.');
        }
    }
}

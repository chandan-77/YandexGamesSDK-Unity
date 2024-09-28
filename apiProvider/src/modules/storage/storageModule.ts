import { YandexGames } from "YandexGamesSDK";

export class StorageModule {
    static async savePlayerData(key: string, data: string): Promise<void> {
        try {
            const player = await window.sdk.getPlayer(); // Cast window to any to get the Yandex SDK player instance

            // Set the data as a key-value object
            await player.setData({ [key]: data }, true); // flush=true to ensure immediate data saving

            // Notify Unity about successful data saving
            unityInstance.SendMessage('YandexGamesSDK', 'OnPlayerDataSaved', 'true');
        } catch (error: any) {
            console.error("Failed to save player data:", error.message);
            // Notify Unity about failure, including the error message
            unityInstance.SendMessage('YandexGamesSDK', 'OnPlayerDataSaved', `false|${error.message}`);
        }
    }

    static async loadPlayerData(key: string): Promise<void> {
        try {
            const player:YandexGames.Player = await window.sdk.getPlayer(); // Cast window to any to get the Yandex SDK player instance

            // Fetch the data for the given key
            const data = await player.getData([key]);

            console.log(data);

            if (data !== undefined) {
                // If the data for the key is found, send it back to Unity
                unityInstance.SendMessage('YandexGamesSDK', 'OnPlayerDataLoaded', JSON.stringify({ success: true, data: data[key] }));
            } else {
                // If no data is found, notify Unity
                unityInstance.SendMessage('YandexGamesSDK', 'OnPlayerDataLoaded', 'false|No data found for the given key');
            }
        } catch (error: any) {
            console.error("Failed to load player data:", error.message);
            // Notify Unity about failure, including the error message
            unityInstance.SendMessage('YandexGamesSDK', 'OnPlayerDataLoaded', `false|${error.message}`);
        }
    }
}

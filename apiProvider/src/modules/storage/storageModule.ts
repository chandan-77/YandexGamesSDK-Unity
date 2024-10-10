export class StorageModule {
    static async savePlayerData(requestId: string, key: string, data: string) {
        try {
            console.log('savePlayerData called with:', { requestId, key, data });

            const player = await window.yandexSDK.getPlayer();
            await player.setData({ [key]: data }, true);

            console.log(`Saving data for ${key}:`, data);

            // Send success response to Unity
            window.SendResponseToUnity(requestId, true, null, null);
        } catch (error: any) {
            console.error("Failed to save player data:", error.message);
            // Send error response to Unity
            window.SendResponseToUnity(requestId, false, null, error.message);
        }
    }

    static async loadPlayerData(requestId: string, key: string) {
        try {
            console.log('loadPlayerData called with:', { requestId, key });

            const player = await window.yandexSDK.getPlayer();
            const result = await player.getData([key]);

            if (result && result.hasOwnProperty(key)) {
                const data = result[key];
                console.log(`Loaded player data for key ${key}:`, data);

                // Send success response to Unity with data
                window.SendResponseToUnity(requestId, true, data, null);
            } else {
                console.warn(`No data found for key: ${key}`);
                // Send error response to Unity
                window.SendResponseToUnity(requestId, false, null, 'No data found for the given key');
            }
        } catch (error: any) {
            console.error("Failed to load player data:", error.message);
            // Send error response to Unity
            window.SendResponseToUnity(requestId, false, null, error.message);
        }
    }
}

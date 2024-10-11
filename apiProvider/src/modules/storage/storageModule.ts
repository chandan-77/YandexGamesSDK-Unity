export class StorageModule {
    private static queue: Array<() => Promise<void>> = [];
    private static isProcessing: boolean = false;

    private static async processQueue() {
        if (StorageModule.isProcessing || StorageModule.queue.length === 0) return;

        StorageModule.isProcessing = true;

        while (StorageModule.queue.length > 0) {
            const task = StorageModule.queue.shift();
            if (task) {
                try {
                    await task();
                } catch (error) {
                    console.error("Error executing task:", error);
                }
            }
        }

        StorageModule.isProcessing = false;
    }

    static savePlayerData(requestId: string, key: string, data: string) {
        StorageModule.queue.push(async () => {
            try {
                console.log('savePlayerData called with:', { requestId, key, data });

                const player = await window.yandexSDK.getPlayer();
                await player.setData({ [key]: data }, true);

                console.log(`Saving data for ${key}:`, data);

                window.SendResponseToUnity(requestId, true, null, null);
            } catch (error: any) {
                console.error("Failed to save player data:", error.message);
                window.SendResponseToUnity(requestId, false, null, error.message);
            }
        });

        StorageModule.processQueue();
    }

    static loadPlayerData(requestId: string, key: string) {
        StorageModule.queue.push(async () => {
            try {
                console.log('loadPlayerData called with:', { requestId, key });

                const player = await window.yandexSDK.getPlayer();
                const result = await player.getData([key]);

                if (result && result.hasOwnProperty(key)) {
                    const data = result[key];
                    console.log(`Loaded player data for key ${key}:`, data);

                    window.SendResponseToUnity(requestId, true, data, null);
                } else {
                    console.warn(`No data found for key: ${key}`);
                    window.SendResponseToUnity(requestId, false, null, 'No data found for the given key');
                }
            } catch (error: any) {
                console.error("Failed to load player data:", error.message);

                window.SendResponseToUnity(requestId, false, null, error.message);
            }
        });

        StorageModule.processQueue();
    }
}

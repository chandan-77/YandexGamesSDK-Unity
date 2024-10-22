import { YandexGames } from "YandexGamesSDK";
import { Utils } from "../system/utils";

export class YandexGamesSDK {
    private static isInitialized: boolean = false;
    private static unityInitResolve: (() => void) | null = null;
    private static yandexGamesSDKReadyResolve: (() => void) | null = null;

    private static unityInstancePromise: Promise<void> = new Promise<void>((resolve) => {
        YandexGamesSDK.unityInitResolve = resolve;
    });

    private static yandexGamesSDKReadyPromise: Promise<void> = new Promise<void>((resolve) => {
        YandexGamesSDK.yandexGamesSDKReadyResolve = resolve;
    });

    /**
     * Initializes the Yandex Games SDK and Unity instance.
     */
    public static async initialize(): Promise<void> {
        try {
            // Load Yandex SDK and then initialize Unity
            await YandexGamesSDK.loadYandexSDK();
        } catch (error: any) {
            console.error("Failed to initialize:", error.message);

            // Wait for unityInstance and YandexGamesSDK to be ready before sending error message
            await YandexGamesSDK.waitForUnityInstance();
            await YandexGamesSDK.waitForYandexGamesSDKReady();

            window.unityInstance.SendMessage('YandexGamesSDK', 'OnSDKInitialized', `false|${error.message}`);
        }
    }

    /**
     * Get the server time.
     * @param {string} requestId - The unique request ID.
     */
    public static async getServerTime(requestId: string): Promise<void> {
        try {
            const sdk: YandexGames.SDK = await window.yandexSDK;

            const serverTime = await sdk.serverTime();

            // Send success response with data
            window.SendResponseToUnity(requestId, true, String(serverTime), null);
            console.log("Server time fetched successfully and sent to Unity.");
        } catch (error: any) {
            console.error("Failed to get server time:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to get server time.');
        }
    }

    /**
     * Get the environment data.
     * @param {string} requestId - The unique request ID.
     */
    public static async getEnvironment(requestId: string): Promise<void> {
        try {
            const sdk: YandexGames.SDK = await window.yandexSDK;

            const env = sdk.environment;  

            // Serialize environment data to JSON
            const jsonData = JSON.stringify(env);

            // Send success response with data
            window.SendResponseToUnity(requestId, true, jsonData, null);
            console.log("Environment data fetched successfully and sent to Unity.");
        } catch (error: any) {
            console.error("Failed to get environment:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to get environment.');
        }
    }

    /**
     * Loads the Yandex SDK script and initializes it.
     */
    private static async loadYandexSDK(): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            const script = document.createElement('script');
            script.src = 'https://yandex.ru/games/sdk/v2'; // Yandex SDK URL
            //script.src = '/sdk.js'; // Yandex SDK URL
            script.onload = () => YandexGamesSDK.onYandexSDKLoaded().then(resolve).catch(reject);
            script.onerror = () => {
                console.error('Failed to load Yandex SDK script');
                YandexGamesSDK.initializeUnity().then(resolve).catch(reject);
            };
            document.head.appendChild(script);
        });
    }

    /**
     * Called when the Yandex SDK script is loaded.
     */
    private static async onYandexSDKLoaded(): Promise<void> {
        try {
            // Ensure YaGames is accessible
            const sdk = await (window as any).YaGames.init();
            (window as any).yandexSDK = sdk;
            console.log('Yandex SDK loaded and initialized successfully');

            await YandexGamesSDK.initializeUnity();

            // Wait for unityInstance and YandexGamesSDK to be ready before sending message
            await YandexGamesSDK.waitForUnityInstance();
            await YandexGamesSDK.waitForYandexGamesSDKReady();

            YandexGamesSDK.sendUnityMessage('OnYSDKLoaded', 'YSDK loaded successfully');

            YandexGamesSDK.isInitialized = true;

            // Notify Unity that the SDK and Unity have been initialized
            window.unityInstance.SendMessage('YandexGamesSDK', 'OnSDKInitialized', 'true');


        } catch (error: any) {
            console.error('Yandex SDK failed to initialize:', error.message);

            // Wait for unityInstance and YandexGamesSDK to be ready before sending error message
            await YandexGamesSDK.waitForUnityInstance();
            await YandexGamesSDK.waitForYandexGamesSDKReady();

            YandexGamesSDK.sendUnityMessage('OnYSDKLoadFailed', error.message);

            // Proceed to initialize Unity even if Yandex SDK fails
            await YandexGamesSDK.initializeUnity();

            // Notify Unity about the initialization failure
            window.unityInstance.SendMessage('YandexGamesSDK', 'OnSDKInitialized', `false|${error.message}`);
        }
    }

    /**
     * Initializes the Unity WebGL instance.
     */
    private static async initializeUnity(): Promise<void> {
        const buildUrl = 'Build';
        const loaderUrl = `${buildUrl}/{{{ LOADER_FILENAME }}}`;

        const config = {
            dataUrl: `${buildUrl}/{{{ DATA_FILENAME }}}`,
            frameworkUrl: `${buildUrl}/{{{ FRAMEWORK_FILENAME }}}`,
            codeUrl: `${buildUrl}/{{{ CODE_FILENAME }}}`,
            streamingAssetsUrl: 'StreamingAssets',
            companyName: '{{{ COMPANY_NAME }}}',
            productName: '{{{ PRODUCT_NAME }}}',
            productVersion: '{{{ PRODUCT_VERSION }}}',
        };

        const canvas = document.querySelector('#unity-canvas') as HTMLCanvasElement;

        return new Promise<void>((resolve, reject) => {
            YandexGamesSDK.loadScript(loaderUrl, () => {
                (window as any).createUnityInstance(canvas, config, YandexGamesSDK.updateLoadingProgress)
                    .then((unityInstance: any) => {
                        console.log('Unity WebGL instance created successfully');
                        window.unityInstance = unityInstance; // Store instance in the class
                        (window as any).unityInstance = unityInstance; // Store instance globally if needed

                        // Resolve any pending waiters
                        if (YandexGamesSDK.unityInitResolve) {
                            YandexGamesSDK.unityInitResolve();
                            YandexGamesSDK.unityInitResolve = null;
                        }

                        resolve();
                    })
                    .catch((error: any) => {
                        console.error('Failed to load Unity game:', error);
                        alert('Failed to load Unity game: ' + error);
                        reject(error);
                    });
            });
        });
    }

    /**
     * Loads an external script.
     * @param url The URL of the script to load.
     * @param onload The callback function when the script is loaded.
     */
    private static loadScript(url: string, onload: () => void): void {
        const script = document.createElement('script');
        script.src = url;
        script.onload = onload;
        script.onerror = () => {
            console.error('Failed to load script:', url);
            alert('Failed to load Unity loader script.');
        };
        document.body.appendChild(script);
    }

    /**
     * Updates the loading progress of Unity.
     * @param progress The loading progress (0 to 1).
     */
    private static updateLoadingProgress(progress: number): void {
        // Optional: Update a loading UI if necessary
        // console.log(`Loading Unity progress: ${progress * 100}%`);
    }

    /**
     * Sends a message to the Unity instance.
     * @param functionName The name of the Unity function to call.
     * @param message The message to send.
     */
    private static sendUnityMessage(functionName: string, message: string): void {
        if (unityInstance) {
            unityInstance.SendMessage('YandexGamesSDK', functionName, message);
        } else {
            console.warn('Unity instance is not initialized.');
        }
    }

    /**
     * Waits for the unityInstance to be available.
     */
    private static waitForUnityInstance(): Promise<void> {
        if (window.unityInstance) {
            return Promise.resolve();
        }
        return YandexGamesSDK.unityInstancePromise;
    }

    /**
     * Waits for the YandexGamesSDK GameObject to be ready.
     */
    private static waitForYandexGamesSDKReady(): Promise<void> {
        return YandexGamesSDK.yandexGamesSDKReadyPromise;
    }

    /**
     * Called by Unity when the YandexGamesSDK GameObject is ready.
     */
    public static OnYandexGamesSDKReady(): void {
        if (YandexGamesSDK.yandexGamesSDKReadyResolve) {
            YandexGamesSDK.yandexGamesSDKReadyResolve();
            YandexGamesSDK.yandexGamesSDKReadyResolve = null;
        }
    }

    /**
     * Checks if the SDK is initialized.
     * @returns True if initialized, otherwise false.
     */
    static checkForInitialization(): boolean {
        return YandexGamesSDK.isInitialized;
    }
}




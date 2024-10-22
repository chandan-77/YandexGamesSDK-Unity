import { YandexGames } from "YandexGamesSDK";

export class LeaderboardModule {
    /**
     * Submit a score to a leaderboard.
     * @param {string} requestId - The unique request ID.
     * @param {string} leaderboardName - The name of the leaderboard.
     * @param {number} score - The score to submit.
     * @param {string} [extraData] - Optional user description.
     */
    static async submitScore(requestId: string, leaderboardName: string, score: number, extraData = null) {
        try {
            const sdk = await window.yandexSDK;
            const leaderboards = await sdk.getLeaderboards();

            // Check if the method is available
            const isAvailable = await sdk.isAvailableMethod('leaderboards.setLeaderboardScore');
            if (!isAvailable) {
                throw new Error('setLeaderboardScore method is not available.');
            }

            await leaderboards.setLeaderboardScore(leaderboardName, score, extraData);
            // Send success response
            window.SendResponseToUnity(requestId, true, null, null);
            console.log("Score submitted successfully to Unity.");
        } catch (error: any) {
            // Enhanced error handling
            console.error("Error submitting score:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to submit score.');
        }
    }

    /**
     * Get leaderboard entries.
     * @param {string} requestId - The unique request ID.
     * @param {string} leaderboardName - The name of the leaderboard.
     * @param {number} quantityTop - Number of top entries to fetch.
     * @param {number} quantityAround - Number of entries around the player.
     * @param {boolean} includeUser - Whether to include the user's entry.
     */
    static async getLeaderboardEntries(requestId: string, leaderboardName: string, quantityTop = 5, quantityAround = 5, includeUser = true) {
        try {
            const sdk = await window.yandexSDK;
            const leaderboards = await sdk.getLeaderboards();

            // Check if the method is available
            const isAvailable = await sdk.isAvailableMethod('leaderboards.getLeaderboardEntries');
            if (!isAvailable) {
                throw new Error('getLeaderboardEntries method is not available.');
            }

            const data = await leaderboards.getLeaderboardEntries(leaderboardName, {
                includeUser: includeUser,
                quantityTop: quantityTop,
                quantityAround: quantityAround,
            });

            // Serialize entries to JSON
            const jsonData = JSON.stringify(data);
            // Send success response with data
            window.SendResponseToUnity(requestId, true, jsonData, null);
            console.log("Leaderboard entries fetched successfully and sent to Unity.");
        } catch (error: any) {
            console.error("Error fetching leaderboard entries:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to fetch leaderboard entries.');
        }
    }

    /**
     * Get the player's entry in a leaderboard.
     * @param {string} requestId - The unique request ID.
     * @param {string} leaderboardName - The name of the leaderboard.
     */
    static async getPlayerEntry(requestId: string, leaderboardName: string) {
        try {
            const sdk = await window.yandexSDK;
            const leaderboards = await sdk.getLeaderboards();

            // Check if the method is available
            const isAvailable = await sdk.isAvailableMethod('leaderboards.getLeaderboardPlayerEntry');
            if (!isAvailable) {
                throw new Error('getLeaderboardPlayerEntry method is not available.');
            }

            const entry = await leaderboards.getLeaderboardPlayerEntry(leaderboardName);
            // Serialize entry to JSON
            const jsonData = JSON.stringify(entry);
            // Send success response with data
            window.SendResponseToUnity(requestId, true, jsonData, null);
            console.log("Player entry fetched successfully and sent to Unity.");
        } catch (error: any) {
            console.error("Error fetching player entry:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to fetch player entry.');
        }
    }
}

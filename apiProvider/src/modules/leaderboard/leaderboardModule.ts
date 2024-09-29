import { YandexGames } from "YandexGamesSDK";
import { Utils } from "../../system/utils";

export class LeaderboardModule {
    static async submitScore(leaderboardName: string, score: number): Promise<void> {
        if (!Utils.isLocalhost()) {
            try {
                await window.yandexSDK.getLeaderboards().then(async (leaderboards: YandexGames.Leaderboards) => {
                    await leaderboards.setLeaderboardScore(leaderboardName, score);
                    unityInstance.SendMessage('YandexGamesSDK', 'OnSubmitScoreSuccess', '');
                });
            } catch (error: any) {
                unityInstance.SendMessage('YandexGamesSDK', 'OnSubmitScoreFailure', error.message);
            }
        } else {
            // Use mock data
            console.warn("Using mock data for submitScore");
            // Simulate success
            unityInstance.SendMessage('YandexGamesSDK', 'OnSubmitScoreSuccess', '');
        }
    }

    static async getLeaderboardEntries(
        leaderboardName: string,
        offset: number,
        limit: number
    ): Promise<void> {
        if (!Utils.isLocalhost()) {
            try {
                await window.yandexSDK.getLeaderboards().then(async (leaderboards: YandexGames.Leaderboards) => {
                    const data = await leaderboards.getLeaderboardEntries(leaderboardName, {
                        includeUser: true,
                        quantityTop: offset + limit,
                        quantityAround: 0,
                    });
                    const entries = data.entries.slice(offset, offset + limit);
                    const jsonData = JSON.stringify({ entries });
                    unityInstance.SendMessage('YandexGamesSDK', 'OnGetLeaderboardEntriesSuccess', jsonData);
                });
            } catch (error: any) {
                unityInstance.SendMessage('YandexGamesSDK', 'OnGetLeaderboardEntriesFailure', error.message);
            }
        } else {
            // Use mock data
            console.warn("Using mock data for getLeaderboardEntries");
            const mockEntries = [
                {
                    score: 1000,
                    rank: 1,
                    player: {
                        publicName: "Player1",
                        uniqueID: "1",
                        getAvatarSrc: (size: "small" | "medium" | "large") => "",
                        getAvatarSrcSet: (size: "small" | "medium" | "large") => "",
                        lang: "en",
                        scopePermissions: { avatar: "granted", public_name: "granted" },
                    },
                    formattedScore: "1,000",
                },
                {
                    score: 900,
                    rank: 2,
                    player: {
                        publicName: "Player2",
                        uniqueID: "2",
                        getAvatarSrc: (size: "small" | "medium" | "large") => "",
                        getAvatarSrcSet: (size: "small" | "medium" | "large") => "",
                        lang: "en",
                        scopePermissions: { avatar: "granted", public_name: "granted" },
                    },
                    formattedScore: "900",
                },
                // Add more mock entries as needed
            ];
            const jsonData = JSON.stringify({ entries: mockEntries });
            unityInstance.SendMessage('YandexGamesSDK', 'OnGetLeaderboardEntriesSuccess', jsonData);
        }
    }

    static async getPlayerEntry(leaderboardName: string): Promise<void> {
        if (!Utils.isLocalhost()) {
            try {
                await window.yandexSDK.getLeaderboards().then(async (leaderboards: YandexGames.Leaderboards) => {
                    const entry = await leaderboards.getLeaderboardPlayerEntry(leaderboardName);
                    const jsonData = JSON.stringify(entry);
                    unityInstance.SendMessage('YandexGamesSDK', 'OnGetPlayerEntrySuccess', jsonData);
                });
            } catch (error: any) {
                unityInstance.SendMessage('YandexGamesSDK', 'OnGetPlayerEntryFailure', error.message);
            }
        } else {
            // Use mock data
            console.warn("Using mock data for getPlayerEntry");
            const mockEntry = {
                score: 800,
                rank: 3,
                player: {
                    publicName: "MockPlayer",
                    uniqueID: "mock_user",
                    getAvatarSrc: (size: "small" | "medium" | "large") => "",
                    getAvatarSrcSet: (size: "small" | "medium" | "large") => "",
                    lang: "en",
                    scopePermissions: { avatar: "granted", public_name: "granted" },
                },
                formattedScore: "800",
            };
            const jsonData = JSON.stringify(mockEntry);
            unityInstance.SendMessage('YandexGamesSDK', 'OnGetPlayerEntrySuccess', jsonData);
        }
    }
}

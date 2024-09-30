import { YandexGames } from "YandexGamesSDK";

export class LeaderboardModule {
    static async submitScore(leaderboardName: string, score: number): Promise<void> {
        try {
            await window.yandexSDK.getLeaderboards().then(async (leaderboards: YandexGames.Leaderboards) => {
                await leaderboards.setLeaderboardScore(leaderboardName, score);
                unityInstance.SendMessage('YandexGamesSDK', 'OnSubmitScoreSuccess', '');
            });
        } catch (error: any) {
            unityInstance.SendMessage('YandexGamesSDK', 'OnSubmitScoreFailure', error.message);
        }

    }

    static async getLeaderboardEntries(
        leaderboardName: string,
        offset: number,
        limit: number
    ): Promise<void> {
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

    }

    static async getPlayerEntry(leaderboardName: string): Promise<void> {
        try {
            await window.yandexSDK.getLeaderboards().then(async (leaderboards: YandexGames.Leaderboards) => {
                const entry = await leaderboards.getLeaderboardPlayerEntry(leaderboardName);
                const jsonData = JSON.stringify(entry);
                unityInstance.SendMessage('YandexGamesSDK', 'OnGetPlayerEntrySuccess', jsonData);
            });
        } catch (error: any) {
            unityInstance.SendMessage('YandexGamesSDK', 'OnGetPlayerEntryFailure', error.message);
        }

    }
}

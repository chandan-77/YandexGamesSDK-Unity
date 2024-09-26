mergeInto(LibraryManager.library, {
    SubmitScore: function (leaderboardNamePtr, score) {
        var leaderboardName = UTF8ToString(leaderboardNamePtr);

        if (typeof ysdk !== 'undefined') {
            ysdk.leaderboards.setLeaderboardScore(leaderboardName, score).then(function() {
                SendMessage('YandexGamesSDK', 'OnSubmitScoreSuccess');
            }).catch(function(error) {
                SendMessage('YandexGamesSDK', 'OnSubmitScoreFailure', error.message);
            });
        } else {
            SendMessage('YandexGamesSDK', 'OnSubmitScoreFailure', 'Yandex SDK not initialized.');
        }
    },

    GetLeaderboardEntries: function (leaderboardNamePtr, offset, limit) {
        var leaderboardName = UTF8ToString(leaderboardNamePtr);

        if (typeof ysdk !== 'undefined') {
            ysdk.leaderboards.getLeaderboardEntries(leaderboardName, {includeUser: true, quantityAround: 0, quantityTop: limit, offset: offset}).then(function(result) {
                var entries = result.entries.map(function(entry) {
                    return {
                        rank: entry.rank,
                        playerId: entry.player.uniqueID,
                        name: entry.player.name,
                        score: entry.score,
                        avatarUrl: entry.player.getAvatarSrc(),
                    };
                });

                var jsonData = JSON.stringify({entries: entries});
                SendMessage('YandexGamesSDK', 'OnGetLeaderboardEntriesSuccess', jsonData);
            }).catch(function(error) {
                SendMessage('YandexGamesSDK', 'OnGetLeaderboardEntriesFailure', error.message);
            });
        } else {
            SendMessage('YandexGamesSDK', 'OnGetLeaderboardEntriesFailure', 'Yandex SDK not initialized.');
        }
    },

    GetPlayerEntry: function (leaderboardNamePtr) {
        var leaderboardName = UTF8ToString(leaderboardNamePtr);

        if (typeof ysdk !== 'undefined') {
            ysdk.leaderboards.getLeaderboardPlayerEntry(leaderboardName).then(function(entry) {
                var playerEntry = {
                    rank: entry.rank,
                    playerId: entry.player.uniqueID,
                    name: entry.player.name,
                    score: entry.score,
                    avatarUrl: entry.player.getAvatarSrc(),
                };

                var jsonData = JSON.stringify(playerEntry);
                SendMessage('YandexGamesSDK', 'OnGetPlayerEntrySuccess', jsonData);
            }).catch(function(error) {
                SendMessage('YandexGamesSDK', 'OnGetPlayerEntryFailure', error.message);
            });
        } else {
            SendMessage('YandexGamesSDK', 'OnGetPlayerEntryFailure', 'Yandex SDK not initialized.');
        }
    }
});

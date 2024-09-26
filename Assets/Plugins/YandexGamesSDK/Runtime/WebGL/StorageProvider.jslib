mergeInto(LibraryManager.library, {
    SavePlayerData: function (gameObjectNamePtr, keyPtr, dataPtr) {
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var key = UTF8ToString(keyPtr);
        var data = UTF8ToString(dataPtr);

        if (typeof ysdk !== 'undefined') {
            ysdk.storage.set(key, data)
                .then(function () {
                    SendMessage(gameObjectName, 'OnPlayerDataSaved', 'true');
                })
                .catch(function (error) {
                    SendMessage(gameObjectName, 'OnPlayerDataSaved', 'false|' + error.message);
                });
        } else {
            SendMessage(gameObjectName, 'OnPlayerDataSaved', 'false|YSDK not initialized');
        }
    },

    LoadPlayerData: function (gameObjectNamePtr, keyPtr) {
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var key = UTF8ToString(keyPtr);

        if (typeof ysdk !== 'undefined') {
            ysdk.storage.get(key)
                .then(function (data) {
                    if (data) {
                        SendMessage(gameObjectName, 'OnPlayerDataLoaded', JSON.stringify(data));
                    } else {
                        SendMessage(gameObjectName, 'OnPlayerDataLoaded', 'false|No data found');
                    }
                })
                .catch(function (error) {
                    SendMessage(gameObjectName, 'OnPlayerDataLoaded', 'false|' + error.message);
                });
        } else {
            SendMessage(gameObjectName, 'OnPlayerDataLoaded', 'false|YSDK not initialized');
        }
    }
});

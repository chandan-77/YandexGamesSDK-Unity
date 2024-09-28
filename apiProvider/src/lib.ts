mergeInto(LibraryManager.library, {
    AuthenticateUser: function () {
        if (typeof window.AuthenticateUser === 'function') {
            window.AuthenticateUser();
        } else {
            console.error('AuthenticateUser is not defined on window.');
        }
    },
    SavePlayerData: function (keyPtr:string, dataPtr:string) {
        if (typeof window.SavePlayerData === 'function') {
            var key = UTF8ToString(keyPtr);
            var data = UTF8ToString(dataPtr);
    
            window.SavePlayerData(key,data);
        } else {
            console.error('SavePlayerData is not defined on window.');
        }
    },
    LoadPlayerData: function (keyPtr:string) {
        if (typeof window.LoadPlayerData === 'function') {
            var key = UTF8ToString(keyPtr);

            window.LoadPlayerData(key);
        } else {
            console.error('SavePlayerData is not defined on window.');
        }
    },
});
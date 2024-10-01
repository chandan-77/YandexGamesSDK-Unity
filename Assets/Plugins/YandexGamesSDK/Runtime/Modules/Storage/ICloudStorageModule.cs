using System;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Storage
{
    public interface ICloudStorageModule
    {
         void SaveData(string key, object data, Action<bool, string> callback = null);
         void LoadData(string key, Action<bool, string> callback = null);
    }
}
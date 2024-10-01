using System;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Types
{

    [Serializable]
    public record EnvironmentData
    {
        public AppData app { get; init; }
        public BrowserData browser { get; init; }
        public Data data { get; init; }
        public I18n i18n { get; init; }
        public bool isTelegram { get; init; }
        public string domain { get; init; }
    }

    [Serializable]
    public record AppData
    {
        public string id { get; init; }
    }

    [Serializable]
    public record BrowserData
    {
        public string lang { get; init; }
    }

    [Serializable]
    public record Data
    {
        public string baseUrl { get; init; }
        public string secondDomain { get; init; }
    }

    [Serializable]
    public record I18n
    {
        public string lang { get; init; }
        public string tld { get; init; }
    }

    [Serializable]
    public class Root
    {
        public EnvironmentData env { get; set; }
    }
}
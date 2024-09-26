using System;

namespace Plugins.YandexGamesSDK.Runtime.Modules.Authentication
{
    [Serializable]
    public class UserProfile
    {
        public string id;
        public string name;
        public string avatarUrlSmall;
        public string avatarUrlMedium;
        public string avatarUrlLarge;
    }
}
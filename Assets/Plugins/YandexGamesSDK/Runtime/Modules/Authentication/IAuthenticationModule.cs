using System;

namespace Plugins.YandexGamesSDK.Runtime.Modules.Authentication
{
    public interface IAuthenticationModule
    {
        void AuthenticateUser(Action<bool, string> callback);
        UserProfile GetUserProfile();
    }
}
using System;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Types;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Authentication
{
    public interface IAuthenticationModule
    {
        void AuthenticateUser(Action<bool, string> callback);
        UserProfile GetUserProfile();
    }
}
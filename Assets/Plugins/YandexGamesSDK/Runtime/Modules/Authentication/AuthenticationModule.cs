using System;
using System.Runtime.InteropServices;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Abstractions;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Types;
using UnityEngine;
using IAuthenticationModule = PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Authentication.IAuthenticationModule;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Modules.Authentication
{
    public class AuthenticationModule : YGModuleBase, IAuthenticationModule
    {
        public UserProfile CurrentUser { get; private set; }
        private Action<bool, string> authenticationCallback;

        [DllImport("__Internal")]
        private static extern void AuthenticateUser(bool requireSignin);

        public override void Initialize()
        {
        }

        public void AuthenticateUser(Action<bool, string> callback, bool requireSignin = false)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        authenticationCallback = callback;
        AuthenticateUser(requireSignin);
#else
            Debug.Log("Authentication is only available in WebGL builds.");
            callback(false, "Authentication is only available in WebGL builds.");
#endif
        }

        public void OnAuthenticationSuccess(string jsonProfile)
        {
            CurrentUser = JsonUtility.FromJson<UserProfile>(jsonProfile);
            authenticationCallback?.Invoke(true, null);
        }

        public void OnAuthenticationFailure(string error)
        {
            Debug.LogError($"Authentication failed: {error}");
            authenticationCallback?.Invoke(false, error);
        }

        public UserProfile GetUserProfile()
        {
            return CurrentUser;
        }
    }
}
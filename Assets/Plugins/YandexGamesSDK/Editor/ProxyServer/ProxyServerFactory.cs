using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Dashboard;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Editor.ProxyServer
{
    public static class ProxyServerFactory
    {
        public static NodeProxyServer ProxyServer => _proxyServer ??= new NodeProxyServer(YandexGamesSDKConfig.Instance);

        private static NodeProxyServer _proxyServer;
    }
}
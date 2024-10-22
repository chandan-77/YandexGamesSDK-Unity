using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlayablesStudio.Plugins.YandexGamesSDK.Editor.ProxyServer.Abstractions;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Dashboard;
using UnityEngine;
using UnityEditor; // Add this namespace

namespace PlayablesStudio.Plugins.YandexGamesSDK.Editor.ProxyServer
{
    public class InternalProxyServer : IProxyServer
    {
        private YandexGamesSDKConfig _config;
        private CancellationTokenSource cancellationTokenSource;
        private Task logRetrievalTask;

        public InternalProxyServer(YandexGamesSDKConfig config)
        {
            _config = config;
        }

        private StringBuilder logBuilder = new StringBuilder();
        public string Logs => logBuilder.ToString(); // Property to retrieve the logs

        public event Action<string> OnError;
        public event Action OnLogUpdate;
        public bool IsRunning { get; private set; }

        // DllImport to call the Go-based server functions
        [DllImport("dev_proxy", EntryPoint = "StartServer")]
        public static extern void StartServer(string host, string path, string appID, bool csp, int port, string tld, bool logReq);

        [DllImport("dev_proxy", EntryPoint = "StopServer")]
        public static extern void StopServer();

        [DllImport("dev_proxy", EntryPoint = "GetLogs")]
        public static extern IntPtr GetLogs();

        private Task serverTask;

        // Method to start the proxy server with logging and error handling
        public void StartProxyServer()
        {
            if (IsRunning)
            {
                logBuilder.AppendLine("Proxy server is already running.");
                OnLogUpdate?.Invoke();
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                serverTask = Task.Run(() =>
                {
                    StartServer("", _config.developmentSettings.buildPath,
                        _config.appID, true, _config.developmentSettings.serverPort, "ru", true);
                });

                IsRunning = true;

                // Start log retrieval task
                logRetrievalTask = Task.Run(async () =>
                {
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        RetrieveLogs();
                        await Task.Delay(1000, cancellationTokenSource.Token); // Adjust the delay as needed
                    }
                }, cancellationTokenSource.Token);

                logBuilder.AppendLine("Proxy server started successfully.");

                string url = $"https://yandex.ru/games/app/{_config.appID}/?draft=true&game_url=https://localhost:{_config.developmentSettings.serverPort}";
                logBuilder.AppendLine($"You can open your game with {url}");

                OnLogUpdate?.Invoke();

                // Open the URL in the default browser
                OpenUrlInBrowser(url);

                Debug.Log("Proxy server started on a background thread.");
            }
            catch (Exception ex)
            {
                HandleError($"Failed to start the proxy server: {ex.Message}");
            }
        }

        private void OpenUrlInBrowser(string url)
        {
            // This method uses UnityEditor API, so ensure that your code is inside an Editor folder
#if UNITY_EDITOR
            UnityEditor.EditorUtility.OpenWithDefaultApp(url);
#else
            // Fallback for runtime (if needed)
            Application.OpenURL(url);
#endif
        }

        public void RetrieveLogs()
        {
            IntPtr ptr = GetLogs();
            if (ptr != IntPtr.Zero)
            {
                string logs = Marshal.PtrToStringAnsi(ptr);
                Marshal.FreeHGlobal(ptr);

                if (!string.IsNullOrEmpty(logs))
                {
                    logBuilder.Append(logs);
                    OnLogUpdate?.Invoke();
                }
            }
        }

        // Method to stop the proxy server with proper cleanup and logging
        public void StopProxyServer()
        {
            if (!IsRunning)
            {
                logBuilder.AppendLine("Proxy server is not running.");
                OnLogUpdate?.Invoke();
                return;
            }

            try
            {
                cancellationTokenSource.Cancel();
                StopServer();
                IsRunning = false;

                RetrieveLogs(); // Retrieve any remaining logs

                logBuilder.AppendLine("Proxy server stopped successfully.");
                OnLogUpdate?.Invoke();

                Debug.Log("Proxy server stopped.");
            }
            catch (Exception ex)
            {
                HandleError($"Failed to stop the proxy server: {ex.Message}");
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        // Method to handle cleanup operations
        public void Cleanup()
        {
            if (IsRunning)
            {
                StopProxyServer();
            }

            logBuilder.Clear();
            OnLogUpdate?.Invoke();
            Debug.Log("Proxy server logs cleared and cleaned up.");
        }

        // Method to clear the log
        public void ClearLogs()
        {
            logBuilder.Clear();
            OnLogUpdate?.Invoke();
            Debug.Log("Logs have been cleared.");
        }

        // Private method to handle error logging and event triggering
        private void HandleError(string errorMessage)
        {
            logBuilder.AppendLine($"ERROR: {errorMessage}");
            OnLogUpdate?.Invoke();
            OnError?.Invoke(errorMessage);

            Debug.LogError(errorMessage);
        }
    }
}

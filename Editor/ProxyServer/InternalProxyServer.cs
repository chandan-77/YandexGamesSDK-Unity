using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlayablesStudio.Plugins.YandexGamesSDK.Editor.ProxyServer.Abstractions;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Dashboard;
using UnityEngine;
using UnityEditor;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Editor.ProxyServer
{
    public class InternalProxyServer : IProxyServer
    {
        private YandexGamesSDKConfig _config;
        private CancellationTokenSource cancellationTokenSource;
        private Task logRetrievalTask;

        [InitializeOnLoadMethod]
        private static void RegisterDomainReloadCallback()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            EditorApplication.quitting += OnEditorQuitting;
        }

        private static void OnBeforeAssemblyReload()
        {
            if (Instance != null && Instance.IsRunning)
            {
                Instance.StopProxyServer();
            }
        }

        private static void OnEditorQuitting()
        {
            if (Instance != null && Instance.IsRunning)
            {
                Instance.StopProxyServer();
            }
        }

        private static InternalProxyServer Instance;

        public InternalProxyServer(YandexGamesSDKConfig config)
        {
            _config = config;
            Instance = this;
        }

        private StringBuilder logBuilder = new StringBuilder();
        public string Logs => logBuilder.ToString(); // Property to retrieve the logs

        public event Action<string> OnError;
        public event Action OnLogUpdate;
        public bool IsRunning { get; private set; }

        [DllImport("libdev_proxy")]
        public static extern void StartServer(string path, bool csp, int port, bool logReq);

        [DllImport("libdev_proxy")]
        public static extern void StopServer();

        [DllImport("libdev_proxy")]
        public static extern IntPtr GetLogs();

        private Task serverTask;

        // Method to start the server with logging and error handling
        public void StartProxyServer()
        {
            if (IsRunning)
            {
                logBuilder.AppendLine("Server is already running.");
                OnLogUpdate?.Invoke();
                return;
            }

            try
            {
                cancellationTokenSource = new CancellationTokenSource();

                serverTask = Task.Run(() =>
                {
                    try
                    {
                        StartServer(_config.developmentSettings.buildPath, true, _config.developmentSettings.serverPort, true);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Server task error: {e.Message}");
                        HandleError(e.Message);
                    }
                }, cancellationTokenSource.Token);

                IsRunning = true;

                // Start log retrieval task with cancellation support
                logRetrievalTask = Task.Run(async () =>
                {
                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        try
                        {
                            RetrieveLogs();
                            await Task.Delay(1000, cancellationTokenSource.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Log retrieval error: {e.Message}");
                            break;
                        }
                    }
                }, cancellationTokenSource.Token);

                logBuilder.AppendLine("Server started successfully.");

                string encodedGameUrl = Uri.EscapeDataString($"https://localhost:{_config.developmentSettings.serverPort}/");
                string url = $"https://yandex.ru/games/app/{_config.appID}?draft=true&game_url={encodedGameUrl}";
                logBuilder.AppendLine($"You can open your game with {url}");

                OnLogUpdate?.Invoke();

                // Open the URL in the default browser
                OpenUrlInBrowser(url);

                Debug.Log("Server started on a background thread.");
            }
            catch (Exception ex)
            {
                HandleError($"Failed to start the server: {ex.Message}");
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

        // Method to stop the server with proper cleanup and logging
        public void StopProxyServer()
        {
            if (!IsRunning)
            {
                return;
            }

            try
            {
                // Cancel ongoing tasks
                if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
                {
                    cancellationTokenSource.Cancel();
                }

                // Stop the server
                StopServer();
                IsRunning = false;

                // Wait for tasks to complete with a timeout
                if (serverTask != null)
                {
                    if (!serverTask.Wait(TimeSpan.FromSeconds(2)))
                    {
                        Debug.LogWarning("Server task did not complete in time");
                    }
                }

                if (logRetrievalTask != null)
                {
                    if (!logRetrievalTask.Wait(TimeSpan.FromSeconds(2)))
                    {
                        Debug.LogWarning("Log retrieval task did not complete in time");
                    }
                }

                // Final cleanup
                RetrieveLogs();
                logBuilder.AppendLine("Server stopped successfully.");
                OnLogUpdate?.Invoke();
            }
            catch (Exception ex)
            {
                HandleError($"Failed to stop the server: {ex.Message}");
            }
            finally
            {
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
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
            Debug.Log("Server logs cleared and cleaned up.");
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

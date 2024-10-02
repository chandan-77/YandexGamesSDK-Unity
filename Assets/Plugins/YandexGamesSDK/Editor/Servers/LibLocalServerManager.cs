using System;
using System.Runtime.InteropServices;
using System.Text;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Dashboard;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Editor.Servers
{
    public class LibLocalServerManager
    {
        private static StringBuilder logBuilder = new StringBuilder();
        public static string Logs => logBuilder.ToString();
        public static bool IsRunning { get; private set; } = false;

        // Event to notify log updates
        public static event Action OnLogUpdate;

        // Import the Go functions
        [DllImport("proxy", CallingConvention = CallingConvention.Cdecl)]
        private static extern int StartServer(string host, string path, string appID, int port, string tld, int csp, int logReq);

        [DllImport("proxy", CallingConvention = CallingConvention.Cdecl)]
        private static extern int StopServer();

        /// <summary>
        /// Start the local server using the Go shared library
        /// </summary>
        public static void StartLocalServer(YandexGamesSDKConfig config)
        {
            StartLocalServer(config.developmentSettings.buildPath, config.developmentSettings.serverPort);
        }

        public static void StartLocalServer(string buildPath, int port)
        {
            var config = YandexGamesSDKConfig.Instance;

            if (IsRunning)
            {
                UnityEngine.Debug.LogWarning("Server is already running.");
                return;
            }

            // Prepare the required parameters
            string host = ""; // Leave blank if starting with path
            string path = buildPath;
            string appID = config.appID;
            string tld = "ru"; // Yandex default TLD, can be made configurable
            int cspFlag = config.developmentSettings.includeCSP ? 1 : 0;
            int logReqFlag = config.developmentSettings.enableLogging ? 1 : 0;

            // Start the server
            try
            {
                int result = StartServer(host, path, appID, port, tld, cspFlag, logReqFlag);
                if (result == 0)
                {
                    IsRunning = true;
                    UnityEngine.Debug.Log("Local server started successfully.");
                }
                else
                {
                    UnityEngine.Debug.LogError($"Failed to start the server. Error code: {result}");
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Failed to start local server: " + ex.Message);
            }
        }

        /// <summary>
        /// Stops the local server if it's running.
        /// </summary>
        public static void StopLocalServer()
        {
            if (!IsRunning)
            {
                UnityEngine.Debug.LogWarning("Server is not running.");
                return;
            }

            try
            {
                int result = StopServer();
                if (result == 0)
                {
                    IsRunning = false;
                    UnityEngine.Debug.Log("Local server stopped successfully.");
                }
                else
                {
                    UnityEngine.Debug.LogError("Failed to stop the server.");
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Exception while stopping server: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears the current logs.
        /// </summary>
        public static void ClearLogs()
        {
            logBuilder.Clear();
            OnLogUpdate?.Invoke();
            UnityEngine.Debug.Log("Logs have been cleared.");
        }

        /// <summary>
        /// Finds an available port on the local machine.
        /// </summary>
        public static int FindAvailablePort()
        {
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            int port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        /// <summary>
        /// Cleans up the server process when the application quits.
        /// </summary>
        public static void Cleanup()
        {
            if (IsRunning)
            {
                StopLocalServer();
            }
        }
    }
}

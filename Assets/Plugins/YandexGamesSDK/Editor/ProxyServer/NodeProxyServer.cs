using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Dashboard;
using UnityEngine;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Editor.ProxyServer
{
    public class NodeProxyServer
    {
        private Process serverProcess;
        private StringBuilder logBuilder = new StringBuilder();
        public string Logs => logBuilder.ToString();
        public event Action<string> OnError;
        public bool IsRunning => serverProcess != null && !serverProcess.HasExited;

        public event Action OnLogUpdate;

        private YandexGamesSDKConfig _config;
        public NodeProxyServer(YandexGamesSDKConfig config)
        {
            _config = config;
        }

        public void StartServer()
        {
            // Terminate the previous server process if it's still running
            if (serverProcess != null && !serverProcess.HasExited)
            {
                serverProcess.Kill();
                serverProcess.Dispose();
                serverProcess = null;

                UnityEngine.Debug.Log("Previous server process terminated.");
            }

            string nodeExecutable = FindNodePath();
            if (string.IsNullOrEmpty(nodeExecutable))
            {
                var err = "Unable to find 'node'. Please ensure that Node.js is installed and available in your system PATH.";
             
                UnityEngine.Debug.LogError(err);
                logBuilder.AppendLine(err);

                OnLogUpdate?.Invoke();
                
                return;
            }

            string npxPath = FindNpxPath();

            if (string.IsNullOrEmpty(npxPath))
            {
                var err =  "Unable to find 'npx'. Please ensure that Node.js and npm are installed and 'npx' is available in your system PATH.";

                UnityEngine.Debug.LogError(err);
                logBuilder.AppendLine(err);

                OnLogUpdate?.Invoke();
                
                return;
            }

            string arguments =
                $"@yandex-games/sdk-dev-proxy -p \"{_config.developmentSettings.buildPath}\" --app-id={_config.appID} --port={_config.developmentSettings.serverPort} -l";

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = nodeExecutable,
                Arguments = $"\"{npxPath}\" {arguments}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = _config.developmentSettings.buildPath,
            };

            // Log the PATH for debugging purposes
            UnityEngine.Debug.Log("Process PATH: " + startInfo.EnvironmentVariables["PATH"]);

            try
            {
                serverProcess = new Process();
                serverProcess.StartInfo = startInfo;

                // Handle standard output
                serverProcess.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        logBuilder.AppendLine(args.Data);
                        OnLogUpdate?.Invoke();
                        UnityEngine.Debug.Log(args.Data);
                    }
                };

                // Handle standard error
                serverProcess.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        logBuilder.AppendLine($"ERROR: {args.Data}");
                        OnLogUpdate?.Invoke();
                        UnityEngine.Debug.LogError("Process Error: " + args.Data);
                    }
                };

                serverProcess.Exited += (sender, args) =>
                {
                    logBuilder.AppendLine("Server process exited.");
                    OnLogUpdate?.Invoke();
                    serverProcess.Dispose();
                    serverProcess = null;
                    UnityEngine.Debug.Log("Server process has exited.");
                };

                bool started = serverProcess.Start();
                if (started)
                {
                    serverProcess.BeginOutputReadLine();
                    serverProcess.BeginErrorReadLine();
                    UnityEngine.Debug.Log("Local server started successfully.");
                }
                else
                {
                    UnityEngine.Debug.LogError("Failed to start the server process.");
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Failed to start local server: " + ex.Message);
            }
        }

        /// <summary>
        /// Finds the path to 'node' dynamically by checking the system PATH and common install locations.
        /// </summary>
        private string FindNodePath()
        {
            string nodeExecutableName = "node";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                nodeExecutableName = "node.exe"; // On Windows, 'node' is 'node.exe'
            }

            // First, try to find 'node' in the system PATH
            string pathEnv = Environment.GetEnvironmentVariable("PATH");
            string[] paths = pathEnv.Split(Path.PathSeparator);

            foreach (string path in paths)
            {
                string fullPath = Path.Combine(path, nodeExecutableName);
                if (File.Exists(fullPath))
                {
                    UnityEngine.Debug.Log("Found node at: " + fullPath);
                    return fullPath;
                }
            }

            // If not found, check common installation directories
            string[] commonPaths;
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                commonPaths = new string[]
                {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "nodejs"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "nodejs"),
                };
            }
            else // macOS and Linux
            {
                commonPaths = new string[]
                {
                    "/usr/local/bin",
                    "/usr/bin",
                    "/opt/homebrew/bin",
                };
            }

            foreach (string dir in commonPaths)
            {
                string fullPath = Path.Combine(dir, nodeExecutableName);
                if (File.Exists(fullPath))
                {
                    UnityEngine.Debug.Log("Found node at: " + fullPath);
                    return fullPath;
                }
            }

            // Not found
            return null;
        }

        /// <summary>
        /// Finds the path to 'npx' dynamically by checking the system PATH and common install locations.
        /// </summary>
        private string FindNpxPath()
        {
            string npxExecutableName = "npx";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                npxExecutableName = "npx.cmd"; // On Windows, 'npx' is 'npx.cmd'
            }

            // First, try to find 'npx' in the system PATH
            string pathEnv = Environment.GetEnvironmentVariable("PATH");
            string[] paths = pathEnv.Split(Path.PathSeparator);

            foreach (string path in paths)
            {
                string fullPath = Path.Combine(path, npxExecutableName);
                if (File.Exists(fullPath))
                {
                    UnityEngine.Debug.Log("Found npx at: " + fullPath);
                    return fullPath;
                }
            }

            // If not found, check common installation directories
            string[] commonPaths;
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                commonPaths = new string[]
                {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "nodejs"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "nodejs"),
                };
            }
            else // MacOS and Linux
            {
                commonPaths = new string[]
                {
                    "/usr/local/bin",
                    "/usr/bin",
                    "/opt/homebrew/bin",
                };
            }

            foreach (string dir in commonPaths)
            {
                string fullPath = Path.Combine(dir, npxExecutableName);
                if (File.Exists(fullPath))
                {
                    UnityEngine.Debug.Log("Found npx at: " + fullPath);
                    return fullPath;
                }
            }

            // Not found
            return null;
        }

        /// <summary>
        /// Stops the local server if it's running.
        /// </summary>
        public void StopServer()
        {
            if (IsRunning)
            {
                try
                {
                    serverProcess.CloseMainWindow();
                    serverProcess.WaitForExit(5000); // Wait up to 5 seconds

                    if (!serverProcess.HasExited)
                    {
                        serverProcess.Kill();
                        serverProcess.WaitForExit();
                    }

                    UnityEngine.Debug.Log("Local server stopped successfully.");
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Exception while stopping server: {ex.Message}");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("Server is not running.");
            }
        }

        /// <summary>
        /// Clears the current logs.
        /// </summary>
        public void ClearLogs()
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
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        /// <summary>
        /// Cleans up the server process when the application quits.
        /// </summary>
        public  void Cleanup()
        {
            if (IsRunning)
            {
                StopServer();
            }
        }
    }
}
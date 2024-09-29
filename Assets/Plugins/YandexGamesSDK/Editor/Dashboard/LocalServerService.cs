using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Plugins.YandexGamesSDK.Runtime.Dashboard;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Plugins.YandexGamesSDK.Editor.Dashboard
{
    public class LocalServerManager
    {
        private Process serverProcess;
        private StringBuilder logBuilder = new StringBuilder();
        public string Logs => logBuilder.ToString();
        public bool IsRunning => serverProcess != null && !serverProcess.HasExited;

        // Event to notify log updates
        public event Action OnLogUpdate;

        public async void StartLocalServer(string buildPath, string port)
        {
            var config = YandexGamesSDKConfig.Instance;

            // Terminate the previous server process if it's still running
            if (serverProcess != null && !serverProcess.HasExited)
            {
                serverProcess.Kill();
                serverProcess.Dispose();
                serverProcess = null;

                UnityEngine.Debug.Log("Previous server process terminated.");
            }

            string npxPath = "/opt/homebrew/bin/npx"; // Full path to npx
            string nodeDirectory = "/opt/homebrew/bin"; // Directory containing node, npm, npx

            string arguments = $"@yandex-games/sdk-dev-proxy -p \"{buildPath}\" --app-id={config.appID} --port={port} -l";

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = npxPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false,
                WorkingDirectory = buildPath,
            };

            // Update PATH
            string currentPath = startInfo.EnvironmentVariables["PATH"];
            startInfo.EnvironmentVariables["PATH"] = $"{nodeDirectory}:{currentPath}";

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

        public static int FindAvailablePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public void Cleanup()
        {
            if (IsRunning)
            {
                StopServer();
            }
        }
    }
}
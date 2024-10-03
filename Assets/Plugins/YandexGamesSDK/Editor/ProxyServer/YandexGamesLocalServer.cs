using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Dashboard;
using Debug = UnityEngine.Debug;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Editor.ProxyServer
{
    public class YandexGamesLocalServer
    {
        private HttpListener _httpListener;
        private HttpClient _httpClient;
        private bool _isRunning = false;
        private string csp = string.Empty;
        private DateTime lastCspUpdate = DateTime.MinValue;
        private static readonly TimeSpan CspTimeout = TimeSpan.FromMinutes(1);

        private static StringBuilder logBuilder = new StringBuilder();
        public static string Logs => logBuilder.ToString();
        public bool IsRunning => _isRunning;
        public static event Action OnLogUpdate;

        public int Port = 8080;
        public string YandexAppId = "YOUR_APP_ID";
        public bool EnableCsp = true;
        public bool EnableLogging = true;
        public string HostToProxy;
        public string Tld;
        private YandexGamesSDKConfig _config;

        public YandexGamesLocalServer(YandexGamesSDKConfig config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        public void StartServer()
        {
            if (_isRunning)
            {
                Log("Server is already running.");
                return;
            }

            try
            {
                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add($"http://localhost:{_config.developmentSettings.serverPort}/");
                _httpListener.Start();
                _isRunning = true;

                Log($"Server is running on http://localhost:{_config.developmentSettings.serverPort}/");
                _httpListener.BeginGetContext(HandleRequest, null);

                // Construct the Yandex Games URL similar to Node.js logic
                string yandexUrl =
                    $"https://yandex.{(string.IsNullOrEmpty(Tld) ? "ru" : Tld)}/games/app/{_config.appID}/?draft=true&game_url=http://localhost:{_config.developmentSettings.serverPort}";

                Log($"You can open your game with: {yandexUrl}");

                // Open the Yandex Games URL in the default web browser
                OpenUrlInBrowser(yandexUrl);
            }
            catch (Exception e)
            {
                LogError($"Failed to start server: {e.Message}");
            }
        }

        private void OpenUrlInBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // Required to open the URL in the default web browser
                });
            }
            catch (Exception e)
            {
                LogError($"Failed to open browser: {e.Message}");
            }
        }

        private async void HandleRequest(IAsyncResult result)
        {
            if (!_isRunning)
                return;

            try
            {
                var context = _httpListener.EndGetContext(result);
                var request = context.Request;
                var response = context.Response;

                if (EnableLogging)
                {
                    Log($"{request.HttpMethod} {request.Url}");
                }

                // Handle static content serving
                if (_config.developmentSettings.buildPath != null && request.Url.AbsolutePath == "/")
                {
                    string indexPath = Path.Combine(_config.developmentSettings.buildPath, "index.html");
                    if (File.Exists(indexPath))
                    {
                        string htmlContent = File.ReadAllText(indexPath);
                        if (EnableCsp)
                        {
                            await FetchCsp();
                            htmlContent = AppendCspMeta(htmlContent);
                        }

                        byte[] fileContent = Encoding.UTF8.GetBytes(htmlContent);
                        response.ContentType = "text/html";
                        response.ContentLength64 = fileContent.Length;
                        await response.OutputStream.WriteAsync(fileContent, 0, fileContent.Length);
                    }
                    else
                    {
                        response.StatusCode = 404;
                        byte[] errorContent = Encoding.UTF8.GetBytes("404 - File Not Found");
                        await response.OutputStream.WriteAsync(errorContent, 0, errorContent.Length);
                    }
                }
                else if (HostToProxy != null)
                {
                    // Handle proxying
                    await ProxyRequest(request, response);
                }
                else
                {
                    // Serve static files
                    string filePath = Path.Combine(_config.developmentSettings.buildPath, request.Url.LocalPath.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        byte[] fileContent = File.ReadAllBytes(filePath);
                        response.ContentType = GetContentType(filePath);
                        response.ContentLength64 = fileContent.Length;
                        await response.OutputStream.WriteAsync(fileContent, 0, fileContent.Length);
                    }
                    else
                    {
                        response.StatusCode = 404;
                        byte[] errorContent = Encoding.UTF8.GetBytes("404 - File Not Found");
                        await response.OutputStream.WriteAsync(errorContent, 0, errorContent.Length);
                    }
                }

                response.OutputStream.Close();
                _httpListener.BeginGetContext(HandleRequest, null);
            }
            catch (Exception e)
            {
                LogError($"Failed to handle request: {e.Message}");
            }
        }

        private async Task ProxyRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string targetUrl = $"{HostToProxy}{request.Url.PathAndQuery}";
                var proxyRequest = new HttpRequestMessage(new HttpMethod(request.HttpMethod), targetUrl);

                foreach (string header in request.Headers)
                {
                    proxyRequest.Headers.TryAddWithoutValidation(header, request.Headers[header]);
                }

                var proxyResponse = await _httpClient.SendAsync(proxyRequest);
                response.StatusCode = (int)proxyResponse.StatusCode;

                foreach (var header in proxyResponse.Headers)
                {
                    response.Headers[header.Key] = string.Join(",", header.Value);
                }

                byte[] responseData = await proxyResponse.Content.ReadAsByteArrayAsync();
                response.ContentLength64 = responseData.Length;
                await response.OutputStream.WriteAsync(responseData, 0, responseData.Length);
            }
            catch (Exception e)
            {
                LogError($"Failed to proxy request: {e.Message}");
                response.StatusCode = 502;
                byte[] errorContent = Encoding.UTF8.GetBytes("502 - Bad Gateway");
                await response.OutputStream.WriteAsync(errorContent, 0, errorContent.Length);
            }
        }

        private async Task FetchCsp()
        {
            if ((DateTime.Now - lastCspUpdate) < CspTimeout)
                return;

            lastCspUpdate = DateTime.Now;

            try
            {
                string appUrl = $"https://yandex.ru/games/app/{YandexAppId}?draft=true";
                var response = await _httpClient.GetAsync(appUrl);
                string appHtml = await response.Content.ReadAsStringAsync();

                // Use a simple string search to extract CSP for demonstration purposes.
                int cspStart = appHtml.IndexOf("Content-Security-Policy");
                if (cspStart >= 0)
                {
                    int contentStart = appHtml.IndexOf("content=", cspStart) + 9;
                    int contentEnd = appHtml.IndexOf("\"", contentStart);
                    csp = appHtml.Substring(contentStart, contentEnd - contentStart);
                }
            }
            catch (Exception e)
            {
                LogError($"Failed to fetch CSP: {e.Message}");
            }
        }

        private string AppendCspMeta(string html)
        {
            try
            {
                if (string.IsNullOrEmpty(csp)) return html;

                int headIndex = html.IndexOf("<head>");
                if (headIndex < 0) return html;

                string metaTag = $"<meta http-equiv=\"Content-Security-Policy\" content=\"{csp}\">";
                return html.Insert(headIndex + 6, metaTag);
            }
            catch (Exception e)
            {
                LogError($"Error while adding CSP meta tag: {e.Message}");
                return html;
            }
        }

        private string GetContentType(string filePath)
        {
            switch (Path.GetExtension(filePath).ToLower())
            {
                case ".html":
                    return "text/html";
                case ".js":
                    return "application/javascript";
                case ".css":
                    return "text/css";
                default:
                    return "application/octet-stream";
            }
        }

        public void StopServer()
        {
            if (_isRunning)
            {
                _httpListener.Stop();
                _httpListener.Close();
                _isRunning = false;
                Log("Server stopped.");
            }
        }

        private void Log(string message)
        {
            logBuilder.AppendLine(message);
            Debug.Log(message);
            OnLogUpdate?.Invoke();
        }

        private void LogError(string message)
        {
            logBuilder.AppendLine($"ERROR: {message}");
            Debug.LogError(message);
            OnLogUpdate?.Invoke();
        }
    }
}

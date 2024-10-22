using System;
using PlayablesStudio.Plugins.YandexGamesSDK.Editor.ProxyServer;
using PlayablesStudio.Plugins.YandexGamesSDK.Editor.ProxyServer.Abstractions;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Dashboard;
using UnityEditor;
using UnityEngine;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Editor.Dashboard
{
    public class YandexGamesSDKDashboard : EditorWindow
    {
        private YandexGamesSDKConfig config;
        private Vector2 logScrollPos;
        private string logText = "";
        private bool autoScroll = true;

        private IProxyServer _proxyServer;

        [MenuItem("Yandex Games SDK/Dashboard")]
        public static void ShowWindow()
        {
            GetWindow<YandexGamesSDKDashboard>("Yandex Games SDK Dashboard");
        }

        private void OnEnable()
        {
            LoadConfig();
            _proxyServer = ProxyServerFactory.ProxyServer;

            _proxyServer.OnLogUpdate += UpdateLogs;

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.quitting += OnQuitting;
        }

        private void OnDisable()
        {
            _proxyServer.Cleanup();

            _proxyServer.OnLogUpdate -= UpdateLogs;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.quitting -= OnQuitting;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            if (config == null)
            {
                EditorGUILayout.HelpBox("YandexGamesSDKConfig asset not found. Please create one.",
                    MessageType.Warning);

                if (GUILayout.Button("Create Config Asset"))
                {
                    CreateConfigAsset();
                }

                return;
            }

            // Display Config Asset Field
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            config = (YandexGamesSDKConfig)EditorGUILayout.ObjectField("Config Asset", config,
                typeof(YandexGamesSDKConfig), false);

            EditorGUILayout.Space();

            // Begin displaying config properties
            SerializedObject serializedConfig = new SerializedObject(config);
            serializedConfig.Update();

            // General Settings
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("appID"), new GUIContent("App ID"));

            EditorGUILayout.PropertyField(serializedConfig.FindProperty("verboseLogging"),
                new GUIContent("Verbose Logging"));

            EditorGUILayout.Space();

            // Mock Data
            EditorGUILayout.LabelField("Mock Data", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("useMockData"),
                new GUIContent("Use Mock Data"));
            if (config.useMockData)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedConfig.FindProperty("mockData"), true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // Pause Settings
              
            EditorGUILayout.LabelField("Pause Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("pauseSettings"), true);
            
            EditorGUILayout.Space();

            // Development Settings
            EditorGUILayout.LabelField("Development Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("developmentSettings"), true);

            serializedConfig.ApplyModifiedProperties();

            EditorGUILayout.Space();

            // Rest of your dashboard code (Server Controls, Logs, etc.)
            DrawServerControls();
            DrawServerLogs();
        }

        private void DrawServerControls()
        {
            EditorGUILayout.LabelField("Local Server Controls", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (_proxyServer.IsRunning)
            {
                if (GUILayout.Button("Stop Server"))
                {
                    _proxyServer.StopProxyServer();
                }
            }
            else
            {
                if (GUILayout.Button("Start Server"))
                {
                    if (string.IsNullOrEmpty(config.appID))
                    {
                        if (EditorUtility.DisplayDialog("Invalid App ID", "Please insert the Yandex Games App Id.",
                                "Ok."))
                        {
                            GUI.FocusControl("AppIDField");
                        }

                        return;
                    }

                    string indexPath = System.IO.Path.Combine(config.developmentSettings.buildPath, "index.html");

                    if (!System.IO.File.Exists(indexPath))
                    {
                        if (EditorUtility.DisplayDialog("Invalid Build Path",
                                "The build path is not valid as it does not contain 'index.html'. Would you like to build the project to set a valid build path?",
                                "Yes, Build Now", "Cancel"))
                        {
                            config.developmentSettings.overrideOnBuild = true;
                            config.developmentSettings.runLocalServerAfterBuild = true;

                            EditorApplication.ExecuteMenuItem("File/Build And Run");
                        }

                        return;
                    }

                    _proxyServer.StartProxyServer();
                }
            }

            EditorGUILayout.EndHorizontal();

            // Server Status Indicator
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Server Status: ");
            GUIStyle statusStyle = new GUIStyle(EditorStyles.boldLabel);
            if (_proxyServer.IsRunning)
            {
                statusStyle.normal.textColor = Color.green;
                GUILayout.Label("Running", statusStyle);
            }
            else
            {
                statusStyle.normal.textColor = Color.red;
                GUILayout.Label("Stopped", statusStyle);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawServerLogs()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Server Logs", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            autoScroll = GUILayout.Toggle(autoScroll, "Auto-Scroll", GUILayout.Width(100));
            if (GUILayout.Button("Clear Logs"))
            {
                _proxyServer.ClearLogs();

                logText = "";
            }

            EditorGUILayout.EndHorizontal();

            logScrollPos = EditorGUILayout.BeginScrollView(logScrollPos, GUILayout.Height(300));
            EditorGUILayout.TextArea(logText, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            // Repaint to update UI
            if (autoScroll)
            {
                Repaint();
            }
        }

        private void UpdateLogs()
        {
            logText = _proxyServer.Logs;
        }

        private void LoadConfig()
        {
            if (config == null)
            {
                config = Resources.Load<YandexGamesSDKConfig>("YandexGamesSDKConfig");
            }

            if (config == null)
            {
                Debug.LogWarning("YandexGamesSDKConfig asset not found in Resources folder.");
            }
        }

        private void CreateConfigAsset()
        {
            config = CreateInstance<YandexGamesSDKConfig>();
            AssetDatabase.CreateAsset(config, "Assets/YandexGamesSDK/Resources/YandexGamesSDKConfig.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = config;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                Debug.Log("TODO: Play Mode is ExitingEditMode");
            }
        }

        private void OnQuitting()
        {
            _proxyServer.Cleanup();
        }
    }
}
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Dashboard;
using UnityEditor;
using UnityEngine;

namespace PlayablesStudio.Plugins.YandexGamesSDK.Editor.Dashboard
{
    public class YandexGamesSDKDashboard : EditorWindow
    {
        private YandexGamesSDKConfig config;
        private LocalServerManager _localServerManager;
        private Vector2 logScrollPos;
        private string logText = "";
        private bool autoScroll = true;

        [MenuItem("Yandex Games SDK/Dashboard")]
        public static void ShowWindow()
        {
            GetWindow<YandexGamesSDKDashboard>("Yandex Games SDK Dashboard");
        }

        private void OnEnable()
        {
            LoadConfig();
            _localServerManager = new LocalServerManager();
            _localServerManager.OnLogUpdate += UpdateLogs;

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.quitting += OnQuitting;
        }

        private void OnDisable()
        {
            _localServerManager.OnLogUpdate -= UpdateLogs;
            _localServerManager.Cleanup();

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.quitting -= OnQuitting;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            if (config == null)
            {
                EditorGUILayout.HelpBox("YandexGamesSDKConfig asset not found. Please create one.", MessageType.Warning);

                if (GUILayout.Button("Create Config Asset"))
                {
                    CreateConfigAsset();
                }

                return;
            }

            // Display Config Asset Field
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            config = (YandexGamesSDKConfig)EditorGUILayout.ObjectField("Config Asset", config, typeof(YandexGamesSDKConfig), false);

            EditorGUILayout.Space();

            // Begin displaying config properties
            SerializedObject serializedConfig = new SerializedObject(config);
            serializedConfig.Update();

            // General Settings
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("appID"), new GUIContent("App ID"));
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("verboseLogging"), new GUIContent("Verbose Logging"));

            EditorGUILayout.Space();

            // Mock Data
            EditorGUILayout.LabelField("Mock Data", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("useMockData"), new GUIContent("Use Mock Data"));
            if (config.useMockData)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedConfig.FindProperty("mockData"), true);
                EditorGUI.indentLevel--;
            }

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
            if (_localServerManager.IsRunning)
            {
                if (GUILayout.Button("Stop Server"))
                {
                    _localServerManager.StopServer();
                }
            }
            else
            {
                if (GUILayout.Button("Start Server"))
                {
                    StartServer();
                }
            }
            EditorGUILayout.EndHorizontal();

            // Server Status Indicator
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Server Status: ");
            GUIStyle statusStyle = new GUIStyle(EditorStyles.boldLabel);
            if (_localServerManager.IsRunning)
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
                _localServerManager.ClearLogs();
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

        private void StartServer()
        {
            string buildPath = config.developmentSettings.buildPath;
            string port = config.developmentSettings.serverPort.ToString();

            if (string.IsNullOrEmpty(buildPath))
            {
                EditorUtility.DisplayDialog("Error", "Build Path is not set.", "OK");
                return;
            }

            _localServerManager.StartLocalServer(buildPath, port);
        }

        private void UpdateLogs()
        {
            logText = _localServerManager.Logs;
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
                _localServerManager.StopServer();
            }
        }

        private void OnQuitting()
        {
            _localServerManager.Cleanup();
        }
    }
}

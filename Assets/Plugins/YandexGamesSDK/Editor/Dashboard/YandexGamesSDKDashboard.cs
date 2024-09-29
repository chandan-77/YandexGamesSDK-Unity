using Plugins.YandexGamesSDK.Runtime.Dashboard;
using UnityEditor;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.YandexGamesSDK.Editor.Dashboard
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

            // Register to cleanup when Unity closes or the window is destroyed
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

            SerializedObject serializedConfig = new SerializedObject(config);

            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("useMockData"));
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("appID"));
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("isYandexPlatform"));
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("verboseLogging"));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Mock User Profile", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("mockUserProfile"), true);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Mock Leaderboard Entries", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("mockLeaderboardEntries"), true);

            // Add more sections for other configurations

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Development Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("runLocalServerAfterBuild"), new GUIContent("Run Local Server After Build"));

            // Add fields for server parameters
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Server Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("OverrideOnBuild"), new GUIContent("Override On Build"));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Build Path:");
            config.BuildPath = EditorGUILayout.TextField(config.BuildPath);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Server Port:");
            config.ServerPort = EditorGUILayout.IntField(config.ServerPort, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            serializedConfig.ApplyModifiedProperties();

            EditorGUILayout.Space();
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
                    string buildPath = config.BuildPath;
                    string appId = config.appID;
                    string port = config.ServerPort.ToString();

                    if (string.IsNullOrEmpty(buildPath))
                    {
                        EditorUtility.DisplayDialog("Error", "Build Path is not set.", "OK");
                        return;
                    }

                    _localServerManager.StartLocalServer(buildPath, port);
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

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Server Logs", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            autoScroll = GUILayout.Toggle(autoScroll, "Auto-Scroll", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            logScrollPos = EditorGUILayout.BeginScrollView(logScrollPos, GUILayout.Height(300));
            EditorGUILayout.TextArea(logText, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private void UpdateLogs()
        {
            logText = _localServerManager.Logs;
            if (autoScroll)
            {
                // Force the GUI to update scroll position to the bottom
                Repaint();
            }
        }

        private void LoadConfig()
        {
            config = Resources.Load<YandexGamesSDKConfig>("YandexGamesSDKConfig");

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
                // Optionally, stop the server when entering play mode
                _localServerManager.StopServer();
            }
        }

        private void OnQuitting()
        {
            _localServerManager.Cleanup();
        }
    }
}

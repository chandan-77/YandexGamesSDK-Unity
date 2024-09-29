using Plugins.YandexGamesSDK.Runtime.Dashboard;
using UnityEditor;
using UnityEngine;

namespace Plugins.YandexGamesSDK.Editor.Dashboard
{
    public class YandexGamesSDKDashboard : EditorWindow
    {
        private YandexGamesSDKConfig config;

        [MenuItem("Yandex Games SDK/Dashboard")]
        public static void ShowWindow()
        {
            GetWindow<YandexGamesSDKDashboard>("Yandex Games SDK Dashboard");
        }

        private void OnEnable()
        {
            LoadConfig();
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

            
            serializedConfig.ApplyModifiedProperties();
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
    }
}
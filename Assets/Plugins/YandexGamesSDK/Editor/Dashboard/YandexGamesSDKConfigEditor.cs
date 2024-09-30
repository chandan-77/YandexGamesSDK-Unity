using Plugins.YandexGamesSDK.Runtime.Dashboard;
using UnityEditor;

namespace Plugins.YandexGamesSDK.Editor.Dashboard
{
    [CustomEditor(typeof(YandexGamesSDKConfig))]
    public class YandexGamesSDKConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty appIDProp;
        private SerializedProperty isYandexPlatformProp;
        private SerializedProperty verboseLoggingProp;
        private SerializedProperty useMockDataProp;
        private SerializedProperty mockDataProp;
        private SerializedProperty developmentSettingsProp;

        private void OnEnable()
        {
            appIDProp = serializedObject.FindProperty("appID");
            isYandexPlatformProp = serializedObject.FindProperty("isYandexPlatform");
            verboseLoggingProp = serializedObject.FindProperty("verboseLogging");
            useMockDataProp = serializedObject.FindProperty("useMockData");
            mockDataProp = serializedObject.FindProperty("mockData");
            developmentSettingsProp = serializedObject.FindProperty("developmentSettings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(appIDProp);
            EditorGUILayout.PropertyField(isYandexPlatformProp);
            EditorGUILayout.PropertyField(verboseLoggingProp);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Mock Data", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useMockDataProp);
            if (useMockDataProp.boolValue)
            {
                EditorGUILayout.PropertyField(mockDataProp, true);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Development Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(developmentSettingsProp, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
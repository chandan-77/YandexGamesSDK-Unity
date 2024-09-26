using UnityEditor;
using System.IO;
using UnityEngine;

namespace Plugins.YandexGamesSDK.Editor
{
    public class JSConversionEditorWindow : EditorWindow
    {
        [MenuItem("Tools/Yandex Games SDK/Convert JS to JSLib")]
        public static void ShowWindow()
        {
            GetWindow<JSConversionEditorWindow>("JS to JSLib Converter");
        }

        private void OnGUI()
        {
            GUILayout.Label("Convert all .js files to .jslib in YandexGamesSDK", EditorStyles.boldLabel);

            if (GUILayout.Button("Convert JS to JSLib"))
            {
                ConvertJsToJslib();
            }
        }

        private void ConvertJsToJslib()
        {
            string sourceDirectory = Path.Combine(Application.dataPath, "Plugins", "YandexGamesSDK", "Runtime", "WebGL");
            string destinationDirectory = sourceDirectory;  // Same directory for output

            string[] jsFiles = Directory.GetFiles(sourceDirectory, "*.js", SearchOption.AllDirectories);

            foreach (string jsFile in jsFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(jsFile);
                string destinationFile = Path.Combine(destinationDirectory, fileName + ".jslib");

                File.Copy(jsFile, destinationFile, true);

                Debug.Log($"Converted {jsFile} to {destinationFile}");
            }

            Debug.Log("All .js files have been converted to .jslib.");
        }
    }
}
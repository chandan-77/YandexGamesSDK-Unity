using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ExportUnityPackage
    {
        public static void ExportPackageWithVersion(string version)
        {
            PlayerSettings.bundleVersion = version;

            string[] folders = { "Assets/Plugins/YandexGamesSDK" };

            var exportDir = "Releases";
            
            if (!System.IO.Directory.Exists(exportDir))
            {
                System.IO.Directory.CreateDirectory(exportDir);
            }

            var packageName = $"{exportDir}/yandex_games_sdk_{version}.unitypackage";

            AssetDatabase.ExportPackage(folders, packageName, ExportPackageOptions.Recurse);

            Debug.Log($"Exported to: {packageName}");
        }
    }
}
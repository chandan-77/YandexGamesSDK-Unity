using System.IO;
using PlayablesStudio.Plugins.YandexGamesSDK.Runtime.Dashboard;
using UnityEditor;
using UnityEngine;

namespace YandexGames.Editor
{
    [InitializeOnLoad]
    public static class PackageInstaller
    {
        private const string CONFIG_NAME = "YandexGamesSDKConfig";
        private const string BASE_FOLDER = "YandexGamesSDK";
        private const string RESOURCES_FOLDER = "Resources";

        static PackageInstaller()
        {
            EnsureConfigAsset();
        }

        static void EnsureConfigAsset()
        {
            string resourcesPath = GetResourcesPath();
            string assetPath = Path.Combine("Assets", BASE_FOLDER, RESOURCES_FOLDER, $"{CONFIG_NAME}.asset");

            // Check if config already exists
            var existingConfig = Resources.Load<YandexGamesSDKConfig>(CONFIG_NAME);
            if (existingConfig != null)
            {
                Debug.Log($"[YandexGamesSDK] Config already exists at {assetPath}");
                return;
            }

            // Create necessary directories
            EnsureDirectoryExists(resourcesPath);

            // Create and save new config
            var config = ScriptableObject.CreateInstance<YandexGamesSDKConfig>();
            AssetDatabase.CreateAsset(config, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[YandexGamesSDK] Created new config at {assetPath}");
        }

        static string GetResourcesPath()
        {
            return Path.Combine(Application.dataPath, BASE_FOLDER, RESOURCES_FOLDER);
        }

        static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log($"[YandexGamesSDK] Created directory: {path}");
            }
        }

        static void CopyConfigAsset()
        {
            string destinationDir = Path.Combine(Application.dataPath, "YandexGamesSDK", "Resources");
            string destinationPath = Path.Combine(destinationDir, "YandexGamesSDK.asset");

            string packagePath = GetPackagePath();

            if (string.IsNullOrEmpty(packagePath))
            {
                Debug.LogError("[YandexGamesSDK] Could not determine the package path.");
                return;
            }

            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            if (!File.Exists(destinationPath))
            {
                var sdkConfig = YandexGamesSDKConfig.Instance;
                AssetDatabase.Refresh();
                Debug.Log("[YandexGamesSDK] Config asset copied to Assets/YandexGamesSDK/Resources/");
            }
            else
            {
                Debug.Log("[YandexGamesSDK] Config asset already exists at Assets/YandexGamesSDK/Resources/");
            }
        }

        static string GetPackagePath()
        {
            string scriptFilePath = GetScriptFilePath();
            if (string.IsNullOrEmpty(scriptFilePath))
            {
                return null;
            }

            string scriptDir = Path.GetDirectoryName(scriptFilePath);

            string packagePath = Path.GetFullPath(Path.Combine(scriptDir, ".."));

            return packagePath;
        }

        static string GetScriptFilePath()
        {
            string[] guids = AssetDatabase.FindAssets("PackageInstaller t:Script");
            if (guids.Length == 0)
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return Path.GetFullPath(path);
        }

        static void CopyDirectory(string sourceDir, string destinationDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Copy files in the current directory
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(tempPath, true);
            }

            // Copy subdirectories recursively
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destinationDir, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
        }
    }
}

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Plugins.YandexGamesSDK.Editor
{
    [InitializeOnLoad]
    public static class PackageInstaller
    {
        static PackageInstaller()
        {
            CopyWebGLTemplate();
            CopyConfigAsset();
        }

        /// <summary>
        /// Copies the WebGL template from the package to the project's Assets folder.
        /// </summary>
        static void CopyWebGLTemplate()
        {
            string sourceRelativePath = Path.Combine("Runtime", "WebGLTemplates", "YandexGames");
            string destinationPath = Path.Combine(Application.dataPath, "WebGLTemplates", "YandexGames");

            string packagePath = GetPackagePath();

            if (string.IsNullOrEmpty(packagePath))
            {
                Debug.LogError("[YandexGamesSDK] Could not determine the package path.");
                return;
            }

            string sourcePath = Path.Combine(packagePath, sourceRelativePath);

            if (!Directory.Exists(sourcePath))
            {
                Debug.LogError($"[YandexGamesSDK] Source path does not exist: {sourcePath}");
                return;
            }

            if (!Directory.Exists(destinationPath))
            {
                // Create the destination directory
                Directory.CreateDirectory(destinationPath);

                // Copy the template files
                CopyDirectory(sourcePath, destinationPath);

                // Refresh the AssetDatabase to recognize new files
                AssetDatabase.Refresh();

                Debug.Log("[YandexGamesSDK] WebGL Template successfully copied to Assets/WebGLTemplates/YandexGames");
            }
            else
            {
                Debug.Log("[YandexGamesSDK] WebGL Template already exists at Assets/WebGLTemplates/YandexGames");
            }
        }

        /// <summary>
        /// Copies the YandexGamesSDKConfig.asset from the package to the project's Assets folder.
        /// </summary>
        static void CopyConfigAsset()
        {
            string sourceRelativePath = Path.Combine("Runtime", "Resources", "YandexGamesSDKConfig.asset");
            string destinationDir = Path.Combine(Application.dataPath, "YandexGamesSDK", "Resources");
            string destinationPath = Path.Combine(destinationDir, "YandexGamesSDKConfig.asset");

            string packagePath = GetPackagePath();

            if (string.IsNullOrEmpty(packagePath))
            {
                Debug.LogError("[YandexGamesSDK] Could not determine the package path.");
                return;
            }

            string sourcePath = Path.Combine(packagePath, sourceRelativePath);

            if (!File.Exists(sourcePath))
            {
                Debug.LogError($"[YandexGamesSDK] Config asset does not exist at source path: {sourcePath}");
                return;
            }

            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            if (!File.Exists(destinationPath))
            {
                File.Copy(sourcePath, destinationPath);
                AssetDatabase.Refresh();
                Debug.Log("[YandexGamesSDK] Config asset copied to Assets/YandexGamesSDK/Resources/");
            }
            else
            {
                // Perform validation or prompt the user
                Debug.Log("[YandexGamesSDK] Config asset already exists at Assets/YandexGamesSDK/Resources/");
                // Optional: Implement version checking or prompting for overwrite
            }
        }

        /// <summary>
        /// Determines the package path based on the location of this script.
        /// </summary>
        /// <returns>The package path if found, or null otherwise.</returns>
        static string GetPackagePath()
        {
            // Get the path of this script
            string scriptFilePath = GetScriptFilePath();
            if (string.IsNullOrEmpty(scriptFilePath))
            {
                return null;
            }

            // Get the directory of the script
            string scriptDir = Path.GetDirectoryName(scriptFilePath);

            // The script is in /Runtime/, so go up to package root
            string packagePath = Path.GetFullPath(Path.Combine(scriptDir, ".."));

            return packagePath;
        }

        /// <summary>
        /// Finds the file path of this script using the AssetDatabase.
        /// </summary>
        /// <returns>The file path if found, or null otherwise.</returns>
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

        /// <summary>
        /// Recursively copies a directory and its contents.
        /// </summary>
        /// <param name="sourceDir">The source directory.</param>
        /// <param name="destinationDir">The destination directory.</param>
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


using UnityEditor;

namespace EtienneEditor.Animator2D
{
    /// <summary>
    /// Provides utility methods for accessing editor resources.
    /// </summary>
    internal static class AnimatorPath
    {
        /// <summary>
        /// The path to the Editor folder.
        /// </summary>
        public static string EditorPath => editorPath ??= FindEditorPath();
        private static string editorPath;

        /// <summary>
        /// The path to the Editor Default Resources folder.
        /// </summary>
        public static string EditorDefaultResourcesPath => editorDefaultResourcesPath ??= $"{EditorPath}Editor Default Resources/";
        private static string editorDefaultResourcesPath;

        private const string FILTER = "Animator2DEditorWindow t:VisualTreeAsset";

        /// <summary>
        /// Finds the path to the Editor folder.
        /// </summary>
        private static string FindEditorPath()
        {
            string[] guids = AssetDatabase.FindAssets(FILTER, new[] { "Packages", "Assets" });
            if (guids.Length == 0)
            {
                UnityEngine.Debug.LogError($"Could not find any asset matching the filter '{FILTER}'.");
                return null;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            const int subFolderCount = 3;
            string directoryPath = assetPath;
            for (int i = 0; i < subFolderCount; i++)
            {
                directoryPath = System.IO.Path.GetDirectoryName(directoryPath);
            }
            return $"{directoryPath}/";
        }

        /// <summary>
        /// Loads an asset from the Editor Default Resources folder.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="fileName">The name of the file to load.</param>
        /// <returns>The loaded asset.</returns>
        internal static T LoadFromEditorResources<T>(string fileName) where T : UnityEngine.Object
        {
            string assetPath = EditorDefaultResourcesPath + fileName;
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
    }
}

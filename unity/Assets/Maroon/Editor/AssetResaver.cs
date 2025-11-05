using UnityEngine;
using UnityEditor;

namespace Maroon.Editor
{
    /// <summary>
    /// Asset Resaver, can be used after a Unity version upgrade to resave all assets with new serialization versions.
    /// Prevents later PRs having many lines of changes in a file, although only changing a minor thing.
    /// </summary>
    public class AssetResaver : EditorWindow
    {
        private static readonly string[] foldersToSearch = { "Assets" };

        [MenuItem("Tools/Reserialize Assets")]
        public static void ReserializeAllAssets()
        {
            AssetDatabase.ForceReserializeAssets();
            Debug.Log("Finished reserializing assets.");
        }
    }
}
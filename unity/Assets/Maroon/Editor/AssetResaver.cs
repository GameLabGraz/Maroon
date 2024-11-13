using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Maroon.Editor
{
    /// <summary>
    /// Asset Resaver, can be used after a Unity version upgrade to resave all assets with new serialization versions.
    /// Prevents later PRs having many lines of changes in a file, although only changing a minor thing.
    /// </summary>
    public class AssetResaver : EditorWindow
    {
        [MenuItem("Tools/Resave Assets")]
        public static void ResaveAllAssets()
        {
            string[] foldersToSearch = new[] { "Assets" };
            int changes = 0;

            // Resave prefabs
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", foldersToSearch);
            foreach (string guid in prefabGuids)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab != null)
                {
                    EditorUtility.SetDirty(prefab);
                    AssetDatabase.SaveAssets();
                    changes++;
                    Debug.Log("Resaved Prefab: " + prefabPath);
                }
            }

            // Resave Scenes
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", foldersToSearch);
            foreach (string guid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guid);
                if (File.Exists(scenePath))
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                    changes++;
                    Debug.Log("Resaved Scene: " + scenePath);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"Finished resaving assets. Made {changes} change(s).");
        }
    }
}
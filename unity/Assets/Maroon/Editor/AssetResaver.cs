using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

namespace Maroon.Editor
{
    /// <summary>
    /// Asset Resaver, can be used after a Unity version upgrade to resave all assets with new serialization versions.
    /// Prevents later PRs having many lines of changes in a file, although only changing a minor thing.
    /// </summary>
    public class AssetResaver : EditorWindow
    {
        private static readonly string[] foldersToSearch = { "Assets" };

        [MenuItem("Tools/Resave Assets")]
        public static void ResaveAllAssets()
        {
            int changes = 0;

            changes += ResaveAllAssetsOfType<GameObject>("t:Prefab");
            changes += ResaveAllAssetsOfType<Material>("t:Material");
            changes += ResaveAllAssetsOfType<Shader>("t:Shader");
            changes += ResaveAllAssetsOfType<ScriptableObject>("t:ScriptableObject");
            changes += ResaveAllAssetsOfType<Texture>("t:Texture");
            changes += ResaveAllAssetsOfType<AnimationClip>("t:AnimationClip");
            changes += ResaveAllScenes();

            AssetDatabase.Refresh();
            Debug.Log($"Finished resaving assets. Made {changes} change(s).");
        }

        /// <summary>
        /// Resaves all assets of a certain type
        /// </summary>
        /// <typeparam name="T">The type of asset to resave, e.g. Materials</typeparam>
        /// <param name="filter">File filter, e.g. t:Material</param>
        /// <returns>The number of assets resaved</returns>
        private static int ResaveAllAssetsOfType<T>(string filter) where T : UnityEngine.Object
        {
            int changes = 0;
            string[] guids = AssetDatabase.FindAssets(filter, foldersToSearch);
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    EditorUtility.SetDirty(asset);
                    AssetDatabase.SaveAssets();
                    changes++;
                }
            }
            Debug.Log($"Resaved {changes} assets of type {typeof(T).Name}.");
            return changes;
        }

        /// <summary>
        /// Resaves all scenes
        /// </summary>
        /// <returns>The number of scenes resaved</returns>
        private static int ResaveAllScenes()
        {
            int changes = 0;
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", foldersToSearch);
            foreach (string guid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guid);
                if (File.Exists(scenePath))
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                    changes++;
                }
            }

            Debug.Log($"Resaved {changes} scenes.");
            return changes;
        }
    }
}
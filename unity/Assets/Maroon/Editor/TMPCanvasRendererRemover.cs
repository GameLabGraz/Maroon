using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using TMPro;

namespace Maroon.Editor
{
    /// <summary>
    /// A few versions ago, TMP components required a CanvasRenderer component to be attached to them.
    /// Now this is not the case anymore, and Unity throws a warning instead.
    /// As these warnings are annoying, and we have many legacy TMPs in the project, this script automates removing the CanvasRenderers.
    /// </summary>
    public class TMPCanvasRendererRemover : EditorWindow
    {
        private static readonly string[] foldersToSearch = { "Assets" };
        
        [MenuItem("Tools/Remove CanvasRenderer from TMPs")]
        public static void RemoveCanvasRenderersFromTMPs()
        {
            int counter = 0;

            counter += RemoveFromPrefabs();
            counter += RemoveFromScenes();

            AssetDatabase.Refresh();
            Debug.Log($"Finished removing CanvasRenderers from {counter} TMPs.");
        }

        /// <summary>
        /// Removes all CanvasRenderers from all Prefabs that have TMPs
        /// </summary>
        /// <returns>The number of canvasRenderers removed</returns>
        private static int RemoveFromPrefabs()
        {
            int counter = 0;
            string[] guids = AssetDatabase.FindAssets("t:Prefab", foldersToSearch);
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (go != null)
                {
                    foreach (TMP_Text tmp in go.GetComponentsInChildren<TMP_Text>(true))
                    {
                        GameObject childGo = tmp.gameObject;
                        if (childGo.GetComponent<CanvasRenderer>() != null)
                        {
                            // Found TMP and CanvasRenderer -> remove CanvasRenderer
                            Undo.DestroyObjectImmediate(childGo.GetComponent<CanvasRenderer>());
                            Debug.Log($"Removed CanvasRenderer from {childGo.name} at {assetPath}");
                            counter++;
                            EditorUtility.SetDirty(go);
                        }
                    }
                    AssetDatabase.SaveAssets();
                }
            }
            return counter;
        }

        /// <summary>
        /// Removes all CanvasRenderers from all GameObjects that also have TMPs
        /// </summary>
        /// <returns>The number of CanvasRenderers removed</returns>
        private static int RemoveFromScenes()
        {
            int counter = 0;
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", foldersToSearch);
            string initialScenePath = EditorSceneManager.GetActiveScene().path;
            foreach (string guid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guid);
                int startBeforeSceneCounter = counter;
                if (File.Exists(scenePath))
                {
                    EditorSceneManager.OpenScene(scenePath);

                    GameObject[] sceneGameObjects = GameObject.FindObjectsOfType<GameObject>();
                    foreach (GameObject go in sceneGameObjects)
                    {
                        if (go.GetComponent<TMP_Text>() != null && go.GetComponent<CanvasRenderer>() != null)
                        {
                            // Found TMP and CanvasRenderer -> remove CanvasRenderer
                            Undo.DestroyObjectImmediate(go.GetComponent<CanvasRenderer>());
                            Debug.Log($"Removed CanvasRenderer from {go.name} in {scenePath}");
                            counter++;
                        }
                    }

                    if (startBeforeSceneCounter < counter)
                    {
                        // Only save scene if something changed
                        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                    }
                }
            }

            // Restore initial scene
            EditorSceneManager.OpenScene(initialScenePath);
            return counter;
        }
    }
}
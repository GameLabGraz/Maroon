#if UNITY_EDITOR
namespace Fury.Editor
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEditor;

    public class MissingReferenceFinder : EditorWindow
    {

        private Vector2 scrollPos = Vector2.zero;
        GUIStyle headStyle = new GUIStyle();

        private int selectedButtonIndex = -1;

        private Color selectedColor = Color.white;

        public List<Result> results = new List<Result>();
        [MenuItem("Tools/MissingReferenceFinder")]
        public static void ShowWindow()
        {
            MissingReferenceFinder window = GetWindow<MissingReferenceFinder>();
            window.titleContent = new GUIContent("MissingReferenceFinder");
            window.Show();
        }


        private void OnEnable()
        {
            headStyle.fontSize = 15;
            headStyle.fontStyle = FontStyle.Bold;
            headStyle.alignment = TextAnchor.MiddleCenter;
            headStyle.normal.textColor = Color.white;
        }

        public void OnGUI()
        {

            EditorGUILayout.BeginVertical(GUILayout.Width(position.width), GUILayout.Height(position.height));
            {
                GUILayout.Space(10); // Add gap before the heading

                DrawLabel("MissingReferenceFinder", headStyle);

                GUILayout.Space(10); // Add gap after the heading

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("SearchAssetFolder", GetButtonStyle(0)))
                    {
                        selectedButtonIndex = 0;
                        SearchAssetFolder();
                    }

                    if (GUILayout.Button("SearchCurrentScene", GetButtonStyle(1)))
                    {
                        selectedButtonIndex = 1;
                        SearchCurrentScene();
                    }

                    if (GUILayout.Button("SearchSelectedObject", GetButtonStyle(2)))
                    {
                        selectedButtonIndex = 2;
                        SerachSelectedAsset();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(position.width - 5), GUILayout.Height(position.height - 60));
                {
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos); // Adding scroll view for the Components List
                    {
                        foreach (var item in results)
                        {
                            EditorGUILayout.ObjectField(item.Obj, typeof(GameObject), true);
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

        }

        private GUIStyle GetButtonStyle(int buttonIndex)
        {
            if (buttonIndex == selectedButtonIndex)
            {
                // Create a GUIStyle for the selected button appearance
                GUIStyle selectedButtonStyle = new GUIStyle(EditorStyles.toolbarButton);

                // Adjust the visual properties to make the border more visible
                selectedButtonStyle.normal.textColor = Color.white; // Change the text color
                selectedButtonStyle.normal.background = MakeTexture(2, 2, new Color(0.49f, 0.25f, 0.03f, 1f)); // Add a background color

                return selectedButtonStyle;
            }
            else
            {
                // Return the default button style
                return EditorStyles.miniButton;
            }
        }

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }


        private void DrawLabel(string text, GUIStyle style)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, style);
            Color32 color = new Color32(125, 64, 8, 225);
            EditorGUI.DrawRect(rect, color);
            GUI.Label(rect, text, style);
        }

        private void SerachSelectedAsset()
        {
            results.Clear();
            if (Selection.activeGameObject)
                Traverse(Selection.activeGameObject.transform);
            Debug.Log("> Total Results: " + results.Count);
        }

        public void SearchCurrentScene()
        {
            results.Clear();
            GameObject[] gos = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject go in gos) Traverse(go.transform);
            Debug.Log("> Total Results: " + results.Count);
        }

        public void SearchAssetFolder()
        {
            results.Clear();
            var allPrefabs = GetAllPrefabAddress();
            int count = 0;
            EditorUtility.DisplayProgressBar("Processing...", "Begin Job", 0);

            foreach (string prefab in allPrefabs)
            {
                UnityEngine.Object o = AssetDatabase.LoadMainAssetAtPath(prefab);

                if (o == null)
                {
                    Debug.Log("prefab " + prefab + " null?");
                    continue;
                }

                GameObject go;
                go = o as GameObject;
                if (go != null)
                {
                    EditorUtility.DisplayProgressBar("Processing...", go.name, ++count / (float)allPrefabs.Length);
                    Traverse(go.transform);


                }
            }
            Debug.Log("> Total Results: " + results.Count);
            EditorUtility.ClearProgressBar();
        }


        public string[] GetAllPrefabAddress()
        {
            string[] temp = AssetDatabase.GetAllAssetPaths();
            List<string> result = new List<string>();
            foreach (string s in temp)
            {
                // Skip assets from the package cache
                if(s.StartsWith("Packages"))
                    continue;
                
                if (s.Contains(".prefab")) result.Add(s);
            }
            return result.ToArray();
        }

        private void AppendComponentResult(string childPath, int index, GameObject obj)
        {
            results.Add(new Result()
            {
                address = "Missing Component " + index + " of " + childPath,
                Obj = obj,
            });
        }
        private void AppendTransformResult(string childPath, string name, GameObject obj)
        {
            results.Add(new Result()
            {
                address = "Missing Prefab for \"" + name + "\" of " + childPath,
                Obj = obj,
            });
        }
        private void Traverse(Transform transform, string path = "")
        {
            string thisPath = path + "/" + transform.name;
            Component[] components = transform.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    AppendComponentResult(thisPath, i, transform.gameObject);
                }
            }

            for (int c = 0; c < transform.childCount; c++)
            {
                Transform t = transform.GetChild(c);
                PrefabAssetType pt = PrefabUtility.GetPrefabAssetType(t.gameObject);
                if (pt == PrefabAssetType.MissingAsset)
                {
                    AppendTransformResult(path + "/" + transform.name, t.name, transform.gameObject);
                }
                else
                {
                    Traverse(t, thisPath);
                }
            }
        }
    }

    [System.Serializable]
    public class Result
    {
        public string address;
        public GameObject Obj;
    }
}

#endif
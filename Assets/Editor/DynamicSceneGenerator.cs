using UnityEngine;
using UnityEditor;

public class DynamicSceneGenerator : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    private int tab = 0;
    private string[] tabStrings = { "Build Scene", "Parameters", "Quiz" };

    // Add menu named "Dynamic Scene Generator" to the Window menu
    [MenuItem("Window/Dynamic Scene Generator")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DynamicSceneGenerator window = (DynamicSceneGenerator)EditorWindow.GetWindow(typeof(DynamicSceneGenerator), false, "Dynamic Scene Generator");
        window.Show();
    }

    private void OnGUI()
    {
        tab = GUILayout.Toolbar(tab, tabStrings);
        switch(tab)
        {
            case 0:
                showBuildSceneTab();
                break;
            case 1:
                showParameterTab();
                break;
            case 2:
                showQuizTab();
                break;
            default:
                Debug.LogError("DynamicSceneGenerator: Unknown Tab Index.");
                break;
        }
    }

    private void showBuildSceneTab()
    {
        EditorGUILayout.BeginVertical();
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("Text Field", myString);

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Build Web View Scene ..."))
                {

                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Build Physic Lab Scene ..."))
                {

                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
        EditorGUILayout.EndVertical();
    }

    private void showParameterTab()
    {
        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
    }

    private void showQuizTab()
    {

    }
}

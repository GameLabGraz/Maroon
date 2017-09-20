


//EDITOR to add dialogues in the Unity Editor
//Makes a new editor: Window --> Language Editor

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LanguageEditor : EditorWindow
{
    private const float WINDOW_MIN_WIDTH = 800.0f;
    private const float WINDOW_MIN_HEIGHT = 600.0f;

    private LanguageManager m_manager;
    private int m_index; //Store reference to index of m_manager

    [MenuItem("Window/Language Editor")]
    //needs to be static
    public static void GetWindow()
    {
        LanguageEditor window = EditorWindow.GetWindow<LanguageEditor>("Language", true);
        window.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HEIGHT);
    }

    //If Window is Opened
    private void OnEnable()
    {
        //Load
        m_manager = LanguageManager.Load();
        m_index = 0;
    }

    //If Window is Closed
    private void OnDisable()
    {
        //Save
        m_manager.Save();
    }

    //Draw
    private void OnGUI()
    {
        //GUI STUFF
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        if (m_manager.Translations.Count > 0)
        {
            //UPDATE THIS IN CASE OF NEW LANGUAGES
            EditorGUILayout.LabelField(string.Format("Translation {0} Out of {1}", m_index + 1, m_manager.Translations.Count));
            m_manager.Translations[m_index].Key = EditorGUILayout.TextField("Key: ", m_manager.Translations[m_index].Key);
            m_manager.Translations[m_index].English = EditorGUILayout.TextField("English: ", m_manager.Translations[m_index].English);
            m_manager.Translations[m_index].German = EditorGUILayout.TextField("German: ", m_manager.Translations[m_index].German);
            m_manager.Translations[m_index].French = EditorGUILayout.TextField("French: ", m_manager.Translations[m_index].French);
            m_manager.Translations[m_index].Spain = EditorGUILayout.TextField("Spain: ", m_manager.Translations[m_index].Spain);
            m_manager.Translations[m_index].Italian = EditorGUILayout.TextField("Italian: ", m_manager.Translations[m_index].Italian);
        }
        else
        {
            EditorGUILayout.LabelField("No Dialogues yet");
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        //Add dialogue
        if (GUILayout.Button("New Dialogue"))
        {
            EditorGUI.FocusTextInControl(null);
            m_manager.Add(new Translation());
            //Go to end
            m_index = m_manager.Translations.Count - 1;
        }
        //Delete Dialogue
        if (GUILayout.Button("Delete Dialogue") && m_manager.Translations.Count > 0)
        {
            EditorGUI.FocusTextInControl(null);
            m_manager.RemoveIndex(m_index);
            m_index = Mathf.Clamp(m_index, 0, m_manager.Translations.Count - 1);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        if (GUILayout.Button("Previous") && m_manager.Translations.Count > 0)
        {
            EditorGUI.FocusTextInControl(null);
            m_index--;
            if (m_index < 0)
                m_index = 0;
        }
        if (GUILayout.Button("Next") && m_manager.Translations.Count > 0)
        {
            EditorGUI.FocusTextInControl(null);
            m_index++;
            if (m_index > m_manager.Translations.Count - 1)
                m_index = m_manager.Translations.Count - 1;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        if (GUILayout.Button("First") && m_manager.Translations.Count > 0)
        {
            EditorGUI.FocusTextInControl(null);
            m_index = 0;
        }
        if (GUILayout.Button("Last") && m_manager.Translations.Count > 0)
        {
            EditorGUI.FocusTextInControl(null);
            m_index = m_manager.Translations.Count - 1;
        }
        EditorGUILayout.EndHorizontal();
    }
}


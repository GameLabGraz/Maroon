using UnityEditor;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(PC_Dropdown), true)]
[CanEditMultipleObjects]
public class PC_Dropdown_EditorPanel : DropdownEditor
{
    SerializedProperty m_localizedKeys;
    SerializedProperty m_localizedOptions;

//    protected override void OnEnable()
    protected void OnEnable()
    {
        base.OnEnable();
        m_localizedKeys = serializedObject.FindProperty("m_options");
        m_localizedKeys = serializedObject.FindProperty("m_keys");
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_localizedKeys, new GUIContent("Localized Options"));
        serializedObject.ApplyModifiedProperties();
        
    }
}

[CustomPropertyDrawer(typeof(PC_Dropdown.LocalizedOptionDataList), true)]
class LocalizedDropdownOptionListDrawer : PropertyDrawer
{
    private ReorderableList m_ReorderableList;

    private void Init(SerializedProperty property)
    {
        if (m_ReorderableList != null)
            return;

        SerializedProperty array = property.FindPropertyRelative("m_OptionKeys");

        m_ReorderableList = new ReorderableList(property.serializedObject, array);
        m_ReorderableList.drawElementCallback = DrawOptionData;
        m_ReorderableList.drawHeaderCallback = DrawHeader;
        m_ReorderableList.elementHeight += 40;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init(property);

        m_ReorderableList.DoList(position);
    }

    private void DrawOptionData(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty itemData = m_ReorderableList.serializedProperty.GetArrayElementAtIndex(index);
        SerializedProperty itemText = itemData.FindPropertyRelative("m_Text");
        SerializedProperty itemKey = itemData.FindPropertyRelative("m_Key");
        SerializedProperty itemImage = itemData.FindPropertyRelative("m_Image");

        RectOffset offset = new RectOffset(0, 0, -1, -3);
        rect = offset.Add(rect);
        rect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(rect, itemKey, new GUIContent("Key", "The key of the localized resource."));
        rect.y += EditorGUIUtility.singleLineHeight + 3; //Padding
        EditorGUI.PropertyField(rect, itemText, new GUIContent("Text", "If the key is empty the text will be used."));
        rect.y += EditorGUIUtility.singleLineHeight + 3;
        EditorGUI.PropertyField(rect, itemImage, GUIContent.none);
    }

    private void DrawHeader(Rect rect)
    {
        GUI.Label(rect, "Localized Options");
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property);

        return m_ReorderableList.GetHeight();
    }
}

using PlatformControls.PC;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(PC_Slider), true)]
[CanEditMultipleObjects]
public class PC_Slider_Editor : SliderEditor
{
    SerializedProperty m_localizedEnableReset;

//    protected override void OnEnable()
    protected void OnEnable()
    {
        base.OnEnable();
        m_localizedEnableReset = serializedObject.FindProperty("resetEnabled");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_localizedEnableReset);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();

    }
}
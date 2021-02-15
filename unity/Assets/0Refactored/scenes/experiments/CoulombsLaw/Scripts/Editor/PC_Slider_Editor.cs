using PlatformControls.PC;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(PC_Slider), true)]
[CanEditMultipleObjects]
public class PC_Slider_Editor : SliderEditor
{
    SerializedProperty m_localizedEnableReset;
    SerializedProperty m_OnStartDrag;
    SerializedProperty m_OnEndDrag;
    SerializedProperty m_OnSetSliderValueViaInput;

//    protected override void OnEnable()
    protected void OnEnable()
    {
        base.OnEnable();
        m_localizedEnableReset = serializedObject.FindProperty("resetEnabled");
        m_OnStartDrag = serializedObject.FindProperty("onStartDrag");
        m_OnEndDrag = serializedObject.FindProperty("onEndDrag");
        m_OnSetSliderValueViaInput = serializedObject.FindProperty("onSetSliderValueViaInput");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_localizedEnableReset);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(m_OnStartDrag);
        EditorGUILayout.PropertyField(m_OnEndDrag);
        EditorGUILayout.PropertyField(m_OnSetSliderValueViaInput);
        serializedObject.ApplyModifiedProperties();
    }
}
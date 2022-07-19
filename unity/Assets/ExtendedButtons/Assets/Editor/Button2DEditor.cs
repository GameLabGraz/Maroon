#region UNITY_EDITOR

using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace ExtendedButtons.Editor
{
    [CustomEditor(typeof(Button2D))]
    public class Button2DEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Button2D targetButton = (Button2D)target;

            SerializedProperty property = serializedObject.FindProperty("onEnter");
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);

            property = serializedObject.FindProperty("onDown");
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);

            property = serializedObject.FindProperty("onUp");
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);

            property = serializedObject.FindProperty("onExit");
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endregion
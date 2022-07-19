#region UNITY_EDITOR

using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace ExtendedButtons.Editor
{
    [CustomEditor(typeof(Button2DExtended))]
    public class Button2DExtendedEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Button2DExtended targetButton = (Button2DExtended)target;

            SerializedProperty property = serializedObject.FindProperty("onEnter");
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);

            property = serializedObject.FindProperty("onDown");
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);

            property = serializedObject.FindProperty("onUp");
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);

            property = serializedObject.FindProperty("onExit");
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);

            property = serializedObject.FindProperty("onBeginDrag");
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);

            property = serializedObject.FindProperty("onDrag");
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);

            property = serializedObject.FindProperty("onEndDrag");
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endregion
using TMPro.EditorUtilities;
using UnityEditor;

namespace Maroon.UI.Editor
{
    [CustomEditor(typeof(InputField))]
    public class InputFieldEditor : TMP_InputFieldEditor
    {
        private SerializedProperty _allowResetProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _allowResetProperty = serializedObject.FindProperty("allowReset");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_allowResetProperty);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
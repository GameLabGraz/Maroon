using UnityEditor;

namespace Maroon.UI.Editor
{
    [CustomEditor(typeof(Slider))]
    public class SliderEditor : UnityEditor.UI.SliderEditor
    {
        private SerializedProperty _allowResetProperty;
        private SerializedProperty _onSliderInitProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _allowResetProperty = serializedObject.FindProperty("allowReset");
            _onSliderInitProperty = serializedObject.FindProperty("onSliderInit");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_allowResetProperty);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(_onSliderInitProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

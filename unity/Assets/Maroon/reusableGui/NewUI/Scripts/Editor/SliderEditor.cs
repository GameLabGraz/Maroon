using UnityEditor;

namespace Maroon.UI.Editor
{
    [CustomEditor(typeof(Slider))]
    public class SliderEditor : UnityEditor.UI.SliderEditor
    {
        private SerializedProperty _allowResetProperty;
        private SerializedProperty _onSliderInitProperty;
        private SerializedProperty _onStartDragProperty;
        private SerializedProperty _onEndDragProperty;
        private SerializedProperty _onSetSliderValueProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _allowResetProperty = serializedObject.FindProperty("allowReset");
            _onSliderInitProperty = serializedObject.FindProperty("onSliderInit");
            _onStartDragProperty = serializedObject.FindProperty("onStartDrag");
            _onEndDragProperty = serializedObject.FindProperty("onEndDrag");
            _onSetSliderValueProperty = serializedObject.FindProperty("onSetSliderValueViaInput");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_allowResetProperty);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(_onSliderInitProperty);
            EditorGUILayout.PropertyField(_onStartDragProperty);
            EditorGUILayout.PropertyField(_onEndDragProperty);
            EditorGUILayout.PropertyField(_onSetSliderValueProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

using UnityEditor;

namespace Maroon.Physics.CoordinateSystem.Editor
{
    [CustomEditor(typeof(AxisController))]
    public class AxisControllerEditor : UnityEditor.Editor
    {
        private AxisController _manager;
        private SerializedProperty _enableNegativeDirection;
        private SerializedProperty _enableThirdDimension;
        private SerializedProperty _markerFontSize;
        private SerializedProperty _isWorldLengthUniform;
        private SerializedProperty _uniformWorldAxisLength;
        private SerializedProperty _axisList;
        private SerializedProperty _enableVisualIndicator;
        private SerializedProperty _spaceIndicator;

        private void Awake()
        {
            _manager = (AxisController)target;
        }

        private void OnEnable()
        {
            _axisList = serializedObject.FindProperty("_axisList");
            _markerFontSize = serializedObject.FindProperty("_markerFontSize");
            _enableThirdDimension = serializedObject.FindProperty("_enableThirdDimension");
            _isWorldLengthUniform = serializedObject.FindProperty("_lengthUniform");
            _uniformWorldAxisLength = serializedObject.FindProperty("_uniformWorldAxisLength");
            _enableNegativeDirection = serializedObject.FindProperty("_enableNegativeDirection");

            _enableVisualIndicator = serializedObject.FindProperty("_enableVisualIndicator");
            _spaceIndicator = serializedObject.FindProperty("_spaceIndicator");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_enableNegativeDirection);
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                _manager.ToggleNegativeAxisVisibility();
                _manager.ToggleSpaceIndicatorVisibility();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_enableThirdDimension);
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                _manager.ToggleThirdDimension();
                _manager.ToggleSpaceIndicatorVisibility();
            }

            EditorGUILayout.Separator();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_enableVisualIndicator);
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                _manager.ToggleSpaceIndicatorVisibility();
            }

            EditorGUILayout.PropertyField(_spaceIndicator);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Uniformity", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_markerFontSize);
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                _manager.UpdateAxisFontSize();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_isWorldLengthUniform);
            serializedObject.ApplyModifiedProperties();
            if (_isWorldLengthUniform.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.Slider(_uniformWorldAxisLength, 0f, 100f);
                serializedObject.ApplyModifiedProperties();

                if (EditorGUI.EndChangeCheck())
                {
                    _manager.SetAxisWorldLengthUniform();
                }

                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck())
            {
                _manager.UpdateAxisFontSize();
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Axis", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_axisList);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
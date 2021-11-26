using UnityEditor;

namespace Maroon.Physics.CoordinateSystem.Editor
{
    [CustomEditor(typeof(CoordAxis))]
    public class CoordAxisEditor : UnityEditor.Editor
    {
        private CoordAxis axis;
        private SerializedProperty _worldLength;
        private SerializedProperty _localLength;
        private SerializedProperty _axisSubdivision;
        private SerializedProperty _prefab;
        private SerializedProperty _id;
        private SerializedProperty _subdivisionUnit;
        private SerializedProperty _localLengthUnit;

        private void Awake()
        {
            axis = (CoordAxis)target;
        }

        private void OnEnable()
        {
            _id = serializedObject.FindProperty("_axisID");
            _prefab = serializedObject.FindProperty("_axisMarkerPrefab");
            _localLengthUnit = serializedObject.FindProperty("_lengthUnit");
            _worldLength = serializedObject.FindProperty("_axisWorldLength");
            _localLength = serializedObject.FindProperty("_axisLocalLength");
            _subdivisionUnit = serializedObject.FindProperty("_divisionUnit");
            _axisSubdivision = serializedObject.FindProperty("_axisSubdivision");

            axis.SetupAxis();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_id);
            EditorGUILayout.PropertyField(_prefab);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_worldLength);
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                axis.SetupAxis();
            }

            EditorGUILayout.LabelField("Local Axis Length/Unit", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_localLength);
            EditorGUILayout.PropertyField(_localLengthUnit);
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                axis.SetupAxis();
            }

            EditorGUILayout.LabelField("Subdivision of Axis", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_axisSubdivision);
            EditorGUILayout.PropertyField(_subdivisionUnit);
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                axis.SetupAxis();
            }
        }
    }
}
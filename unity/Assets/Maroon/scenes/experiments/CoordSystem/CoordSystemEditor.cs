using UnityEditor;

[CustomEditor(typeof(CoordSystemManager))]
public class CoordSystemManagerEditor : Editor
{
    private CoordSystemManager _manager;
    private SerializedProperty _enableNegativeDirection;
    private SerializedProperty _enableThirdDimension;
    private SerializedProperty _markerFontSize;
    private SerializedProperty _isWorldLengthUniform;
    private SerializedProperty _uniformWorldAxisLength;
    private SerializedProperty _axisList;

    private void Awake()
    {
        _manager = (CoordSystemManager)target;
    }

    private void OnEnable()
    {
        _axisList = serializedObject.FindProperty("_axisList");
        _markerFontSize = serializedObject.FindProperty("_markerFontSize");
        _enableThirdDimension = serializedObject.FindProperty("_enableThirdDimension");
        _isWorldLengthUniform = serializedObject.FindProperty("_lengthUniform");
        _uniformWorldAxisLength = serializedObject.FindProperty("_uniformWorldAxisLength");
        _enableNegativeDirection = serializedObject.FindProperty("_enableNegativeDirection");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

       /* EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_enableNegativeDirection);
        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
        {
            
        }*/

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_enableThirdDimension);
        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
        {
            _manager.ToggleThirdDimension(_enableThirdDimension.boolValue);
        }


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
            EditorGUILayout.PropertyField(_uniformWorldAxisLength);
            serializedObject.ApplyModifiedProperties();
            
            if (EditorGUI.EndChangeCheck())
            {
                _manager.SetAxisWorldLengthUniform(_uniformWorldAxisLength.floatValue);
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
using UnityEditor;

[CustomEditor(typeof(WaterPlane))]
public class WaterPlaneEditor : Editor
{
    private WaterPlane waterPlane;

    private SerializedProperty waveGenerators;

    private SerializedProperty material;

    private SerializedProperty verticesPerLength;
    private SerializedProperty verticesPerWidth;

    private void Awake()
    {
        waterPlane = (WaterPlane)target;
    }

    private void OnEnable()
    {
        waveGenerators = serializedObject.FindProperty("waveGenerators");

        material = serializedObject.FindProperty("material");

        verticesPerLength = serializedObject.FindProperty("verticesPerLength");
        verticesPerWidth = serializedObject.FindProperty("verticesPerWidth");
    }

    public override void OnInspectorGUI()
    {
        //--------------------------------------------------------------------------
        // Check if Material has changed
        //--------------------------------------------------------------------------
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(material);

        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
            waterPlane.UpdateMaterial();

        //--------------------------------------------------------------------------
        // Check if Plane Dimensions has changed
        //--------------------------------------------------------------------------
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(verticesPerLength);
        EditorGUILayout.PropertyField(verticesPerWidth);

        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
            waterPlane.UpdatePlane();

        EditorGUILayout.PropertyField(waveGenerators);
        EditorGUI.indentLevel++;
        if (waveGenerators.isExpanded)
        {
            EditorGUILayout.PropertyField(waveGenerators.FindPropertyRelative("Array.size"));
            for (int i = 0; i < waveGenerators.arraySize; i++)
                EditorGUILayout.PropertyField(waveGenerators.GetArrayElementAtIndex(i));
        }
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
using UnityEditor;

[CustomEditor(typeof(PointWaveWaterPlane))]
public class PointWaveWaterPlaneEditor : Editor
{
    private PointWaveWaterPlane waterPlane;

    private SerializedProperty verticesPerLength;
    private SerializedProperty verticesPerWidth;

    private void Awake()
    {
        waterPlane = (PointWaveWaterPlane)target;
    }

    private void OnEnable()
    {
        verticesPerLength = serializedObject.FindProperty("verticesPerLength");
        verticesPerWidth = serializedObject.FindProperty("verticesPerWidth");
    }

    public override void OnInspectorGUI()
    {
        //--------------------------------------------------------------------------
        // Check if Plane Dimensions has changed
        //--------------------------------------------------------------------------
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(verticesPerLength);
        EditorGUILayout.PropertyField(verticesPerWidth);

        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
            waterPlane.CalculatePlaneMesh();

        serializedObject.ApplyModifiedProperties();
    }
}
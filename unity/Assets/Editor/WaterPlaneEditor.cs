using UnityEditor;

[CustomEditor(typeof(WaterPlane))]
public class WaterPlaneEditor : Editor
{
    private WaterPlane waterPlane;

    private SerializedProperty material;

    private SerializedProperty verticesPerLength;
    private SerializedProperty verticesPerWidth;

    private SerializedProperty updateRate;

    private void Awake()
    {
        waterPlane = (WaterPlane)target;
    }

    private void OnEnable()
    {
        material = serializedObject.FindProperty("material");

        verticesPerLength = serializedObject.FindProperty("verticesPerLength");
        verticesPerWidth = serializedObject.FindProperty("verticesPerWidth");

        updateRate = serializedObject.FindProperty("updateRate");
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

        serializedObject.ApplyModifiedProperties();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(updateRate);

        serializedObject.ApplyModifiedProperties();

    }
}
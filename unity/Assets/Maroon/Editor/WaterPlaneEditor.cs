using UnityEditor;

[CustomEditor(typeof(WaterPlane))]
public class WaterPlaneEditor : Editor
{
    private WaterPlane waterPlane;

    private SerializedProperty material;

    private SerializedProperty verticesPerLength;
    private SerializedProperty verticesPerWidth;

    private SerializedProperty slitPlate;
    private SerializedProperty waterBasinGeneratorPosition;
    private SerializedProperty useMaterialReset;

    private void Awake()
    {
        waterPlane = (WaterPlane)target;
    }

    private void OnEnable()
    {
        material = serializedObject.FindProperty("material");

        verticesPerLength = serializedObject.FindProperty("verticesPerLength");
        verticesPerWidth = serializedObject.FindProperty("verticesPerWidth");

        slitPlate = serializedObject.FindProperty("slitPlate");
        waterBasinGeneratorPosition = serializedObject.FindProperty("waterBasinGeneratorPosition");
        useMaterialReset = serializedObject.FindProperty("useMaterialReset");
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
            waterPlane.CalculatePlaneMesh();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.PropertyField(slitPlate);
        EditorGUILayout.PropertyField(waterBasinGeneratorPosition);
        EditorGUILayout.PropertyField(useMaterialReset);
        serializedObject.ApplyModifiedProperties();
    }
}
using UnityEditor;

[CustomEditor(typeof(Chain))]
public class ChainEditor : Editor
{
    private Chain chain;

    private SerializedProperty chainAxis;

    // Link Properties
    private SerializedProperty linkScale;
    private SerializedProperty linkOffset;
    private SerializedProperty linkCount;
    private SerializedProperty customLinkObject;

    // Joint Properties
    private SerializedProperty jointAxis;
    private SerializedProperty jointSwingAxis;
    private SerializedProperty jointSwingLimit;

    private void Awake()
    {
        chain = (Chain)target;
    }

    private void OnEnable()
    {
        chainAxis = serializedObject.FindProperty("chainAxis");

        // Find Link Proberties
        linkScale = serializedObject.FindProperty("linkScale");
        linkOffset = serializedObject.FindProperty("linkOffset");
        linkCount = serializedObject.FindProperty("linkCount");
        customLinkObject = serializedObject.FindProperty("customLinkObject");

        // Find Joint Proberties
        jointAxis = serializedObject.FindProperty("jointAxis");
        jointSwingAxis = serializedObject.FindProperty("jointSwingAxis");
        jointSwingLimit = serializedObject.FindProperty("jointSwingLimit");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //--------------------------------------------------------------------------
        // Check if Chain Axis has changed
        //--------------------------------------------------------------------------
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(chainAxis);

        //serializedObject.ApplyModifiedProperties();
        //if (EditorGUI.EndChangeCheck())
          //  Debug.Log("Chain Axis changed");

        //--------------------------------------------------------------------------
        EditorGUILayout.Space();

        //--------------------------------------------------------------------------
        // Check if Link Settings has changed
        //--------------------------------------------------------------------------
        EditorGUILayout.LabelField("Link settings", EditorStyles.boldLabel);

        //EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(linkScale);
        EditorGUILayout.PropertyField(linkOffset);
        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
            chain.UpdateSettings();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(linkCount);       
        if (linkCount.intValue < 0)
            linkCount.intValue = 0;
        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
            chain.UpdateNumberOfLinks();
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(customLinkObject);
        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
            chain.UpdateCustomLinkObjects();

        //--------------------------------------------------------------------------

        EditorGUILayout.Space();

        //--------------------------------------------------------------------------
        // Check if Joint Settings has changed
        //--------------------------------------------------------------------------
        EditorGUILayout.LabelField("Joint settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(jointAxis);
        EditorGUILayout.PropertyField(jointSwingAxis);
        EditorGUILayout.PropertyField(jointSwingLimit);

        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
            chain.UpdateSettings();

        //--------------------------------------------------------------------------

    }
}

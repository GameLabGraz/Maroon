using System;
using UnityEditor;

[CustomEditor(typeof(VRButtonController))]
public class VRButtonEditor : Editor
{
    private int selectedToggleValue = 0;
    private string[] toogleValueOptions = { "true", "false" };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VRButtonController VRButtonTarget = target as VRButtonController;

        if (VRButtonTarget.IsToogleButton)
        {
            selectedToggleValue = EditorGUILayout.Popup("Toggle Value", selectedToggleValue, toogleValueOptions);
            VRButtonTarget.ToogleValue = Boolean.Parse(toogleValueOptions[selectedToggleValue]);
        }
    }
}
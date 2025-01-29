using log4net.Util;
using UnityEditor;
using UnityEngine;

namespace Maroon.Physics.Electromagnetism.Editor
{
    // Based on https://discussions.unity.com/t/drawing-position-handle-from-scriptableobject-editor/195416/3
    [CustomEditor(typeof(FieldLine))]
    public class FieldLineEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            FieldLine fieldLine = (FieldLine)target;

            Vector3 originPosition = fieldLine.transform.TransformPoint(Vector3.zero - fieldLine.originOffset);
            originPosition = Handles.PositionHandle(originPosition, Quaternion.identity);

            if (GUI.changed)
            {
                Undo.RecordObject(target, "Move originOffset");
                Vector3 inverseOriginOffset = Vector3.zero - fieldLine.transform.InverseTransformPoint(originPosition);
                fieldLine.originOffset = inverseOriginOffset;
            }
        }
    }
}

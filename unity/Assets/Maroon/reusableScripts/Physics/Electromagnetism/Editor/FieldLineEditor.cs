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

            Vector3 originOffset = (fieldLine.originOffset);
            originOffset.Scale(fieldLine.transform.lossyScale);
            originOffset += fieldLine.transform.position;
            originOffset = Handles.PositionHandle(originOffset, Quaternion.identity);

            if (GUI.changed)
            {
                Undo.RecordObject(target, "Move originOffset");
                Vector3 newOriginOffset = (originOffset - fieldLine.transform.position);
                newOriginOffset.Scale(new Vector3(1f / fieldLine.transform.lossyScale.x, 1f / fieldLine.transform.lossyScale.y, 1f / fieldLine.transform.lossyScale.z));
                fieldLine.originOffset = newOriginOffset;
            }
        }
    }
}

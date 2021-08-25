using UnityEditor;

namespace Maroon.UI.Editor
{
    [CustomEditor(typeof(QuantityPropertyView))]
    public class QuantityPropertyViewEditor : UnityEditor.Editor
    {
        private QuantityPropertyView _propertyView;

        private void OnEnable()
        {
            _propertyView = (QuantityPropertyView)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();

            if (EditorGUI.EndChangeCheck())
            {
                _propertyView.ShowUI();
            }
        }
    }
}
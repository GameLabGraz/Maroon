using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    [RequireComponent(typeof(GUILabelLogic))]
    public class GUIVector3InputLogic : MonoBehaviour
    {
        [SerializeField] private Vector3   value = Vector3.zero;
        [SerializeField] private bool      isInteractable = true;
        [SerializeField] private string    nameLocalizationKey = "";
        [SerializeField] private string    unitName = "";
        [SerializeField] private SI_Prefix prefix = SI_Prefix.NONE;
        [SerializeField] private int       postCommaDigits = 2;

        [SerializeField] private bool clampValuesToSimulationBox = true;
        [SerializeField] private bool displayRelativeToSimulationBox = false;

        // If this is set, the text-inputs will track the objects position (Queried once every LateUpdate)
        private Transform trackedTransform;
        // This Event is only triggered on text-field edits, not when the tracked/affected object moves
        public UnityEngine.Events.UnityEvent<Vector3> OnEndEdit;

        // UI-References
        private TMPro.TMP_InputField[] inputFields = new TMPro.TMP_InputField[3];

        private void UpdateTextFieldValue(int dimension)
        {
            var inputField = inputFields[dimension];
            if (inputField == null) return;

            Vector3 offset = displayRelativeToSimulationBox ? SimulationBox.Instance.Bounds.center : Vector3.zero;
            float prefixFactor = (new SI_Prefix_Info(prefix)).factor;
            float displayValue = (value[dimension] - offset[dimension]) / prefixFactor;

            // There surely is a better way to specify float precision in toString...
            string formatString = "0";
            if (postCommaDigits > 0)
            {
                formatString += ".";
                for (int i = 0; i < postCommaDigits; i++)
                {
                    formatString += "0";
                }
            }
            string newText = displayValue.ToString(formatString);
            if (inputField.text != newText && !inputField.isFocused) // We test if we change text so it still works while input is being edited
            {
                inputField.text = newText;
            }
        }

        private void OnTextFieldEndEdit(string text, int dimension)
        {
            float displayValue = 0.0f;
            bool parseSuccess = float.TryParse(text, out displayValue);
            if (!parseSuccess)
            {
                UpdateTextFieldValue(dimension);
                return;
            }

            Vector3 offset = displayRelativeToSimulationBox ? SimulationBox.Instance.Bounds.center : Vector3.zero;
            float prefixFactor = (new SI_Prefix_Info(prefix)).factor;
            float newValue = displayValue * prefixFactor + offset[dimension];

            if (clampValuesToSimulationBox)
            {
                var box = SimulationBox.Instance.Bounds;
                newValue = Mathf.Clamp(newValue, box.min[dimension], box.max[dimension]);
            }

            value[dimension] = newValue;
            if (trackedTransform != null)
            {
                var pos = trackedTransform.position;
                pos[dimension] = newValue;
                trackedTransform.position = pos;
                value = trackedTransform.position; // Keep value synchronous with tracked transform
            }
            UpdateTextFieldValue(dimension);
            OnEndEdit.Invoke(value);
        }

        private void Start() 
        {
            GetComponent<GUILabelLogic>().SetLabelData(nameLocalizationKey, unitName, prefix);

            var contentObject = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "Content").gameObject;
            inputFields[0]    = GUISubwindowLogic.FindChildObjectByNameRecursive(contentObject, "XInputField").GetComponent<TMPro.TMP_InputField>();
            inputFields[1]    = GUISubwindowLogic.FindChildObjectByNameRecursive(contentObject, "YInputField").GetComponent<TMPro.TMP_InputField>();
            inputFields[2]    = GUISubwindowLogic.FindChildObjectByNameRecursive(contentObject, "ZInputField").GetComponent<TMPro.TMP_InputField>();

            inputFields[0].onEndEdit.AddListener((string value) => { OnTextFieldEndEdit(value, 0); });
            inputFields[0].readOnly = !isInteractable;
            inputFields[1].onEndEdit.AddListener((string value) => { OnTextFieldEndEdit(value, 1); });
            inputFields[1].readOnly = !isInteractable;
            inputFields[2].onEndEdit.AddListener((string value) => { OnTextFieldEndEdit(value, 2); });
            inputFields[2].readOnly = !isInteractable;
        }

        private void LateUpdate()
        {
            // Update text fields
            if (trackedTransform == null) return;
            value = trackedTransform.position;
            UpdateTextFieldValue(0);
            UpdateTextFieldValue(1);
            UpdateTextFieldValue(2);
        }

        public Vector3 GetValue()
        {
            if (trackedTransform != null) return trackedTransform.transform.position;
            return value;
        }

        public void SetValue(Vector3 newValue)
        {
            value = newValue;
            if (trackedTransform != null)
            {
                trackedTransform.transform.position = value;
            }

            UpdateTextFieldValue(0);
            UpdateTextFieldValue(1);
            UpdateTextFieldValue(2);
        }

        public void TrackTransform(Transform transform)
        {
            trackedTransform = transform;
            SetValue(transform.position);
        }
    }
}

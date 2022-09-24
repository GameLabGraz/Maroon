using GEAR.Localization.Text;
using Maroon.GlobalEntities;
using Maroon.Physics.CoordinateSystem;
using PlatformControls.PC;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.UI
{
    public class PC_ObjectSelectionHandler : MonoBehaviour 
    {
        [SerializeField] private GameObject inputXVariable;
        [SerializeField] private GameObject inputYVariable;
        [SerializeField] private GameObject inputZVariable;

        [SerializeField] private PC_Slider coulombValueSlider;
        [SerializeField] private GameObject particlePrefab;

        [SerializeField] private GameObject SourceButtonAddDelete;
        [SerializeField] private GameObject SourceButtonText;

        [Header("Others")]
        [SerializeField] private PC_ObjectSelection selectedObject = null;
        [SerializeField] private ParticleManager _particleManager;

        private PC_InputParser_Float_TMP _xVariableInputParser; 
        private PC_InputParser_Float_TMP _yVariableInputParser;
        private PC_InputParser_Float_TMP _zVariableInputParser;

        void Start()
        {
            AdaptButtonTextCharge();

            _xVariableInputParser = inputXVariable.GetComponent<PC_InputParser_Float_TMP>();
            _yVariableInputParser = inputYVariable.GetComponent<PC_InputParser_Float_TMP>();
            _zVariableInputParser = inputZVariable.GetComponent<PC_InputParser_Float_TMP>();

            _xVariableInputParser?.onValueChangedFloat.AddListener((endVal) =>
            {
                if (IsSelectedObjectOfType(PC_ObjectSelection.SelectObjectType.SourceSelect))
                    MoveToNewPosition(new Vector3(endVal, _yVariableInputParser.GetValue(), _zVariableInputParser.GetValue()));
            });

            _yVariableInputParser?.onValueChangedFloat.AddListener((endVal) =>
            {
                if (IsSelectedObjectOfType(PC_ObjectSelection.SelectObjectType.SourceSelect))
                    MoveToNewPosition(new Vector3(_xVariableInputParser.GetValue(), endVal , _zVariableInputParser.GetValue()));
            });

            _yVariableInputParser?.onValueChangedFloat.AddListener((endVal) =>
            {
                if (IsSelectedObjectOfType(PC_ObjectSelection.SelectObjectType.SourceSelect))
                    MoveToNewPosition(new Vector3(_xVariableInputParser.GetValue(), _yVariableInputParser.GetValue(), endVal));
            });

            coulombValueSlider.onValueChanged.AddListener((newValue) =>
            {
                if (!IsSelectedObjectOfType(PC_ObjectSelection.SelectObjectType.SourceSelect)) return;
           
                selectedObject.GetComponent<global::Maroon.Physics.Electromagnetism.Charge>().strength = newValue;
                _particleManager.ChangeColorOfParticle(selectedObject.gameObject, newValue);

            });

            SourceButtonAddDelete.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (selectedObject != null && selectedObject.type == PC_ObjectSelection.SelectObjectType.SourceSelect)
                {
                    DeleteSelectedCharge();
                }
                else
                {
                    _particleManager.CreateSource(particlePrefab, CoordSystemHandler.Instance.GetWorldPosition(new Vector3(_xVariableInputParser.GetValue(),
                        _yVariableInputParser.GetValue(), _zVariableInputParser.GetValue())), coulombValueSlider.value);
               
                    AdaptButtonTextCharge();
                }
            });
        }

        private void DeleteSelectedCharge()
        {
            var dyingObject = selectedObject.gameObject;
            SelectObject(null);
        
            _particleManager.RemoveSourceFromEField(dyingObject);
            Destroy(dyingObject);
            AdaptButtonTextCharge();
        }


        private void AdaptButtonTextCharge()
        {
            if (SourceButtonText == null || SourceButtonText.GetComponent<LocalizedTMP>() == null) return;
            if (!SourceButtonText) return;
            SourceButtonText.GetComponent<LocalizedTMP>().Key = 
                selectedObject == null || selectedObject.type != PC_ObjectSelection.SelectObjectType.SourceSelect
                    ? "Add Charge"
                    : "Delete Charge";
        }

        public void SelectObject(PC_ObjectSelection obj)
        {
            if (selectedObject == obj) return;
       
            selectedObject = obj;
            if (selectedObject != null)
            {
                AdaptButtonTextCharge();
                UpdatePositionText();
                UpdateChargeValueText();
            }
        }

        private void UpdatePositionText()
        {
            var selectedObjectSystemPosition = CoordSystemHandler.Instance.GetSystemPosition(selectedObject.transform.position, Unit.cm);

            _xVariableInputParser.SetValue(selectedObjectSystemPosition.x);
            _yVariableInputParser.SetValue(selectedObjectSystemPosition.y);
            _zVariableInputParser.SetValue(selectedObjectSystemPosition.z);
        }

        private void UpdateChargeValueText()
        {
            coulombValueSlider.value = selectedObject.GetComponent<global::Maroon.Physics.Electromagnetism.Charge>().strength;
        }

        public void DeselectAll()
        {
            selectedObject = null;
            AdaptButtonTextCharge();
        }

        private void MoveToNewPosition(Vector3 transform)
        {
            if (!selectedObject) return;
            selectedObject.transform.position = CoordSystemHandler.Instance.GetWorldPosition(transform);
        }

        private bool IsSelectedObjectOfType(PC_ObjectSelection.SelectObjectType type) => selectedObject != null && selectedObject.type == type;
    }
}

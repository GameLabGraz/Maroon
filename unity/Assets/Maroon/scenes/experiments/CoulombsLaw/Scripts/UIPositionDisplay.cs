using Maroon.GlobalEntities;
using Maroon.Physics.CoordinateSystem;
using UnityEngine;

public class UIPositionDisplay : MonoBehaviour
{
    [SerializeField] public Transform affectedObject = null;
    [SerializeField] private PC_InputParser_Float_TMP xCoordinateInput;
    [SerializeField] private PC_InputParser_Float_TMP yCoordinateInput;
    [SerializeField] private PC_InputParser_Float_TMP zCoordinateInput;

    private void OnCoordinateTextChanged(float value, int dimension)
    {
        if (affectedObject == null) return;
        var newPos = CoordSystemHandler.Instance.GetSystemPosition(affectedObject.position, Unit.m);
        newPos[dimension] = value;
        newPos = CoordSystem.Instance.GetPositionInWorldSpace(newPos);
        affectedObject.transform.position = newPos;
    }

    private void Awake()
    {
        // Register UI callbacks
        xCoordinateInput?.onValueChangedFloat.AddListener((value) => OnCoordinateTextChanged(value, 0));
        yCoordinateInput?.onValueChangedFloat.AddListener((value) => OnCoordinateTextChanged(value, 1));
        zCoordinateInput?.onValueChangedFloat.AddListener((value) => OnCoordinateTextChanged(value, 2));
    }

    private void LateUpdate()
    {
        // Update text fields
        if (affectedObject == null) return;
        var systemPos = CoordSystemHandler.Instance.GetSystemPosition(affectedObject.position, Unit.m);
        // Note(MartinR): SetValue checks if the new value is different from the current textfield-value, so it
        //  doesn't cause any problems while the text-field is edited even if we call SetValue each frame
        xCoordinateInput.SetValue(systemPos.x);
        yCoordinateInput.SetValue(systemPos.y);
        zCoordinateInput.SetValue(systemPos.z);
    }
}

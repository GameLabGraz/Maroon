using System;
using UnityEngine;

public class ViewChangeHandler : MonoBehaviour
{
    [Header("Information Components")]
    public PathHandler pathHandler;

    [Header("Affected Objects")] 
    public GameObject VisualizationPlane;
    public GameObject VisualizationPlaneOutline;
    public GameObject VisualizationCube;
    public GameObject VisualizationCubeOutline;
    public GameObject VoltmeterPositive;
    public GameObject VoltmeterNegative;

    private CoulombLogic _coulombLogic;
    private PathHandler.CameraPosition _lastView = PathHandler.CameraPosition.CP_Free;
    
    // Start is called before the first frame update
    void Start()
    {
        _coulombLogic = GetComponent<CoulombLogic>();
        Debug.Assert(_coulombLogic, "Coulomb Logic was not found!");
    }

    public void ChangeView(int posIdx)
    {
        ChangeView((PathHandler.CameraPosition) posIdx);
    }
    
    public void ChangeView(PathHandler.CameraPosition newPosition)
    {
        if(_lastView == newPosition) return;
        
        var allowX = newPosition != PathHandler.CameraPosition.CP_Side;
        var allowY = newPosition != PathHandler.CameraPosition.CP_Top;
        var allowZ = newPosition != PathHandler.CameraPosition.CP_Front;
        foreach (var charge in _coulombLogic.GetCharges())
            HandleMovementRestrictions(charge.gameObject, allowX, allowY, allowZ);
        
        HandleMovementRestrictions(VoltmeterPositive, allowX, allowY, allowZ);
        HandleMovementRestrictions(VoltmeterNegative, allowX, allowY, allowZ);
        
        VisualizationCube.transform.rotation = Quaternion.identity;
        VisualizationCube.GetComponent<PC_Rotation>().enabled = newPosition == PathHandler.CameraPosition.CP_Free;
        VisualizationCubeOutline.GetComponent<PC_Rotation>().enabled =
            newPosition == PathHandler.CameraPosition.CP_Free;
        
        VisualizationPlane.transform.rotation = Quaternion.Euler(newPosition == PathHandler.CameraPosition.CP_Top? 90f : 0f, newPosition == PathHandler.CameraPosition.CP_Side? 90f : 0f, 0f);
        VisualizationPlane.transform.GetChild(2).gameObject.SetActive(newPosition == PathHandler.CameraPosition.CP_Free);
        
        //Get All Childs from the visPlane_child[2] and disable their rotation scripts
        for (var i = 0; i < VisualizationPlaneOutline.transform.childCount; ++i)
        {
            var component = VisualizationPlaneOutline.transform.GetChild(i).GetComponent<PC_Rotation>();
            if (component)
                component.enabled = newPosition == PathHandler.CameraPosition.CP_Free;
        }
        
        _lastView = newPosition;
    }

    private void HandleMovementRestrictions(GameObject obj, bool allowX = true, bool allowY = true, bool allowZ = true)
    {
        var dragHandler = obj.GetComponent<PC_DragHandler>();
        if (!dragHandler) return;
        dragHandler.RestrictMovement(allowX, allowY, allowZ);
        if (!dragHandler.ArrowMovement) return;
        dragHandler.ArrowMovement.UpdateMovementRestriction(!allowX, !allowY, !allowZ);
//        Debug.Log("SetUp Restrictions " + charge.name + " --- " + allowX + " - " + allowY + " - " + allowZ);
    }
    
    public void SetupViewSettingsForNewParticles(CoulombChargeBehaviour newCharge)
    {
        if (_coulombLogic.IsIn2dMode()) return;
        HandleMovementRestrictions(newCharge.gameObject, 
            pathHandler.GetCurrentPosition() != PathHandler.CameraPosition.CP_Side,
            pathHandler.GetCurrentPosition() != PathHandler.CameraPosition.CP_Top,
            pathHandler.GetCurrentPosition() != PathHandler.CameraPosition.CP_Front);
    }
}
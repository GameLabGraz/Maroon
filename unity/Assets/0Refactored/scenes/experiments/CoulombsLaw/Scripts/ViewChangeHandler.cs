using Maroon.Physics;
using UnityEngine;
using UnityEngine.Events;


public class ViewChangeHandler : MonoBehaviour
{
    public const int DefaultView = 0;
    public const int FrontView = 1;
    public const int SideView = 2;
    public const int TopView = 3;
    
    [Header("Information Components")]
    public PathHandler pathHandler;

    [Header("Affected Objects")] 
    public GameObject VisualizationPlane;
    public PC_ArrowMovement VisualizationPlaneMovingArrows;
    public GameObject VisualizationPlaneOutline;
    public GameObject VisualizationCube;
    public GameObject VisualizationCubeOutline;
    public GameObject VoltmeterPositive;
    public GameObject VoltmeterNegative;
    public RulerPrefab Ruler;

    [Header("Assessment System")] 
    public QuantityInt currentView = 0;

    public UnityEvent onViewChanged;
    
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
        HandleMovementRestrictions(Ruler.RulerStart, allowX, allowY, allowZ);
        HandleMovementRestrictions(Ruler.RulerEnd, allowX, allowY, allowZ);
        
        VisualizationCube.transform.rotation = Quaternion.identity;
        
        /*
        VisualizationCube.GetComponent<PC_Rotation>().enabled = newPosition == PathHandler.CameraPosition.CP_Free;
        VisualizationCubeOutline.GetComponent<PC_Rotation>().enabled =
            newPosition == PathHandler.CameraPosition.CP_Free;
        */
        
        VisualizationPlane.transform.localPosition = Vector3.zero;
        VisualizationPlane.transform.rotation = Quaternion.Euler(newPosition == PathHandler.CameraPosition.CP_Top? 90f : 0f, newPosition == PathHandler.CameraPosition.CP_Side? 90f : 0f, 0f);
        VisualizationPlaneMovingArrows.gameObject.SetActive(newPosition == PathHandler.CameraPosition.CP_Free);
        
        //Get All Childs from the visPlane_child[2] and disable their rotation scripts
        for (var i = 0; i < VisualizationPlaneOutline.transform.childCount; ++i)
        {
            var component = VisualizationPlaneOutline.transform.GetChild(i).GetComponent<PC_Rotation>();
            if (component)
                component.enabled = newPosition == PathHandler.CameraPosition.CP_Free;
        }

        _lastView = newPosition;

        currentView.Value = newPosition == PathHandler.CameraPosition.CP_Free ? DefaultView :
            newPosition == PathHandler.CameraPosition.CP_Front ? FrontView :
            newPosition == PathHandler.CameraPosition.CP_Side ? SideView : TopView;
        
        onViewChanged.Invoke();
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

    public void AllowChargeInteraction(bool allowMovement)
    {
        foreach (var charge in _coulombLogic.GetChargeGameObjects())
        {
            AllowChargeInteraction(charge, allowMovement);
        }
    }
    
    public void AllowChargeInteraction(GameObject charge, bool allowMovement)
    {
        if(!allowMovement)
            HandleMovementRestrictions(charge, false, false, false);
        else if(_coulombLogic.IsIn2dMode())
            HandleMovementRestrictions(charge, true, true, false);
        else
        {
            var view = (PathHandler.CameraPosition) currentView.Value;
            var allowX = view != PathHandler.CameraPosition.CP_Side;
            var allowY = view != PathHandler.CameraPosition.CP_Top;
            var allowZ = view != PathHandler.CameraPosition.CP_Front;
            HandleMovementRestrictions(charge.gameObject, allowX, allowY, allowZ);
        }
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
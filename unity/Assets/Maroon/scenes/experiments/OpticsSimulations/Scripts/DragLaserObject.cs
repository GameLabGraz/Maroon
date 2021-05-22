using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DragLaserObject : MonoBehaviour
{
    private Camera mainCamera;
    private Plane movementPlane;
    private Material laserMat;
    private Color ogColor;

    private LPProperties laserProperties;

    [SerializeField]
    private Color hoverColor;
    [SerializeField]
    private Color draggingColor;
    [SerializeField]
    private Color activeColor;


    private bool currentlyDragging;
    private bool activeLP;

    private MeshRenderer handleRenderer;
    private Collider handleCollider;
    private LaserSelectionHandler laserHandler;

    Collider thisColl;

    private Vector3 offset = Vector3.zero;


    void Start()
    {
        mainCamera = Camera.main;
        movementPlane = new Plane(Vector3.up, new Vector3(0.0f, 0.0f, 0.0f)); //such an init needed?
        thisColl = GetComponent<Collider>();
        laserMat = GetComponent<Renderer>().material;

        ogColor = laserMat.color;

        hoverColor = Color.red;
        activeColor = Color.green;
        draggingColor = Color.grey;
        currentlyDragging = false;
        
        activeLP = false;
        laserProperties = GetComponent<LPProperties>();
        laserHandler = GameObject.FindGameObjectWithTag("LaserSelectionHandler").GetComponent<LaserSelectionHandler>();

        handleRenderer = gameObject.transform.GetChild(0).GetComponent<MeshRenderer>();
        handleCollider = gameObject.transform.GetChild(0).GetComponent<Collider>();
        handleCollider.enabled = false;
        handleRenderer.enabled = false;
    }


    void OnMouseDown()
    {
        currentlyDragging = true;
        Ray objIntersectRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit thisHit;
        thisColl.Raycast(objIntersectRay, out thisHit, Mathf.Infinity);

        movementPlane.SetNormalAndPosition(Vector3.up, thisHit.point);

        offset = thisHit.point - transform.position;


        makeActive();
    }

    private void OnMouseEnter()
    {
        if(!currentlyDragging) laserMat.color = hoverColor;
    }

    private void OnMouseExit()
    {
        if(!currentlyDragging)
        {
            if (isActiveLP())
            {
                makeActive();
            }
            else
            {
                laserMat.color = ogColor;
            }
        }   
    }

    private void OnMouseUp()
    {
        currentlyDragging = false;
        laserMat.color = hoverColor;
    }

    void OnMouseDrag()
    {
        float dist;
        Ray distRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        movementPlane.Raycast(distRay, out dist);

        Vector3 pointonplane = distRay.GetPoint(dist);
        transform.position = pointonplane - offset;
        laserMat.color = draggingColor;
    }


    bool isActiveLP()
    {
        GameObject activeLaser = laserHandler.currActiveLaser;

        if (activeLaser == null)
        {
            activeLP = false;
            return false;
        }
        return ReferenceEquals(gameObject, activeLaser);
    }

    public void makeActive()
    {
        laserHandler.setActiveIntensityAndWavelength(laserProperties.laserIntensity, laserProperties.laserWavelength);
        activeLP = true;
        laserMat.color = activeColor;
        handleRenderer.enabled = true;
        handleCollider.enabled = true;
    }
    
    public void makeInactive()
    {
        activeLP = false;
        laserMat.color = ogColor;
        handleRenderer.enabled = false;
        handleCollider.enabled = false;
    }

}
   

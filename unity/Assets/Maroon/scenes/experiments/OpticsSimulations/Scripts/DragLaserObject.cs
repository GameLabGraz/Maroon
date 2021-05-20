using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DragLaserObject : MonoBehaviour
{
    private Camera mainCamera;
    private Plane movementPlane;
    private Material lasermat;
    private Color ogColor;
    private bool currentlydragging;
    private Color activeColor;
    private bool activeLP;

    private MeshRenderer handleRenderer;
    private SphereCollider handleCollider;
    private LaserSelectionHandler laserhandler;

    Collider thisColl;

    private Vector3 offset = Vector3.zero;


    void Start()
    {
        mainCamera = Camera.main;
        movementPlane = new Plane(Vector3.up, new Vector3(0.0f, 0.0f, 0.0f)); //todo correct laser height
        thisColl = GetComponent<Collider>();
        lasermat = GetComponent<Renderer>().material;
        ogColor = lasermat.color;
        currentlydragging = false;
        activeColor = Color.green;
        activeLP = false;
        laserhandler = GameObject.FindGameObjectWithTag("LaserSelectionHandler").GetComponent<LaserSelectionHandler>();

        handleRenderer = gameObject.transform.GetChild(0).GetComponent<MeshRenderer>();
        handleCollider = gameObject.transform.GetChild(0).GetComponent<SphereCollider>();
        handleCollider.enabled = false;
        handleRenderer.enabled = false;
    }


    void OnMouseDown()
    {
        currentlydragging = true;
        Ray objIntersectRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit thisHit;
        thisColl.Raycast(objIntersectRay, out thisHit, Mathf.Infinity);

        movementPlane.SetNormalAndPosition(Vector3.up, thisHit.point);

        offset = thisHit.point - transform.position;


        makeActive();
    }

    private void OnMouseEnter()
    {
        if(!currentlydragging) lasermat.color = Color.red;
    }

    private void OnMouseExit()
    {
        if(!currentlydragging)
        {
            if (isActiveLP())
            {
                makeActive();
            }
            else
            {
                lasermat.color = ogColor;
            }
        }   
    }

    private void OnMouseUp()
    {
        currentlydragging = false;
        lasermat.color = Color.red;
    }

    void OnMouseDrag()
    {
        float dist;
        Ray distRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        movementPlane.Raycast(distRay, out dist);

        Vector3 pointonplane = distRay.GetPoint(dist);
        transform.position = pointonplane - offset;
        lasermat.color = Color.grey;
    }


    bool isActiveLP()
    {
        GameObject laserSelHandler = GameObject.Find("LaserSelectionHandler");
        GameObject activeLaser = laserSelHandler.GetComponent<LaserSelectionHandler>().currActiveLaser;
        if (activeLaser == null)
        {
            activeLP = false;
            return false;
        }

        activeLP = ReferenceEquals(gameObject, activeLaser);

        return activeLP;
    }

    public void makeActive()
    {
        laserhandler.setActiveIntensityAndWavelength(gameObject.GetComponent<LPProperties>().laserIntensity, gameObject.GetComponent<LPProperties>().laserWavelength);
        activeLP = true;
        lasermat.color = activeColor;
        handleRenderer.enabled = true;
        handleCollider.enabled = true;
    }
    
    public void makeInactive()
    {
        activeLP = false;
        lasermat.color = ogColor;
        handleRenderer.enabled = false;
        handleCollider.enabled = false;
    }

}
   

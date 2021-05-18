using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleHandler : MonoBehaviour
{

    private Camera mainCamera;
    private Plane rotatePlane;
    private Collider thisColl;
    private Vector3 offset = Vector3.zero;
    private Vector3 draganchor = Vector3.zero;
    private float yrotation;

    private Vector3 rotationanchor;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        thisColl = GetComponent<Collider>();
        new Plane(Vector3.up, new Vector3(0.0f, 0.0f, 0.0f));
    }


    private void OnMouseDown()
    {
        Ray objIntersectRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit thisHit;
        thisColl.Raycast(objIntersectRay, out thisHit, Mathf.Infinity);

        rotatePlane.SetNormalAndPosition(Vector3.up, thisHit.point);

        draganchor = transform.parent.position;
        offset = thisHit.point - transform.position;
        rotationanchor = transform.parent.up;
        yrotation = transform.parent.eulerAngles.y;
    }

    private void OnMouseDrag()
    {
        float dist;
        Ray distRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        rotatePlane.Raycast(distRay, out dist);

        Vector3 pointonplane = distRay.GetPoint(dist);
        
        Vector3 rotVector = pointonplane - offset;

        Vector3 direction = rotVector - draganchor;

        var angl = Vector3.SignedAngle(direction, rotationanchor, Vector3.up);
        transform.parent.rotation = Quaternion.Euler(0.0f, -(angl ) + 180.0f + yrotation, -90.0f);
    }
}

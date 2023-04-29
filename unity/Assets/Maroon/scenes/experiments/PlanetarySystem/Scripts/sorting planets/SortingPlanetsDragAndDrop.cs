using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingPlanetsDragAndDrop : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickUpClip;
    [SerializeField] private AudioClip dropClip;

    public Transform sortingPlanetTarget; // The specific target for each planet
    private float scaleFactor = 1.02f;    // The scale factor to adjust the object size based on the target
    private float snapDistance = 0.4f;    // Distance to snap the planet to the target

    private bool isSnapped = false;

    Vector3 mousePosition;
    Vector3 initialScale;

    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void Start()
    {
        initialScale = transform.localScale;
    }

    private void OnMouseDown()
    {
        if (!isSnapped)
        {
            audioSource.PlayOneShot(pickUpClip);
            mousePosition = Input.mousePosition - GetMousePos();
        }
    }

    private void OnMouseDrag()
    {
        //transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        if (!isSnapped)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
            transform.position = newPosition;

        }
    }

    private void OnMouseUp()
    {
        if (!isSnapped)
        {
            float distanceToTarget = Vector2.Distance(transform.position, sortingPlanetTarget.position);
            Debug.Log("Distance to targe" + distanceToTarget);


            float distanceX = transform.position.x - sortingPlanetTarget.position.x;
            float distanceY = transform.position.y - sortingPlanetTarget.position.y;
            //if (distanceToTarget <= snapDistance)

            Debug.Log("ScreenToWorldPoint" + Camera.main.ScreenToWorldPoint(transform.position) + "ScreenToWorldPoint - Input.mousePosition" + Camera.main.ScreenToWorldPoint(transform.position - Input.mousePosition) +"Input.mousePosition - ScreenToWorldPoint" + Camera.main.ScreenToWorldPoint(Input.mousePosition - transform.position));
            Debug.Log("ScreenToWorldPoint sortingPlanetTarget.position" + Camera.main.ScreenToWorldPoint(sortingPlanetTarget.position) + "WorldToScreenPoint sortingPlanetTarget.position" + Camera.main.WorldToScreenPoint(sortingPlanetTarget.position));
            Debug.Log("Distance:" + " x: " + transform.position.x + " y: " + transform.position.y + " z: " + transform.position.z);
            Debug.Log("sortingPlanetTarget Distance:" + " x: " + sortingPlanetTarget.position.x + " y: " + sortingPlanetTarget.position.y + " z: " + sortingPlanetTarget.position.z);

            if (distanceX > -snapDistance && distanceX < snapDistance &&
                distanceY > -snapDistance && distanceY < snapDistance)

            {
                SnapToTarget();
            }
        }
    }

    private void SnapToTarget()
    {
        transform.SetParent(sortingPlanetTarget);
        transform.position = sortingPlanetTarget.position;

        transform.localScale = new Vector3(1,1,1) * scaleFactor;

        isSnapped = true;
        audioSource.PlayOneShot(dropClip);
    }


}

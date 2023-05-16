using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingPlanetsDragAndDrop : MonoBehaviour, IResetObject
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickUpClip;
    [SerializeField] private AudioClip dropClip;

    public Transform sortingPlanetTarget; 
    private Transform placeholder_slot; 
    private float scaleFactor = 1.02f;   
    private float snapDistance = 0.2f;
    public float altSnapDistance = 0.2f;  

    private bool isSnapped = false;

    Vector3 mousePosition;
    Vector3 initialPosition;


    /*
     * 
     */
    private void Start()
    {
        snapDistance = altSnapDistance;
        initialPosition = transform.position;
        placeholder_slot = transform.parent;
    }


    /*
     * 
     */
    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }


    /*
     * 
     */
    private void OnMouseDown()
    {
        //Debug.Log("snapDistance: " + snapDistance);
        if (!isSnapped)
        {
            audioSource.PlayOneShot(pickUpClip);
            mousePosition = Input.mousePosition - GetMousePos();
        }
    }


    /*
     * 
     */
    private void OnMouseDrag()
    {
        if (!isSnapped)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
            transform.position = newPosition;
        }
    }


    /*
     * 
     */
    private void OnMouseUp()
    {
        if (!isSnapped)
        {
            Vector2 distanceVector = new Vector2(transform.position.x - sortingPlanetTarget.position.x, transform.position.y - sortingPlanetTarget.position.y);
                        
            //Debug.Log("SortingPlanetsDragAndDrop(): distanceVector.magnitude: " + distanceVector.magnitude);
            if (distanceVector.magnitude <= snapDistance)
            {
                //Debug.Log("SortingPlanetsDragAndDrop: OnMouseUp(): snapped successfully to placeholder slot");
                SnapToTarget();
            }
            else
            {
                //Debug.Log("SortingPlanetsDragAndDrop: OnMouseUp():  transform.position = initialPosition = " + initialPosition);
                transform.position = initialPosition;
                isSnapped = false;
            }
        }
    }



   /*
    * Increments the snapped planets and displays a Helpi message when all planets except pluto are in its place 
    */
    private void IncrementSnappedPlanetCount()
    {
        int solarSystemPlanet = 8 + 2;
        PlanetaryController.Instance.sortedPlanetCount++;

        // Check if we have snapped all 10 out of 11 planets, excluding pluto
        if (PlanetaryController.Instance.sortedPlanetCount >= solarSystemPlanet)
        {
            PlanetaryController.Instance.DisplayMessageByKey("OrderedSortingGame");
        }
    }


    /*
     * snaps the planets to target by setting its parent and scales them
     */
    private void SnapToTarget()
    {
        transform.SetParent(sortingPlanetTarget);
        transform.position = sortingPlanetTarget.position;
        transform.localScale = new Vector3(1,1,1) * scaleFactor;

        isSnapped = true;
        IncrementSnappedPlanetCount();
        audioSource.PlayOneShot(dropClip);
    }


    /*
     * reset sortingPlanet parent, scale, position, sortedPlanetCount
     */
    public void ResetObject()
    {
        transform.SetParent(placeholder_slot);
        transform.position = placeholder_slot.position;
        transform.localScale = new Vector3(1, 1, 1);

        PlanetaryController.Instance.ResetSortingGame();
        initialPosition = transform.position;

        isSnapped = false;
        PlanetaryController.Instance.sortedPlanetCount = 0;
    }
}

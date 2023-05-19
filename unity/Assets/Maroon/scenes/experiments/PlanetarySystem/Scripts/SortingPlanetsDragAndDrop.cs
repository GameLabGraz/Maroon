using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingPlanetsDragAndDrop : MonoBehaviour, IResetObject
{
    public AudioSource audioSource;
    public AudioClip pickUpClip;
    public AudioClip dropClip;

    public Transform sortingPlanetTarget; 
    public float snapDistance;  
    private bool isSnapped = false;
    private readonly float scaleFactor = 1.02f;
    public Light sunLightHalo;
    Vector3 mousePosition;


    /*
     * get mouse position
     */
    private void OnMouseDown()
    {
        if (!isSnapped)
        {
            audioSource.PlayOneShot(pickUpClip);
            mousePosition = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        }
    }


    /*
     * calculate position
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
     * snap to target when distance is smaller han snap distance
     * or snap back to start slot
     */
    private void OnMouseUp()
    {
        if (!isSnapped)
        {
            Vector2 distanceVector = new Vector2(transform.position.x - sortingPlanetTarget.position.x, transform.position.y - sortingPlanetTarget.position.y);
                        
            //Debug.Log("SortingPlanetsDragAndDrop(): distanceVector.magnitude: " + distanceVector.magnitude);
            if (distanceVector.magnitude <= snapDistance)
            {
                SnapToTarget();
            }
            else
            {
                transform.position = transform.parent.position;
                isSnapped = false;
            }
        }
    }



   /*
    * increments the snapped planets and displays a Helpi message when all planets except pluto are in its place 
    */
    private void IncrementSnappedPlanetCount()
    {
        int solarSystemPlanet = 10;
        PlanetaryController.Instance.sortedPlanetCount++;

        // check if we have snapped all 10 out of 11 planets, excluding pluto, including moon ad sun
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
        if (sunLightHalo != null)
            sunLightHalo.range = 1.75f;


        isSnapped = true;
        IncrementSnappedPlanetCount();
        audioSource.PlayOneShot(dropClip);
    }


    /*
     * reset sortingPlanet parent, scale, position, sortedPlanetCount
     */
    public void ResetObject()
    {
        transform.localScale = new Vector3(1, 1, 1);
        PlanetaryController.Instance.ResetSortingGame();
        if (sunLightHalo != null)
            sunLightHalo.range = 0.25f;

        isSnapped = false;
        PlanetaryController.Instance.sortedPlanetCount = 0;
    }
}

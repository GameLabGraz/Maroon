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
    private float snapDistance = 0.2f;
    public float altSnapDistance = 0.2f;     // Distance to snap the planet to the target

    private bool isSnapped = false;

    Vector3 mousePosition;
    Vector3 initialPosition;

    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }


    /*
     * 
     */
    private void Start()
    {
        snapDistance = altSnapDistance;
        initialPosition = transform.position;
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
                SnapToTarget();
            }
            else
            {
                transform.position = initialPosition;
                isSnapped = false;
                //StartCoroutine(MoveToPosition(initialPosition, 1.0f));
            }
        }
    }


    /*
     * 
     */
    private void SnapToTarget()
    {
        transform.SetParent(sortingPlanetTarget);
        transform.position = sortingPlanetTarget.position;
        transform.localScale = new Vector3(1,1,1) * scaleFactor;

        isSnapped = true;
        audioSource.PlayOneShot(dropClip);
    }

    // planet interpolates back to start position
    /*
    IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
    */

}

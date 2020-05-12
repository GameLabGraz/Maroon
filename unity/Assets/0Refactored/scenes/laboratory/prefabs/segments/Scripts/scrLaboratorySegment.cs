using System.Collections;
using UnityEngine;

public class scrLaboratorySegment : MonoBehaviour
{
    // #################################################################################################################
    // Members

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Settings

    // Length of this segment
    [SerializeField] private int segmentLength;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Constants

    // Length of a small segment
    private const int segmentLengthSmall = 4;

    // length of a large segment
    private const int segmentLengthLarge = 8;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Current values

    // Index of the segment after insertion
    private int targetIndex = 0;

    // Z position of the segment after insertion
    private int targetForwardTranslation = 0;

    // True if moved to target position
    private bool movedToTargetForwardTranslation = false;

    // True if currently moving
    private bool animationActive = false;

    // #################################################################################################################
    // Methods: Previews in segment

    // Adds a preview to the segment
    public void AddPreview(GameObject prefabPreview, bool isRight = false)
    {
        // Instantiate new preview
        GameObject new_preview = Instantiate(prefabPreview, this.gameObject.transform.position, Quaternion.identity,
                                             this.gameObject.transform);

        // Remove editor only stuff
        new_preview.transform.GetChild(0).gameObject.SetActive(false);
        
        // Rotate preview for other side
        if(isRight)
        {
            new_preview.transform.position = this.gameObject.transform.position +
                                             new Vector3(0, 0, scrLaboratorySegment.segmentLengthLarge);
            new_preview.transform.rotation = new Quaternion(0, 180, 0, 0);
        }
    }

    // #################################################################################################################
    // Methods: Index, position

    // Sets target index and re-calculates target forward translation
    public void setTargetIndexAndForwardTranslation(int targetIndex)
    {
        // If segment is first segment
        if (targetIndex <= 0)
        {
            // Set index and translation
            this.targetIndex = 0;
            this.targetForwardTranslation = 0;
        }

        // If segment is any other segment
        else
        {
            // Set index
            this.targetIndex = targetIndex;

            // Origin + first segment +  all previous segments
            this.targetForwardTranslation = 0 + scrLaboratorySegment.segmentLengthSmall +
                                            (targetIndex - 1) * scrLaboratorySegment.segmentLengthLarge;
        }

        // Segment is possibly not at desired location anymore
        this.movedToTargetForwardTranslation = false;
    }

    // Moves segment to target location or starts animation to target location
    public void moveToTargetForwardTranslation(bool animate = false)
    {
        // If animation
        if(animate)
        {
            this.animationActive = true;
            StartCoroutine(this.AnimateTranslation());
        }

        // If instant jump to position
        else
        {
            this.gameObject.transform.position = Vector3.zero +
                                                 this.gameObject.transform.forward * this.targetForwardTranslation;
            this.movedToTargetForwardTranslation = true;
        }
    }

    // #################################################################################################################
    // Methods: Movement

    // Animation coroutine
    private IEnumerator AnimateTranslation() 
    {
        while(true)
        {
            // Calculate target and distance
            Vector3 vec_target = Vector3.zero + this.gameObject.transform.forward * this.targetForwardTranslation;
            float distance = Vector3.Distance(vec_target, this.gameObject.transform.position);

            // From infinity to 1m
            if(distance > 1.0F)
            {
                // Move closer by current distance per second
                this.gameObject.transform.position -= this.gameObject.transform.forward * distance * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            // From 1m to 0.08m
            else if(distance > 0.08F)
            {
                // Move closer by 1 meter per second
                this.gameObject.transform.position -= this.gameObject.transform.forward * 1 * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            // If close enough to target location
            else
            {
                this.gameObject.transform.position = vec_target;
                this.movedToTargetForwardTranslation = true;
                this.animationActive = false;
                break;
            }
        }
    }
}

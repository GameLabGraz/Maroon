using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEAR.Serialize;

public class scrSegmentBuilder : MonoBehaviour
{
    // #################################################################################################################
    // Members

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Given Segments
    public GameObject staticSegementStart;
    public GameObject staticSegmentEnd;
    public GameObject staticSegmentContainer;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Prefabs
    public GameObject prefabSegment;    
    public GameObject prefabEmptyPreview;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Previews
    public GameObject[] prefabsExperimentPreview;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Counters
    private int numberSegmentsTotal;
    private int numberSegmentsCurrent = 0;

    // #################################################################################################################
    // Methods

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Start is called before the first frame update
    void Start()
    {
        this.buildAllSegments();
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Builds all segments according to the members
    void buildAllSegments()
    {
        // Calculate number of segments to be generated
        this.numberSegmentsTotal = Mathf.CeilToInt(this.prefabsExperimentPreview.Length / 2.0F);

        // Generate segments
        while(this.numberSegmentsCurrent < numberSegmentsTotal)
        {
            // If 2 or more previews left
            if((this.prefabsExperimentPreview.Length - (this.numberSegmentsCurrent * 2)) >= 2)
            {
                this.addSegment(this.prefabsExperimentPreview[this.numberSegmentsCurrent],
                                this.prefabsExperimentPreview[this.numberSegmentsCurrent + 1]);
            }

            // If one preview left
            else
            {
                this.addSegment(this.prefabsExperimentPreview[this.numberSegmentsCurrent], this.prefabEmptyPreview);                
            }

            // Update segment counter
            this.numberSegmentsCurrent += 1;
        }
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Adds one segment to the room
    void addSegment(GameObject prefabPreviewLeft, GameObject prefabPreviewRight, bool animate = false)
    {
        // Move end segment forward
        this.staticSegmentEnd.transform.position += this.staticSegmentEnd.transform.forward * 8;

        // Create new Segment
        GameObject new_segment = Instantiate(this.prefabSegment, new Vector3(0, 0, 0), Quaternion.identity);
        new_segment.transform.SetParent(this.staticSegmentContainer.transform);

        // Add previews
        this.addPreview(new_segment, prefabPreviewLeft);
        this.addPreview(new_segment, prefabPreviewRight, true);

        // Move to fit start transform
        new_segment.transform.position += new_segment.transform.forward * 4;

        // Move to fit previous segments
        new_segment.transform.position += new_segment.transform.forward * this.numberSegmentsCurrent * 8;
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Adds previews to one segment
    void addPreview(GameObject segment, GameObject prePreview, bool isRight = false)
    {
        /*
        GameObject new_preview = Instantiate(prePreview, new Vector3(0, 0, 0), Quaternion.identity);
        new_preview.transform.SetParent(segment.transform);

        if(isRight)
        {

        }
        */
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Update is called once per frame
    void Update()
    {
                
    }
}

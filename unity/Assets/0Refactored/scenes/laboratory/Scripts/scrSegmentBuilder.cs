using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrSegmentBuilder : MonoBehaviour
{
    // Segments and containers
    public GameObject SegementStart;
    public GameObject SegmentEnd;
    public GameObject DynamicSegmentPrefab;
    public GameObject DynamicSegmentContainer;
    
    // Previews to be placed in dynamic segments
    public GameObject[] experiments;

    // Counters
    private int numberOfSegments;
    private int numberOfgeneratedSegments = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate number of segments to be generated
        this.numberOfSegments = Mathf.CeilToInt(this.experiments.Length / 2.0F);

        // Generate segments
        while(numberOfgeneratedSegments < numberOfSegments)
        {
            // Move end segment forward
            //this.SegmentEnd.transform.position += this.SegmentEnd.transform.forward * 8;

            // Generate new segment
            // TODO: generate segments
            this.numberOfgeneratedSegments += 1;

            // Place experiments
            // TODO: place experiments
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        this.SegmentEnd.transform.position = this.SegmentEnd.transform.position + (this.SegmentEnd.transform.forward * 0.1F * Time.deltaTime);

        
    }
}

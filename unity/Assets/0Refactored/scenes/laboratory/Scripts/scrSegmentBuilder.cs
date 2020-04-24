using UnityEngine;

public class scrSegmentBuilder : MonoBehaviour
{
    // #################################################################################################################
    // Members

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Previews
    public GameObject[] prefabsExperimentPreview;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Given Segments
    [SerializeField] private GameObject staticSegementStart;
    [SerializeField] private GameObject staticSegmentEnd;
    [SerializeField] private GameObject staticSegmentContainer;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Prefabs
    [SerializeField] private GameObject prefabSegment;    
    [SerializeField] private GameObject prefabEmptyPreview;

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
        this.BuildAllSegments();
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Builds all segments according to the members
    public void BuildAllSegments()
    {
        // Calculate number of segments to be generated
        this.numberSegmentsTotal = Mathf.CeilToInt(this.prefabsExperimentPreview.Length / 2.0F);

        // Generate segments
        while(this.numberSegmentsCurrent < numberSegmentsTotal)
        {
            // If 2 or more previews left
            if((this.prefabsExperimentPreview.Length - (this.numberSegmentsCurrent * 2)) >= 2)
            {
                this.AddSegment(this.prefabsExperimentPreview[this.numberSegmentsCurrent * 2],
                                this.prefabsExperimentPreview[this.numberSegmentsCurrent * 2 + 1]);
            }

            // If one preview left
            else
            {
                this.AddSegment(this.prefabsExperimentPreview[this.numberSegmentsCurrent * 2], this.prefabEmptyPreview);                
            }

            // Update segment counter
            this.numberSegmentsCurrent += 1;
        }
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Adds one segment to the room
    private void AddSegment(GameObject prefabPreviewLeft, GameObject prefabPreviewRight, bool animate = false)
    {
        // Move end segment forward
        this.staticSegmentEnd.transform.position += this.staticSegmentEnd.transform.forward * 8;

        // Create new segment
        GameObject new_segment = Instantiate(this.prefabSegment, Vector3.zero, Quaternion.identity);
        new_segment.transform.SetParent(this.staticSegmentContainer.transform);

        // Add previews
        this.AddPreview(new_segment, prefabPreviewLeft);
        this.AddPreview(new_segment, prefabPreviewRight, true);

        // Move to fit start transform
        new_segment.transform.position += new_segment.transform.forward * 4;

        // Move to fit previous segments
        new_segment.transform.position += new_segment.transform.forward * this.numberSegmentsCurrent * 8;
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Adds previews to one segment
    private void AddPreview(GameObject segment, GameObject prefabPreview, bool isRight = false)
    {
        // Instantiate new preview
        GameObject new_preview = Instantiate(prefabPreview, Vector3.zero, Quaternion.identity, segment.transform);

        // Remove editor only stuff
        new_preview.transform.GetChild(0).gameObject.SetActive(false);
        
        // Rotate preview for other side
        if(isRight)
        {
            new_preview.transform.position = new Vector3(0, 0, 8);
            new_preview.transform.rotation = new Quaternion(0, 180, 0, 0);
        }
    }
}

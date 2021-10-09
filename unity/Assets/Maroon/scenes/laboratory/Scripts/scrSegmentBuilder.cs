using System.Collections;
using System.Collections.Generic;
using Maroon.GlobalEntities;
using UnityEngine;

public class scrSegmentBuilder : MonoBehaviour
{
    // #################################################################################################################
    // Members

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Settings

    // Activates animation
    public bool animate;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Previews

    /// <summary>
    ///     Laboratory block to be displayed if no laboratory block can be found.
    /// </summary>
    [SerializeField] private GameObject defaultLaboratoryBlock;


    /// <summary>
    ///     Laboratory blocks (aka previews of experiments in the lab) to be displayed. Generated on Start() based on
    ///     currently active scene category.
    /// </summary>
    private List<GameObject> laboratoryBlocks = new List<GameObject>();

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Given Segments

    // Start segment in the scene
    [SerializeField] private GameObject staticSegementStart;

    // End segment in the scene
    [SerializeField] private GameObject staticSegmentEnd;

    // Empty game object that holds segments that are created dynamically
    [SerializeField] private GameObject staticSegmentContainer;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Prefabs

    // Prefab to be used as a template for a new segment
    [SerializeField] private GameObject prefabSegment;

    // Prefab to be placed in a scene if no experiment preview is available   
    [SerializeField] private GameObject prefabEmptyPreview;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Counters (exclude start and end segment)

    // Number of segments to be generated to hold all experiment previews (excluding start and end)
    private int numberSegmentsTotal;

    // Number of segments currently in the scene (excluding start and end)
    private int numberDynamicSegments = 0;

    // #################################################################################################################
    // Methods

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Start is called before the first frame update
    private void Start()
    {
        // Reset laboratory blocks
        this.laboratoryBlocks.Clear();

        // TODO: This needs to be refactored, done by player
        GameManager.Instance.enteringLab();

        // Get scenes to select new laboratory blocks
        Maroon.CustomSceneAsset[] scenes = SceneManager.Instance.ActiveSceneCategory.Scenes;

        // Get new laboratory blocks
        for(int iScenes = 0; iScenes < scenes.Length; iScenes++)
        {
            string laboratoryBlockName = scenes[iScenes].SceneNameWithoutPlatformExtension + ".laboratoryblock";
            GameObject loadedObject = Resources.Load(laboratoryBlockName) as GameObject;
            if(loadedObject == null)
            {
                this.laboratoryBlocks.Add(this.defaultLaboratoryBlock);
                Debug.Log("No laboratoryblock available for: " + laboratoryBlockName);
            }
            else
            {
                this.laboratoryBlocks.Add(loadedObject);
            }
        }

        // Build laboratory segments
        StartCoroutine(this.BuildAllSegments());
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Builds all segments according to the members
    public IEnumerator BuildAllSegments()
    {
        // Calculate number of segments to be generated
        this.numberSegmentsTotal = Mathf.CeilToInt(this.laboratoryBlocks.Count / 2.0F);

        // Hide segment end
        this.staticSegmentEnd.SetActive(false);

        // Generate segments
        while(this.numberDynamicSegments < numberSegmentsTotal)
        {
            // If 2 or more previews left
            if((this.laboratoryBlocks.Count - (this.numberDynamicSegments * 2)) >= 2)
            {
                this.AddSegment(this.laboratoryBlocks[this.numberDynamicSegments * 2],
                                this.laboratoryBlocks[this.numberDynamicSegments * 2 + 1],
                                this.animate);
            }

            // If one preview left
            else
            {
                this.AddSegment(this.laboratoryBlocks[this.numberDynamicSegments * 2],
                                this.prefabEmptyPreview,
                                this.animate);                
            }

            // Wait before adding next segment
            if(this.animate)
            {
                yield return new WaitForSeconds(1);
            }

            // Update segment counter
            this.numberDynamicSegments += 1;
        }

        // Show segment end
        if(this.animate)
        {
            yield return new WaitForSeconds(6);
        }
        this.staticSegmentEnd.SetActive(true);
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Adds one segment to the room
    private void AddSegment(GameObject prefabPreviewLeft, GameObject prefabPreviewRight, bool animate = false)
    {
        // Move end segment forward
        this.staticSegmentEnd.transform.position += this.staticSegmentEnd.transform.forward * 8;

        // Create new segment
        GameObject new_segment = Instantiate(this.prefabSegment, new Vector3(0, 0, 500), Quaternion.identity);
        new_segment.transform.SetParent(this.staticSegmentContainer.transform);
        var segment_script = new_segment.GetComponent<scrLaboratorySegment>();

        // Add previews
        segment_script.AddPreview(prefabPreviewLeft);
        segment_script.AddPreview(prefabPreviewRight, true);

        // Move correct position
        segment_script.setTargetIndexAndForwardTranslation(this.numberDynamicSegments + 1);
        segment_script.moveToTargetForwardTranslation(animate);
    }    
}

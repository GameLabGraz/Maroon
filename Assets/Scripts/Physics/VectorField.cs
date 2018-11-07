//-----------------------------------------------------------------------------
// VectorField.cs
//
// Class for the vector field to represent the field
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Jobs;
using Unity.Collections;


/// <summary>
/// Class for the vector field to represent the field
/// </summary>
public class VectorField : MonoBehaviour
{
    /// <summary>
    /// List of field arrows
    /// </summary>
    private List<GameObject> vectorFieldArrows = new List<GameObject>();

    /// <summary>
    /// The vector field height
    /// </summary>
    private float height;

    /// <summary>
    /// The vector field width
    /// </summary>
	private float width;

    /// <summary>
    /// Indicates if the vector field is displayed or not
    /// </summary>
    private bool vectorFieldVisible = true;

    /// <summary>
    /// The arrow prefab which includes the model
    /// </summary>
    public Object arrowPrefab;

    /// <summary>
    /// The silder to change the vector field resolution
    /// </summary>
	public Slider sliderResolution;

    [SerializeField]
    private int resolution = 20;

    [SerializeField]
    private float fieldStrengthFactor = Teal.FieldStrengthFactor;
    public float getFieldStrenghFactor() { return this.fieldStrengthFactor; }
    private SimulationController simController;

    private const bool useJobSystem = true;

    //For Job System:
    public TransformAccessArray transforms;
    public NativeArray<Vector3> rotationArray;
    public Unity.Jobs.JobHandle turnArrowHandle;
    private MovementJob moveJob;
    private Unity.Jobs.JobHandle moveHandle;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

        height = GetComponent<MeshFilter>().mesh.bounds.size.z;
        width = GetComponent<MeshFilter>().mesh.bounds.size.x;

        createVectorFieldArrows();
    }
    private void OnDisable()
    {
        moveHandle.Complete();
        turnArrowHandle.Complete();
        clearVectorField();
        transforms.Dispose();
        rotationArray.Dispose();
    }
    private void OnEnable()
    {
        transforms = new TransformAccessArray(0, -1);
        rotationArray = new NativeArray<Vector3>((int)System.Math.Pow(resolution, 2), Allocator.TempJob);
    }

    /// <summary>
    /// Creates the vector field arrows and move them to the right position depending on the resolution.
    /// </summary>
	private void createVectorFieldArrows()
    {
        float arrow_scale;
        if (resolution > 20)
            arrow_scale = (1.0f - ((float)resolution - 20.0f) * 0.075f)/6.5f;
        else
            arrow_scale = (1.0f + (20.0f - (float)resolution) * 0.075f)/6.5f;
        if (!useJobSystem)
        {
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    float x = -width / 2 + (width / resolution) * (0.5f + i);
                    float y = -height / 2 + (height / resolution) * (0.5f + j);

                    GameObject arrow = Instantiate(arrowPrefab) as GameObject;
                    arrow.GetComponent<ArrowController>().fieldStrengthFactor = this.fieldStrengthFactor;

                    arrow.transform.localScale = Vector3.Scale(new Vector3(arrow_scale, arrow_scale, arrow_scale), transform.localScale);
                    arrow.transform.parent = transform; //set vectorField as parent
                    arrow.transform.localPosition = new Vector3(x, 0, y);

                    simController.AddNewResetObject(arrow.GetComponent<IResetObject>());
                    vectorFieldArrows.Add(arrow);
                }
            }
        }
        else
        {
            moveHandle.Complete();
            int resolutionsquare = (int)System.Math.Pow(resolution, 2);
            //Add arrows to match resolution requirements
                for (int i = (resolutionsquare - transforms.length); i > 0; i--)
                {
                    GameObject arrow = Instantiate(arrowPrefab) as GameObject;
                    //arrow.GetComponent<ArrowController>().fieldStrengthFactor = this.fieldStrengthFactor;
                    arrow.transform.parent = transform; //set vectorField as parent
                    simController.AddNewResetObject(arrow.GetComponent<IResetObject>());
                    vectorFieldArrows.Add(arrow);
                    transforms.Add(arrow.transform);
                }
            
            // Remove superfluous arrows
                for (int i = transforms.length - 1; i >= resolutionsquare; i--)
                {
                    GameObject arrow = vectorFieldArrows[i];
                    simController.RemoveResetObject(arrow.GetComponent<IResetObject>());
                    DestroyImmediate(arrow);
                    vectorFieldArrows.RemoveAt(i);
                    transforms.RemoveAtSwapBack(i);
                }
                if(transforms.length > rotationArray.Length)
            {
                turnArrowHandle.Complete();
                rotationArray.Dispose();
                rotationArray = new NativeArray<Vector3>(transforms.length, Allocator.TempJob);
            }
            
            Debug.Log("arrow_scale"+arrow_scale);
            if (resolution == 0) return;

            moveJob = new MovementJob()
            {
                //width = width,
                //height = height,
                resolution = resolution,
                xPrecooked = (-width / 2),
                yPrecooked = (-height / 2),
                xWeightfactor = (width / resolutionsquare),
                yWeightfactor = (height / resolution),
                arrow_scale_ = arrow_scale,
                localScale = transform.localScale
            };
            moveHandle = moveJob.Schedule(transforms);
            Unity.Jobs.JobHandle.ScheduleBatchedJobs();
        }
    }

    /// <summary>
    /// Sets the vetor field visibility by the given toggle.
    /// </summary>
    /// <param name="visibility">The toggle to get value from user</param>
    public void setVectorFieldVisible(bool visibility)
    {
        if (visibility)
        {
            vectorFieldVisible = true;
            clearVectorField();
            createVectorFieldArrows();
        }
        else
        {
            vectorFieldVisible = false;
            clearVectorField();
        }

    }

    /// <summary>
    /// Clears the vector field
    /// </summary>
	private void clearVectorField()
    {
            foreach (GameObject arrow in vectorFieldArrows)
            {
                simController.RemoveResetObject(arrow.GetComponent<IResetObject>());
                DestroyImmediate(arrow);
            }
            vectorFieldArrows.Clear();
    }

    /// <summary>
    /// Changed the resolution
    /// </summary>
	public void changeResolution()
    {
        if (!vectorFieldVisible) return;
        if (!useJobSystem) clearVectorField();
        createVectorFieldArrows();
        ArrowSystem.resetSimController();
    }

    public void setResolution(int resolution)
    {
        this.resolution = resolution;
        changeResolution();
    }
}

public struct MovementJob : IJobParallelForTransform
{
    //public float width;
    //public float height;
    public int resolution;
    public float xPrecooked;
    public float yPrecooked;
    public float xWeightfactor;
    public float yWeightfactor;

    public float arrow_scale_;
    public Vector3 localScale;

public void Execute(int index, TransformAccess transform)
    {   /*            
        // These calculations do not have to be executed resolutionsquare times:
        float x = (-width / 2) + ((width / (float)System.Math.Pow(resolution, 2)) * (float)index);
        float y = (-height / 2) + (height / resolution) * (index % resolution);
        */
        float x = xPrecooked + xWeightfactor * (float)index;
        float y = yPrecooked + yWeightfactor * (index % resolution);
        transform.localScale = Vector3.Scale(new Vector3(arrow_scale_, arrow_scale_, arrow_scale_), localScale);
        transform.localPosition = new Vector3(x, 0, y);
    }
}
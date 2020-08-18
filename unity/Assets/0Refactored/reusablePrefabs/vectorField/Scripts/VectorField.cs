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
using System.Collections.Generic;

/// <summary>
/// Class for the vector field to represent the field
/// </summary>
public class VectorField : MonoBehaviour
{
    /// <summary>
    /// List of field arrows
    /// </summary>
    protected List<GameObject> vectorFieldArrows = new List<GameObject>();
    
    [Range(1f, 20f)]
    public float scalingDivider = 3f;
    
    /// <summary>
    /// The vector fields width, height and depth
    /// </summary>
    protected float height;
    protected float width;
    protected float depth;
    
    /// <summary>
    /// Indicates if the vector field is displayed or not
    /// </summary>
    protected bool vectorFieldVisible = true;

    /// <summary>
    /// The arrow prefab which includes the model
    /// </summary>
    public Object arrowPrefab;

    public bool OnlyUpdateInRunMode = true;
    
    public GameObject Field = null;

    public bool ignoreXDimension = false;
    public bool ignoreYDimension = true;
    public bool ignoreZDimension = false;

    [SerializeField]
    protected int resolution = 20;
    public int Resolution
    {
        get => resolution;
        set => resolution = value;
    }

    [SerializeField]
    protected float fieldStrengthFactor = Teal.FieldStrengthFactor;

    /// <summary>
    /// Initialization
    /// </summary>
    protected void Start()
    {
        if(Field == null)
            Field = GameObject.FindGameObjectWithTag("Field");
        
        height = GetComponent<MeshFilter>().mesh.bounds.size.z;
        depth = GetComponent<MeshFilter>().mesh.bounds.size.y;
        width = GetComponent<MeshFilter>().mesh.bounds.size.x;
        createVectorFieldArrows();
    }

    /// <summary>
    /// Creates the vector field arrows and move them to the right position depending on the resolution.
    /// </summary>
    protected virtual void createVectorFieldArrows()
    {
        if (SimulationController.Instance == null)
        {
            Start();
            return;
        }

        float arrow_scale;
        if (resolution > 20)
            arrow_scale = 1 - (resolution - 20) * 0.05f;
        else
            arrow_scale = 1 + (20 - resolution) * 0.05f;
        
        var arrow = Instantiate(arrowPrefab, transform) as GameObject;
        arrow.transform.localScale =
            Vector3.Scale(new Vector3(arrow_scale, arrow_scale, arrow_scale), transform.localScale) / scalingDivider;
        var controller = arrow.GetComponent<ArrowController>();
        controller.fieldStrengthFactor = fieldStrengthFactor;
        controller.OnlyUpdateInRunMode = OnlyUpdateInRunMode;
        controller.Field = Field;
        controller.Allow3dRotation = !ignoreXDimension && !ignoreYDimension && !ignoreZDimension;
        arrow.SetActive(true);
        
        var widthHalf = -width / 2;
        var widthFactor = width / resolution;
        var heightHalf = -height / 2;
        var heightFactor = height / resolution;
        var depthHalf = -depth / 2;        
        var depthFactor = depth / resolution;

        var xRes = ignoreXDimension ? 1 : resolution;
        var yRes = ignoreYDimension ? 1 : resolution;
        var zRes = ignoreZDimension ? 1 : resolution;

        var currentIdx = 0;
        
        for (var i = 0; i < xRes; ++i) // x
        {
            for (var j = 0; j < yRes; ++j) //y
            {
                for (var k = 0; k < zRes; k++) // z
                {
                    var x = -width / 2 + (width / xRes) * (0.5f + i);

                    var y = -depth / 2f + (depth / yRes) * (0.5f + j);
                    var z = -height / 2 + (height / zRes) * (0.5f + k);

                    GameObject currentArrow;
                    if (currentIdx < vectorFieldArrows.Count)
                    {
                        currentArrow = vectorFieldArrows[currentIdx];
                        currentArrow.transform.localScale =
                            Vector3.Scale(new Vector3(arrow_scale, arrow_scale, arrow_scale), transform.localScale) / scalingDivider;
                        currentArrow.SetActive(true);
                    }
                    else
                    {
                        currentArrow = Instantiate(arrow, transform);
                        SimulationController.Instance.AddNewResetObject(currentArrow.GetComponent<IResetObject>());
                        vectorFieldArrows.Add(currentArrow);
                    }
                    currentArrow.transform.localPosition = new Vector3(x, y, z);
                    currentIdx++;
                }
            }
        }

        for (; currentIdx < vectorFieldArrows.Count; ++currentIdx)
        {
            if(!vectorFieldArrows[currentIdx].activeSelf) break;
            vectorFieldArrows[currentIdx].SetActive(false);
        }
        
        arrow.SetActive(false);
    }

    /// <summary>
    /// Sets the vector field visibility by the given toggle.
    /// </summary>
    /// <param name="visibility">The toggle to get value from user</param>
    public void setVectorFieldVisible(bool visibility)
    {
        if (visibility)
        {
            vectorFieldVisible = true;
            // clearVectorField();
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
    protected void clearVectorField()
    {
        foreach (var arrow in vectorFieldArrows)
        {
            arrow.SetActive(false);
        }
    }

    /// <summary>
    /// Changed the resolution
    /// </summary>
	public void changeResolution()
    {
        if (!vectorFieldVisible)
            return;
        createVectorFieldArrows();
    }

    public void setResolution(float resolution)
    {
        this.resolution = (int)resolution;
        changeResolution();
    }


    public void setResolution(int resolution)
    {
        this.resolution = resolution;
        changeResolution();
    }
}

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

    private SimulationController simController;

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

    /// <summary>
    /// Creates the vector field arrows and move them to the right position depending on the resolution.
    /// </summary>
	private void createVectorFieldArrows()
    {
        float arrow_scale;
        if (resolution > 20)
            arrow_scale = 1 - (resolution - 20) * 0.05f;
        else
            arrow_scale = 1 + (20 - resolution) * 0.05f;

        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float x = -width / 2 + (width / resolution) * (0.5f + i);
                float y = -height / 2 + (height / resolution) * (0.5f + j);

                GameObject arrow = Instantiate(arrowPrefab) as GameObject;
                arrow.GetComponent<ArrowController>().fieldStrengthFactor = this.fieldStrengthFactor;

                arrow.transform.localScale = Vector3.Scale(new Vector3(arrow_scale, arrow_scale, arrow_scale), transform.localScale) / 3;
                arrow.transform.parent = transform; //set vectorField as parent
                arrow.transform.localPosition = new Vector3(x, 0, y);

                simController.AddNewResetObject(arrow.GetComponent<IResetObject>());
                vectorFieldArrows.Add(arrow);
            }
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
        if (!vectorFieldVisible)
            return;
        clearVectorField();
        createVectorFieldArrows();
    }

    public void setResolution(int resolution)
    {
        this.resolution = resolution;
        changeResolution();
    }
}

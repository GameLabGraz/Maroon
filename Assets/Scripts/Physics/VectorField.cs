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
    /// The height offset
    /// </summary>
	private float height_offset;

    /// <summary>
    /// The width offset
    /// </summary>
	private float width_offset;

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

    private SimulationController simController;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

        height = GetComponent<MeshFilter>().mesh.bounds.size.z * transform.localScale.y;
        width = GetComponent<MeshFilter>().mesh.bounds.size.x * transform.localScale.x;
        height_offset = transform.position.y;
        width_offset = transform.position.x;

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
                float x = -width / 2 + (width / resolution) * (0.5f + i) + width_offset;
                float y = -height / 2 + (height / resolution) * (0.5f + j) + height_offset;

                Vector3 position = new Vector3(x, y, transform.position.z);
                GameObject arrow = Instantiate(arrowPrefab, position, Quaternion.identity) as GameObject;
                simController.AddNewResetObject(arrow.GetComponent<IResetObject>());

                arrow.transform.localScale = new Vector3(arrow_scale, arrow_scale, arrow_scale);
                arrow.transform.parent = transform; //set vectorField as parent
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

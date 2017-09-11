//-----------------------------------------------------------------------------
// Graph.cs
//
// Script which handles the drawing of the graph
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Script which handles the drawing of the graph
/// </summary>
public class Graph : MonoBehaviour, IResetObject
{
    /// <summary>
    /// Material used for drawing the graph
    /// </summary>
    [SerializeField]
    private Material material;

    /// <summary>
    /// Texture used for drawing the graph
    /// </summary>
    private Texture2D texture;

    /// <summary>
    /// Line renderer which draws the graph
    /// </summary>
    private AdvancedLineRenderer line_renderer;

    /// <summary>
    /// value on x axis of graph
    /// </summary>
    private float time = 0;

    /// <summary>
    /// position of point in line renderer
    /// </summary>
    private int index = 0;

    /// <summary>
    /// Max num of vertices being drawn
    /// </summary>
    private float max_vertex;

    /// <summary>
    /// The rect transform width
    /// </summary>
    private float width;

    /// <summary>
    /// The rect transform height
    /// </summary>
    private float height;

    /// <summary>
    /// The rect transform component of the object
    /// </summary>
    private RectTransform objectRectTransform;

    /// <summary>
    /// The coil, to get the current value to draw
    /// </summary>
    private Coil coil;

    /// <summary>
    /// The game object from which a value curve is be drawn
    /// </summary>
    [SerializeField]
    private GameObject valueGetterObject;

    /// <summary>
    /// The value getter method name which is called to get the draw value.
    /// The method must take exactly one argument with the type MessageArgs.
    /// The argument will be used to get the value.
    /// </summary>
    [SerializeField]
    private string valueGetterMethodByReference;

    /// <summary>
    /// max value of the graph
    /// </summary>
    [SerializeField]
    private float max = 1;

    /// <summary>
    /// min value of the graph
    /// </summary>
    [SerializeField]
    private float min = -1;

    [SerializeField]
    private bool invertedCoordinateSystem = false;

    private SimulationController simController;

    [SerializeField]
    private float stepSize = 0.5f;

    [SerializeField]
    private float yOffset = 0.01f;

    [SerializeField]
    private float lineWidth = 0.1f;

    private int i = 0;

    /// <summary>
    /// Initialization of Graph
    /// </summary>
    void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();


        //objectRectTransform = GetComponent<RectTransform>();
        //height = 10; //objectRectTransform.rect.height;
        //width = 10; //objectRectTransform.rect.width;

        height = GetComponent<MeshFilter>().mesh.bounds.size.z;// * transform.localScale.z;
        width = GetComponent<MeshFilter>().mesh.bounds.size.x;// * transform.localScale.x;

        Debug.Log("Graph Height: " + height);
        Debug.Log("Graph Width: " + width);

        time = invertedCoordinateSystem ? width /2 : -width / 2;
        max_vertex = width / stepSize;

        Debug.Log("Graph Max Vertex: " + max_vertex);

        texture = new Texture2D(100, 100);
        initTexture();

        GetComponent<Renderer>().material.mainTexture = texture;
        
        line_renderer = gameObject.AddComponent<AdvancedLineRenderer>();
        line_renderer.SetWidth(lineWidth, lineWidth);
        line_renderer.SetMaterial(material);
        line_renderer.initLineRenderer();
    }

    /// <summary>
    /// Draws the graph when the simulation is running
    /// </summary>
    void FixedUpdate()
    {
        if (!simController.SimulationRunning)
            return;

        /*
        if(!height.Equals(objectRectTransform.rect.height) || !width.Equals(objectRectTransform.rect.width))
        {
            height = objectRectTransform.rect.height;
            width = objectRectTransform.rect.width;
            resetObject();
        }
        */

        if (i++ % 6 != 0)
            return;

        MessageArgs messageArgs = new MessageArgs();     
        valueGetterObject.SendMessage(valueGetterMethodByReference, messageArgs);
        float value = (float)messageArgs.value;

        line_renderer.SetPosition(index++, new Vector3(time, yOffset, getRange(value)));
        line_renderer.WritePositionsToLineRenderer();
        time = invertedCoordinateSystem ? time - stepSize : time + stepSize;
        if (index > max_vertex)
            resetObject();
    }

    /// <summary>
    /// Normalizes value to fit into the graph
    /// </summary>
    /// <param name="value">Value to normalize</param>
    /// <returns>new value</returns>
    private float getRange(float value)
    {
        if (value > max)
            value = max;
        if (value < min)
            value = min;

        float range = (height * (value - min)) / (max - min) - (height / 2);
        return invertedCoordinateSystem ? -range : range;
    }

    /// <summary>
    /// Initializes the texture of the graph
    /// </summary>
    private void initTexture()
    {
        Color[] col = new Color[texture.width * texture.height];
        for (int i = 0; i < col.Length; i++)
            col[i] = Color.white;

        texture.SetPixels(col);
        texture.Apply();

        for (int i = 0; i < texture.width; i++) //Black line
            texture.SetPixel(i, texture.height / 2, Color.black);

        texture.Apply();
    }

    /// <summary>
    /// Resets the graph
    /// </summary>
    public void resetObject()
    {
        this.time = invertedCoordinateSystem ? width / 2 : -width / 2;
        this.max_vertex = width / stepSize;
        this.index = 0;
        line_renderer.Clear();
    }
}

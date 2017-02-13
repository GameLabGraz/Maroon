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
    public Material material;

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
    /// max value of the graph
    /// </summary>
    public float max = 1;

    /// <summary>
    /// min value of the graph
    /// </summary>
    public float min = -1;  

    /// <summary>
    /// Initialization of Graph
    /// </summary>
    void Start()
    {
        objectRectTransform = GetComponent<RectTransform>();
        height = objectRectTransform.rect.height;
        width = objectRectTransform.rect.width;
        time = -width / 2;
        max_vertex = -time * 2;

        texture = new Texture2D(100, 100);
        initTexture();
        GetComponent<RawImage>().texture = texture;

        line_renderer = gameObject.AddComponent<AdvancedLineRenderer>();
        line_renderer.SetWidth(0.4f, 0.4f);
        line_renderer.SetMaterial(material);
        line_renderer.initLineRenderer();

        coil = GameObject.Find("Coil").GetComponent<Coil>();
    }

    /// <summary>
    /// Draws the graph when the simulation is running
    /// </summary>
    void FixedUpdate()
    {
        if (!SimController.Instance.SimulationRunning)
            return;

        if(!height.Equals(objectRectTransform.rect.height) || !width.Equals(objectRectTransform.rect.width))
        {
            height = objectRectTransform.rect.height;
            width = objectRectTransform.rect.width;
            resetObject();
        }

        float value = coil.getCurrent();
        line_renderer.SetPosition(index++, new Vector3(time, getRange(value), -5));
        line_renderer.WritePositionsToLineRenderer();
        time += 0.5f;
        if (index > max_vertex * 2)
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

        return (height * (value - min)) / (max - min) - (height / 2);
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
        this.time = -width / 2;
        this.max_vertex = -time * 2;
        this.index = 0;
        line_renderer.Clear();
    }
}

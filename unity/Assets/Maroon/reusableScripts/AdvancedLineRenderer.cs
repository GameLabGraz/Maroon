//-----------------------------------------------------------------------------
// AdvancedLineRenderer.cs
//
// WrapperClass for Basic Line Renderer
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// WrapperClass for Basic Line Renderer
/// </summary>
public class AdvancedLineRenderer : MonoBehaviour
{
    /// <summary>
    /// Basic line renderer which is used for drawing
    /// </summary>
    private LineRenderer line_;

    /// <summary>
    /// Number of vertices drawn
    /// </summary>
    private int vertexCount_ = 0;

    /// <summary>
    /// Dictionary of vertex positions
    /// </summary>
    private SortedDictionary<int, Vector3> positions_ = new SortedDictionary<int, Vector3>();

    /// <summary>
    /// Line width for drawing
    /// </summary>
    private Tuple<float, float> lineWidth_ = new Tuple<float, float>(0.1f, 0.1f);

    /// <summary>
    /// Line color for drawing
    /// </summary>
    private Tuple<Color, Color> lineColors_ = new Tuple<Color, Color>(Color.white, Color.white);

    /// <summary>
    /// Use world space if true
    /// </summary>
    private bool useWorldSpace_ = false;

    /// <summary>
    /// Material used for drawing
    /// </summary>
    [SerializeField]
    private Material material;

    private bool _generateLightingData;

    /// <summary>
    /// Get or Set if world space is used
    /// </summary>
    public bool useWorldSpace
    {
        get { return useWorldSpace_; }
        set
        {
            useWorldSpace_ = value;
            line_.useWorldSpace = value;
        }
    }

    public bool GenerateLightingData
    {
        get { return _generateLightingData; }
        set
        {
            _generateLightingData = value;
            line_.generateLightingData = value;
        }
    }

    /// <summary>
    /// Initialization of Advance Line Renderer
    /// </summary>
    public void Awake()
    {
        line_ = gameObject.GetComponent<LineRenderer>();
        if (line_ == null)
        {
            line_ = gameObject.AddComponent<LineRenderer>();
            InitLineRenderer();
        }

        useWorldSpace = false;
        //material = new Material(Shader.Find("Particles/Additive"));
    }

    /// <summary>
    /// Setter for setting the color to draw
    /// </summary>
    /// <param name="startColor">start color</param>
    /// <param name="endColor">end color</param>
    public void SetColors(Color startColor, Color endColor)
    {
        lineColors_ = new Tuple<Color, Color>(startColor, endColor);
        line_.startColor = startColor;
        line_.endColor = endColor;
    }

    /// <summary>
    /// Getter for color to draw.
    /// </summary>
    /// <returns>color currently used to draw</returns>
    public Tuple<Color, Color> GetColors()
    {
        return lineColors_;
    }

    /// <summary>
    /// Set one position where line renderer should draw.
    /// This function only buffers the position and does not yet draw.
    /// </summary>
    /// <param name="index">index of vertex</param>
    /// <param name="position">position in space of vertex</param>
    public void SetPosition(int index, Vector3 position)
    {
        positions_[index] = position;
    }

    /// <summary>
    /// Get drawing position at specified index of the
    /// Advanced Line Renderer
    /// </summary>
    /// <param name="index">index of vertex in line renderer</param>
    /// <returns>position at index</returns>
    public Vector3 GetPosition(int index)
    {
        Vector3 value;
        if (positions_.TryGetValue(index, out value))
            return value;
        throw new KeyNotFoundException("Key " + index + " was not found.");

    }

    /// <summary>
    /// Get all positions from the Advanced Line Renderer
    /// </summary>
    /// <returns>list of all positions</returns>
    public List<KeyValuePair<int, Vector3>> GetPositions()
    {
        var intVectorList = new List<KeyValuePair<int, Vector3>>();
        foreach (var entry in positions_)
        {
            intVectorList.Add(entry);
        }
        return intVectorList;
    }

    /// <summary>
    /// Set all position of the line renderer at once.
    /// </summary>
    /// <param name="dictEntries">new entries for the positions</param>
    public void SetPositions(List<KeyValuePair<int, Vector3>> dictEntries)
    {
        foreach (var entry in dictEntries)
        {
            positions_[entry.Key] = entry.Value;
        }
    }

    /// <summary>
    /// Setter function for the material used to draw
    /// </summary>
    /// <param name="material">new material</param>
    public void SetMaterial(Material material)
    {
        this.material = material;
        line_.material = material;
    }

    /// <summary>
    /// Draw all stored positions.
    /// </summary>
    public void WritePositionsToLineRenderer()
    {
        vertexCount_ = positions_.Keys.Max() + 1;
        line_.positionCount = vertexCount_;
        foreach (var entry in positions_)
        {
            line_.SetPosition(entry.Key, entry.Value);
        }
    }

    /// <summary>
    /// Setter function for the vertex count of the line renderer
    /// </summary>
    /// <param name="count">new vertex count</param>
    public void SetVertexCount(int vertextCount)
    {
        vertexCount_ = vertextCount;
        line_.positionCount = vertextCount;
    }

    /// <summary>
    /// Getter function for the vertex count.
    /// </summary>
    /// <returns>vertex count</returns>
    public int GetVertexCount()
    {
        return vertexCount_;
    }

    /// <summary>
    /// Setter function for the line width of the 
    /// Advanced Line Renderer
    /// </summary>
    /// <param name="startWidth"></param>
    /// <param name="endWidth"></param>
    public void SetWidth(float startWidth, float endWidth)
    {
        lineWidth_ = new Tuple<float, float>(startWidth, endWidth);
        line_.startWidth = startWidth;
        line_.endWidth = endWidth;
    }

    /// <summary>
    /// Getter function for line width
    /// </summary>
    /// <returns></returns>
    public Tuple<float, float> GetWidth()
    {
        return lineWidth_;
    }

    /// <summary>
    /// Clear the Advanced Line renderer.
    /// Deletes all positions and sets vertex count to 0
    /// </summary>
    public void Clear()
    {
        vertexCount_ = 0;
        clearLineRenderer();
        positions_.Clear();
    }

    /// <summary>
    /// Removes certain point at specified index from
    /// the vertices of the Advanced Line Renderer
    /// </summary>
    /// <param name="index">index at which to remove vertex</param>
    public void RemoveAt(int index)
    {
        positions_.Remove(index);
        clearLineRenderer();
        WritePositionsToLineRenderer();
    }

    /// <summary>
    /// Initialize Line Renderer
    /// </summary>
    public void InitLineRenderer()
    {
        SetWidth(lineWidth_.Item1, lineWidth_.Item2);
        SetColors(lineColors_.Item1, lineColors_.Item2);
        line_.positionCount = vertexCount_;
        line_.useWorldSpace = useWorldSpace_;
        line_.material = material;
        line_.generateLightingData = _generateLightingData;
    }

    /// <summary>
    /// Private Clearing function used to reset
    /// the underlying line renderer
    /// </summary>
    private void clearLineRenderer()
    {
        InitLineRenderer();
    }
}
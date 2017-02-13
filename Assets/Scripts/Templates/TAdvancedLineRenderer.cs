//-----------------------------------------------------------------------------
// TAdvancedLineRenderer.cs
//
// WrapperClass for Basic Line Renderer
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    private SortedDictionary<int, Vector3> positions_;

    /// <summary>
    /// Line width for drawing
    /// </summary>
    private Tuple<float, float> lineWidth_;

    /// <summary>
    /// Line color for drawing
    /// </summary>
    private Tuple<Color, Color> lineColors_;

    /// <summary>
    /// Use world space if true
    /// </summary>
    private bool useWorldSpace_ = false;

    /// <summary>
    /// Material used for drawing
    /// </summary>
    private Material material;

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

    /// <summary>
    /// Initialization of Advance Line Renderer
    /// </summary>
    public void Awake()
    {
        line_ = gameObject.GetComponent<LineRenderer>();
        if (line_ == null)
            line_ = gameObject.AddComponent<LineRenderer>();

        //initLineRenderer();
        useWorldSpace = false;
        material = new Material(Shader.Find("Particles/Additive"));
    }

    /// <summary>
    /// Constructor of Advanced Line Renderer
    /// </summary>
    public AdvancedLineRenderer()
    {
        positions_ = new SortedDictionary<int, Vector3>();
        lineWidth_ = new Tuple<float, float>(0.1f, 0.1f);
        lineColors_ = new Tuple<Color, Color>(Color.white, Color.white);
        //material = new Material(Shader.Find("Particles/Additive"));
    }

    /// <summary>
    /// Setter for setting the color to draw
    /// </summary>
    /// <param name="start">start color</param>
    /// <param name="end">end color</param>
    public void SetColors(Color start, Color end)
    {
        lineColors_ = new Tuple<Color, Color>(start, end);
        line_.SetColors(start, end);
    }

    /// <summary>
    /// Getter for color to draw.
    /// </summary>
    /// <returns>color currently used to draw</returns>
    public Tuple<Color, Color> GetColors()
    {
        return this.lineColors_;
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
        List<KeyValuePair<int, Vector3>> intVectorList = new List<KeyValuePair<int, Vector3>>();
        foreach (KeyValuePair<int, Vector3> entry in positions_)
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
        foreach (KeyValuePair<int, Vector3> entry in dictEntries)
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
        line_.SetVertexCount(vertexCount_);
        foreach (KeyValuePair<int, Vector3> entry in positions_)
        {
            line_.SetPosition(entry.Key, entry.Value);
        }
    }

    /// <summary>
    /// Setter function for the vertex count of the line renderer
    /// </summary>
    /// <param name="count">new vertex count</param>
    public void SetVertexCount(int count)
    {
        this.vertexCount_ = count;
        line_.SetVertexCount(count);
    }

    /// <summary>
    /// Getter function for the vertex count.
    /// </summary>
    /// <returns>vertex count</returns>
    public int GetVertexCount()
    {
        return this.vertexCount_;
    }

    /// <summary>
    /// Setter function for the line width of the 
    /// Advanced Line Renderer
    /// </summary>
    /// <param name="starting width"></param>
    /// <param name="ending width"></param>
    public void SetWidth(float start, float end)
    {
        this.lineWidth_ = new Tuple<float, float>(start, end);
        line_.SetWidth(start, end);
    }

    /// <summary>
    /// Getter function for line width
    /// </summary>
    /// <returns></returns>
    public Tuple<float, float> GetWidth()
    {
        return this.lineWidth_;
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
    public void initLineRenderer()
    {
        this.SetWidth(lineWidth_.item1, lineWidth_.item2);
        this.SetColors(lineColors_.item1, lineColors_.item2);
        line_.SetVertexCount(vertexCount_);
        line_.useWorldSpace = useWorldSpace_;
        line_.material = material;
    }

    /// <summary>
    /// Private Clearing function used to reset
    /// the underlying line renderer
    /// </summary>
    private void clearLineRenderer()
    {
        UnityEngine.Object.DestroyImmediate(line_);
        line_ = gameObject.AddComponent<LineRenderer>();
        initLineRenderer();
    }
}
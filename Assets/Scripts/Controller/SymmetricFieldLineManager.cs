using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymmetricFieldLineManager : FieldLineManager
{
    /// <summary>
    /// The symmetry count, number of copys
    /// </summary>
    [SerializeField]
    private int symmetryCount = Teal.DefaultNumFieldLines;

    /// <summary>
    /// The symmetry axis
    /// </summary>
    [SerializeField]
    private Vector3 symmetryAxis = Vector3.up;

    /// <summary>
    /// Set of existing field line clones
    /// </summary>
    private HashSet<GameObject> clones = new HashSet<GameObject>();



    /// <summary>
    /// Indicates if symmetry is enabled
    /// </summary>
    private bool symmetryEnabled = true;


    private const bool ObjectPoolingApproach = true;
    private float m_rotation_scale = 0.0f;

    /// <summary>
    /// Sets the symmetry count
    /// </summary>
    /// <param name="symmetryCnt"></param>
    public void setSymmetryCount(int symmetryCnt)
    {
        symmetryEnabled = symmetryCnt == 1 ? false : true;
        symmetryCount = symmetryCnt;
        if (ObjectPoolingApproach)
        {
            ClearClones();
            m_rotation_scale = 360f / symmetryCount;
            foreach (FieldLine fieldLine in fieldLines)
            {
                fieldLine.draw();

                // clone field line around symmetry axis
                float rotation = m_rotation_scale;

                for (int i = 1; i < symmetryCount; ++i)
                {
                    GameObject clone;
                        try
                        {
                            clone = fieldLine.pooledclones.Pop();
                            clone.SetActive(true);
                        }
                        catch (System.InvalidOperationException)
                        {
                        clone = Instantiate(fieldLine.gameObject, fieldLine.transform.position, Quaternion.identity);
                        CloseFieldLine CloseFieldLine = clone.GetComponent<CloseFieldLine>();
                        if (CloseFieldLine) Destroy(CloseFieldLine);
                        FieldLine skripttodelete = clone.GetComponent<FieldLine>();
                        if (skripttodelete) Destroy(skripttodelete);

                        clone.transform.SetParent(fieldLine.transform.parent);
                        clone.transform.localScale = fieldLine.transform.localScale;
                    }


                    //rotate clones to fill the whole 360°
                    clone.transform.localEulerAngles = rotation * symmetryAxis;

                    AdvancedLineRenderer LineRender_ = clone.GetComponent<AdvancedLineRenderer>();
                    if (LineRender_ == null)
                    {
                        LineRender_ = clone.AddComponent<AdvancedLineRenderer>();
                        LineRender_.initLineRenderer();
                    }
                    fieldLine.clones.Add(LineRender_);
                    clones.Add(clone);
                    rotation += m_rotation_scale;
                }
            }
        }
    }

    protected override void DrawFieldLines()
    {
        if (!ObjectPoolingApproach)
        {
            ClearClones();

            foreach (FieldLine fieldLine in fieldLines)
            {
                fieldLine.draw();

                // clone field line around symmetry axis
                float rotation_scale = 360f / symmetryCount;
                float rotation = rotation_scale;

                for (int i = 1; i < symmetryCount; ++i)
                {
                    GameObject clone = Instantiate(fieldLine.gameObject, fieldLine.transform.position, Quaternion.identity);
                    clone.GetComponent<AdvancedLineRenderer>().SetVertexCount(fieldLine.GetLinePositions().Count);
                    clone.GetComponent<AdvancedLineRenderer>().SetPositions(fieldLine.GetLinePositions());

                    //workaround to keep the fieldline and its clones at the same scale
                    Vector3 temp = clone.transform.localScale;
                    clone.transform.SetParent(fieldLine.transform.parent);
                    clone.transform.localScale = temp;

                    //rotate clones to fill the whole 360°
                    clone.transform.localEulerAngles = rotation * symmetryAxis;
                    clones.Add(clone);
                    rotation += rotation_scale;
                }
            }
        }
        else
        {
            foreach (FieldLine fieldLine in fieldLines)
            {
                fieldLine.draw();
                int vertexcount = fieldLine.GetVertexCount();
                List<KeyValuePair<int, Vector3>> LinePositions = vertexcount > 0 ? fieldLine.GetLinePositions() : null;

                foreach (AdvancedLineRenderer clone in fieldLine.clones)
                {
                    if (vertexcount > 0)
                    {
                        clone.SetVertexCount(vertexcount);
                        clone.SetPositions(LinePositions);
                        clone.WritePositionsToLineRenderer();
                    }
                    else
                        clone.Clear();
                }
            }
        }
    }

    /// <summary>
    /// Clears all the copies of the field lines
    /// </summary>
    private void ClearClones()
    {
        if (ObjectPoolingApproach)
        {
            foreach (FieldLine fieldLine in fieldLines)
            {
                foreach (AdvancedLineRenderer clone in fieldLine.clones)
                {
                    clone.transform.gameObject.SetActive(false);
                    fieldLine.pooledclones.Push(clone.transform.gameObject);
                }
                fieldLine.clones.Clear();
            }
        }
        else
        foreach (GameObject clone in clones)
                DestroyImmediate(clone);
            clones.Clear();
    }
}

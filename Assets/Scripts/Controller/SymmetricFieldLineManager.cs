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


    /// <summary>
    /// Sets the symmetry count
    /// </summary>
    /// <param name="symmetryCnt"></param>
    public void setSymmetryCount(int symmetryCnt)
    {
        symmetryEnabled = symmetryCnt == 1 ? false : true;
        symmetryCount = symmetryCnt;
    }

    protected override void DrawFieldLines()
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

    /// <summary>
    /// Clears all the copies of the field lines
    /// </summary>
    private void ClearClones()
    {
        foreach (GameObject clone in clones)
            DestroyImmediate(clone);
        clones.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymmetricFieldLineManager : FieldLineManager
{
    /// <summary>
    /// The symmetry count, number of copies
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
    private readonly HashSet<GameObject> clones = new HashSet<GameObject>();

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
        symmetryEnabled = symmetryCnt != 1;
        symmetryCount = symmetryCnt;
    }

    public void setSymmetryCount(float symmetryCnt)
    {
        setSymmetryCount((int)symmetryCnt);
    }

    protected override void DrawFieldLines()
    {
        ClearClones();
        foreach (var fieldLine in fieldLines)
        {
            fieldLine.Draw();

            // clone field line around symmetry axis
            var rotationScale = 360f / symmetryCount;
            var rotation = rotationScale;

            for (var i = 1; i < symmetryCount; ++i)
            {
                var clone = Instantiate(fieldLine.gameObject, fieldLine.transform.position, Quaternion.identity);
                clone.GetComponent<AdvancedLineRenderer>().SetVertexCount(fieldLine.GetLinePositions().Count);
                clone.GetComponent<AdvancedLineRenderer>().SetPositions(fieldLine.GetLinePositions());

                //workaround to keep the field line and its clones at the same scale
                var temp = clone.transform.localScale;
                clone.transform.SetParent(fieldLine.transform.parent);
                clone.transform.localScale = temp;

                //rotate clones to fill the whole 360°
                clone.transform.localEulerAngles = rotation * symmetryAxis;
                clones.Add(clone);
                rotation += rotationScale;
            }
        }
    }

    /// <summary>
    /// Clears all the copies of the field lines
    /// </summary>
    private void ClearClones()
    {
        foreach (var clone in clones)
            Destroy(clone);
        clones.Clear();
    }
}

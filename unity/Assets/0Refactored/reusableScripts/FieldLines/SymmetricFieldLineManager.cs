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

    protected override void Start()
    {
        base.Start();
        CheckSymmetryAxis();
    }

    private static void CheckSymmetryAxis()
    {
        //check symmetry axis
        var emObjects = FindObjectsOfType<EMObject>();
        for (var i = 1; i < emObjects.Length; i++)
        {
            var check1 = Vector3.Cross(emObjects[0].FieldAlignment, emObjects[i].FieldAlignment).sqrMagnitude < 0.01f;
            var check2 = Vector3.Cross(emObjects[0].FieldAlignment, emObjects[i].transform.position - emObjects[0].transform.position).sqrMagnitude < 0.01f;

            if (!check1 || !check2)
                Debug.LogError("Warning: The EMObjects do not have the same symmetry axis. Please do not use the SymmetricFieldLineManager.");
        }
    }
    
    /// <summary>
    /// Sets the symmetry count
    /// </summary>
    /// <param name="symmetryCnt"></param>
    public void setSymmetryCount(int symmetryCnt)
    {
        symmetryEnabled = symmetryCnt != 1;
        symmetryCount = symmetryCnt;
    }

    public int GetSymmetryCount()
    {
        return symmetryCount;
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
                var lineRender = clone.GetComponent<AdvancedLineRenderer>();
                if (lineRender)
                {
                    lineRender.SetVertexCount(fieldLine.GetLinePositions().Count);
                    lineRender.SetPositions(fieldLine.GetLinePositions());
                }

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

using UnityEngine;
using System.Collections.Generic;

public class CloneGenerator : MonoBehaviour
{
    public int rowCount = 1;
    public Vector3 direction;
    public Vector3 direction2;
    public Vector3 randomOffset;
    public GizmoType gizmoType;

    public enum GizmoType
    {
        Sphere,
        Cube
    }

    private List<Vector3> GetPoints()
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < rowCount; i++)
        {
            if (direction2.sqrMagnitude > 0)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    Vector3 newPoint = new Vector3(direction.x * i + direction2.x * j, direction.y * i + direction2.y * j, direction.z * i + direction2.z * j);
                    points.Add(newPoint + GetRandomOffset());
                }
            }
            else
            {
                Vector3 newPoint = new Vector3(direction.x * i, direction.y * i, direction.z * i);
                points.Add(newPoint + GetRandomOffset());
            }
        }

        points.RemoveAt(0);
        return points;
    }

    private Vector3 GetRandomOffset()
    {
        return new Vector3(Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(-randomOffset.y, randomOffset.y),
            Random.Range(-randomOffset.z, randomOffset.z));
    }

    void Start()
    {
        foreach (Vector3 point in GetPoints())
        {
            GameObject go = Instantiate(gameObject, transform.position + point, transform.rotation, gameObject.transform.parent);
            go.transform.localScale = gameObject.transform.localScale;
            Destroy(go.GetComponent<CloneGenerator>());
            go.GetComponent<ChargedParticle>().UpdateColor();
        }

        Destroy(this);
    }

    void OnDrawGizmos()
    {
        var cp = GetComponent<ChargedParticle>();
        if (cp)
        {
            cp.UpdateColor();
            Gizmos.color = GetComponent<Renderer>().material.color;
        }

        foreach (Vector3 point in GetPoints())
        {
            switch (gizmoType)
            {
                case GizmoType.Sphere:
                    Gizmos.DrawSphere(gameObject.transform.position + point, gameObject.transform.lossyScale.x / 2f);
                    break;
                case GizmoType.Cube:
                    Gizmos.DrawCube(gameObject.transform.position + point, gameObject.transform.lossyScale);
                    break;
            }
        }
    }
}
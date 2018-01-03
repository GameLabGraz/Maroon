using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MagnetCircle : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private readonly List<Vector3> _linePositions = new List<Vector3>();
    [Range(3, 256)] public int PositionCount = 128;
    [Range(0.1f, 100f)] public float Radius = 1.0f;

    public float Height = 1f;
    [Range(0.01f, 10f)] public float LineWidth = 0.1f;

    public BField Field;

    public Vector3 OriginOffset = Vector3.zero;

    private EMObject _emObj;

    public float LineSegmentLength = 1f;

    public float xradius = 1f;
    public float yradius = 1f;

    public float closingDistance = 0.1f;
    public float maxAngle = 360f;


    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _emObj = gameObject.GetComponentInParent<EMObject>();
    }

    private void Start()
    {
//        float heightStep = Height / PositionCount;
//        for (int i = 0; i < PositionCount; i++)
//        {
//            _linePositions.Add(new Vector3(0f, heightStep * i, 0f));
//        }
//
//        Debug.Log("Line Positions: " + _linePositions.Count);
//        for (int i = 0; i < PositionCount; i++)
//        {
//            Debug.Log(i + ": " + _linePositions[i]);
//        }
//
//        _lineRenderer.positionCount = PositionCount;
//        _lineRenderer.SetPositions(_linePositions.ToArray());

        DrawLine();
    }

    private float t = 0f;

    private void Update()
    {
        t += Time.deltaTime;
        if (t > 1f)
        {
            t = 0f;
//            DrawLine();
        }
    }

    public void DrawLine()
    {
        Color c1 = Color.red;
        _lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        _lineRenderer.startColor = c1;
        _lineRenderer.endColor = c1;
        _lineRenderer.startWidth = LineWidth;
        _lineRenderer.endWidth = LineWidth;
//        _lineRenderer.useWorldSpace = false;

        float deltaTheta = (float) (2.0 * Mathf.PI) / PositionCount;
        float theta = 0f;

        Vector3 position = transform.position - OriginOffset;
//        float closingAngle = fixClosingAngle + (4 - emObj.getFieldStrength()) * 2;
//        float closingAngle = (4 - _emObj.getFieldStrength()) * 2;
        float closingAngle = _emObj.getFieldStrength();


//        float x;
//        float y;
//        float z = 0f;
//       
//        float angle = 20f;
//        _lineRenderer.positionCount = PositionCount;
//        _lineRenderer.loop = true;
//       
//        for (int i = 0; i < (PositionCount + 1); i++)
//        {
//            x = Mathf.Sin (Mathf.Deg2Rad * angle) * xradius;
//            y = Mathf.Cos (Mathf.Deg2Rad * angle) * yradius;
//                   
//            _lineRenderer.SetPosition (i,new Vector3(x,y,z) );
//                   
//            angle += (360f / PositionCount);
//        }

        Debug.Log("Start closingAngle: " + closingAngle);
        float angleSum = 0f;

//        PositionCount = Mathf.CeilToInt(360f / closingAngle);
//        Debug.Log("PositionCount: " + PositionCount);

        bool prevAssigned = false;
        Vector3 prev = Vector3.zero;

        int positionCount = 0;
        while (angleSum > -maxAngle && angleSum < maxAngle)
        {
            Vector3 p = Vector3.Normalize(-Field.get(position) * Teal.FieldStrengthFactor);
            Vector3 direction = new Vector3
            (
                Mathf.Cos(closingAngle * Mathf.Deg2Rad) * p.x - Mathf.Sin(closingAngle * Mathf.Deg2Rad) * p.y,
                Mathf.Sin(closingAngle * Mathf.Deg2Rad) * p.x + Mathf.Cos(closingAngle * Mathf.Deg2Rad) * p.y,
                p.z
            );
            direction *= Radius;
            position += direction * LineSegmentLength;

            if (prevAssigned)
            {
                float angle = Vector3.Angle(prev, position);
                angleSum += angle;
                var startVec = _lineRenderer.GetPosition(0);
                Debug.Log("angle: " + angle + ", Sum: " + angleSum + "     " + "Dist: " + Vector3.Distance(startVec, position));
                
                if (angleSum > 350f && Vector3.Distance(startVec, position) < closingDistance)
                {
                    Debug.LogError("MagnetCircle: closing Distance reached!");
                    break;
                }
            }

            prev = position;
            prevAssigned = true;

            positionCount++;



            if (positionCount > 1000)
            {
                Debug.LogError("MagnetCircle: Limit of 1000 positions reached!");
                break;
            }
        }

        PositionCount = positionCount;
        Debug.Log("PositionCount: " + PositionCount);
        _lineRenderer.positionCount = PositionCount + 1;

        for (int i = 0; i < PositionCount + 1; i++)
        {
//            angleSum += closingAngle;
//            Debug.Log("angleSum: " + angleSum);

//            float x = Radius * Mathf.Cos(theta);
//            float z = Radius * Mathf.Sin(theta);
//            Vector3 pos = new Vector3(x, 0, z);
//            _lineRenderer.SetPosition(i, pos);
//            theta += deltaTheta;

            Vector3 p = Vector3.Normalize(-Field.get(position) * Teal.FieldStrengthFactor);
            Vector3 direction = new Vector3
            (
                Mathf.Cos(closingAngle * Mathf.Deg2Rad) * p.x - Mathf.Sin(closingAngle * Mathf.Deg2Rad) * p.y,
                Mathf.Sin(closingAngle * Mathf.Deg2Rad) * p.x + Mathf.Cos(closingAngle * Mathf.Deg2Rad) * p.y,
                p.z
            );
            direction *= Radius;

            position += direction * LineSegmentLength;

//            _lineRenderer.SetPosition(i, transform.InverseTransformPoint(position));
            _lineRenderer.SetPosition(i, position);

//            Vector3 dist = transform.position;
//            if (coil) // hack for coil
//                dist -= 1.5f * new Vector3(Mathf.Abs(emObj.transform.up.x), Mathf.Abs(emObj.transform.up.y),
//                            Mathf.Abs(emObj.transform.up.z));
//            if (Vector3.Distance(position, dist) <= 0.8f || Vector3.Distance(position, transform.position) <= 0.4f)
//                break;
        }

//        _lineRenderer.SetPositions(_linePositions.ToArray());
    }
}
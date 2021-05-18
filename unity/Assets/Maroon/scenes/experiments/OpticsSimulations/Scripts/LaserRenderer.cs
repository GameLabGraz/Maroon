using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LaserRenderer : MonoBehaviour
{


    private LineRenderer[] lineRenderers = new LineRenderer[100];
    public Material LaserMaterial;
    private List<OpticsSegment> LaserSegments;

    public GameObject[] laserPointers;

    /* values to change */
    [SerializeField]
    [Range(1.0f, 10f)]
    private float lensOuter_RI = 1.3f;
    [SerializeField]
    [Range(1.0f, 10f)]
    private float lensInner_RI = 1.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float reflection_vs_refraction = 0.3f;

    [SerializeField]
    [Range(0.0f, 0.99f)]
    private float loss = 0.9f;
    //[SerializeField]
    //private bool renderAllLasersAtMaxIntenisty = false;

    private GameObject thislens;

    private int updated = 0;
    // Start is called before the first frame update
    void Start()
    {
        // add line renderers to parent object
        //find all laserpointers we wanna use

        LaserSegments = new List<OpticsSegment>();
        thislens = GameObject.FindGameObjectWithTag("Lens");

        foreach (Transform child in transform)// destroy all lasers that belong to the lens, to make everything work in editmode
        {
            DestroyImmediate(child.gameObject);
        }

        for (int i = 0; i < 100; i++)
        {
            lineRenderers[i] = new GameObject().AddComponent<LineRenderer>() as LineRenderer;
            lineRenderers[i].material = LaserMaterial;
            lineRenderers[i].transform.parent = gameObject.transform;

            //lineRenderers[i].SetPosition(0, new Vector3(2, 2, 2));
            //lineRenderers[i].SetPosition(1, new Vector3(3, 3, 3));
            lineRenderers[i].startWidth = 0.02f;
            lineRenderers[i].endWidth = 0.02f;
            lineRenderers[i].numCapVertices = 5;
            lineRenderers[i].useWorldSpace = false;

        }
    }


    void Update()
    {
        //update what the renderers showed, and change if it differs now.
        if(LaserSegments == null)
        {
            LaserSegments = new List<OpticsSegment>();
        }
        LaserSegments.Clear();

        //get all laserpointers in scene 

        laserPointers = GameObject.FindGameObjectsWithTag("LaserPointer");
        GameObject opLens = GameObject.FindGameObjectWithTag("Lens");
        OpticsLens currLens = opLens.GetComponent<LensMeshGenerator>().thisLensLens;

        currLens.radius = opLens.GetComponent<LensMeshGenerator>().lensRadius;
        currLens.innerRefractiveidx = lensInner_RI;
        currLens.outerRefractiveidx = lensOuter_RI;

        foreach (var laserp in laserPointers)
        {
            Vector3 relLaserPos = gameObject.transform.InverseTransformPoint(laserp.transform.position);
            Vector3 relLaserDir = gameObject.transform.InverseTransformDirection(laserp.transform.up);
            //todo add laserpointer properties

            OpticsRay laserRay = new OpticsRay(new Vector2(relLaserPos.x, relLaserPos.z), new Vector2(relLaserDir.x, relLaserDir.z), 1.0f, laserp.GetComponent<LPProperties>().laserColor);
            opLens.GetComponent<OpticsSim>().calcHitsRecursive(laserRay, currLens, true, ref LaserSegments, loss, reflection_vs_refraction);

        }
        UpdateLasers();

    }

    void UpdateLasers()
    {
        //if (!(updated == 0)) return;
        //updated = 1;

        for(int i = 0; i < lineRenderers.Length; i++)
        {
            if(i < LaserSegments.Count)
            {
                lineRenderers[i].enabled = true;
                UpdateLR(LaserSegments[i], lineRenderers[i]);
            }
            else
            {
                lineRenderers[i].enabled = false;
            }
        }
    }

    void UpdateLR(OpticsSegment opticsSeg, LineRenderer Lr, bool useIntensity = true)
    {
        Lr.SetPosition(0, new Vector3( opticsSeg.p1.x, 0.0f, opticsSeg.p1.y)*thislens.transform.localScale.x + thislens.transform.position); // mby works with vec2? 
        Lr.SetPosition(1, new Vector3( opticsSeg.p2.x, 0.0f, opticsSeg.p2.y)*thislens.transform.localScale.x + thislens.transform.position);
        if(useIntensity) Lr.material.color = new Color(opticsSeg.segmentColor.r, opticsSeg.segmentColor.g, opticsSeg.segmentColor.b, opticsSeg.intensity);
    }
}

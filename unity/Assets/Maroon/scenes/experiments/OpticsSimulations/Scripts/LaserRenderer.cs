using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.Physics;

//[ExecuteInEditMode]
public class LaserRenderer : MonoBehaviour
{

    [SerializeField]
    private LineRenderer[] lineRenderers = new LineRenderer[100];
    public Material LaserMaterial;
    private List<OpticsSegment> LaserSegments;

    public GameObject[] laserPointers;

    [SerializeField]
    private float laserWidth = 0.01f;
    /* values to change */
    [SerializeField]
    [Range(1.0f, 10f)]
    private float lensOuter_RI = 1.3f;
    [SerializeField]
    [Range(0.0f, 10f)]
    private float lensInner_RI = 0.0f;

    public QuantityFloat inner_RI;
    
    public QuantityFloat outer_RI;

    public QuantityFloat lensradius1;

    public QuantityFloat lensradius2;
    public QuantityFloat lensrad3;

    public OpticsLens currentLens;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float reflection_vs_refraction = 0.3f;

    public QuantityFloat refl_vs_refr;

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

        //foreach (Transform child in transform)// destroy all lasers that belong to the lens, to make everything work in editmode
        //{
        //    DestroyImmediate(child.gameObject);
        //}

        for (int i = 0; i < lineRenderers.Length; i++)
        {
            lineRenderers[i] = new GameObject().AddComponent<LineRenderer>();
            lineRenderers[i].material = LaserMaterial;
            lineRenderers[i].transform.parent = gameObject.transform;

            lineRenderers[i].startWidth = laserWidth;
            lineRenderers[i].endWidth = laserWidth;
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
        OpticsLens currLens = opLens.GetComponent<LensMeshGenerator>().thisLensLens; // todo this chould be changed, not pretty

        lensradius1.Value = currLens.leftCircle.radius;
        //Debug.Log("lensradius1 " + lensradius1.Value);
        lensradius2.Value = currLens.rightCircle.radius;
        
        

        
        currLens.radius = opLens.GetComponent<LensMeshGenerator>().lensRadius;
        currLens.innerRefractiveidx = inner_RI;
        currLens.outerRefractiveidx = outer_RI;
        currentLens = currLens;

        lensrad3.Value = 0.0f;
        (float left, float right ) = opLens.GetComponent<OpticsSim>().getBoundsHit(currLens);
        lensrad3.Value = (right - left) + opLens.GetComponent<LensMeshGenerator>().radcalc.Value - opLens.GetComponent<LensMeshGenerator>().radcalc2.Value;

        foreach (var laserp in laserPointers)
        {
            Vector3 relLaserPos = gameObject.transform.InverseTransformPoint(laserp.transform.position);
            Vector3 relLaserDir = gameObject.transform.InverseTransformDirection(laserp.transform.up);
            var lpprop = laserp.GetComponent<LPProperties>();

            float intensity = lpprop.laserIntensity;
            float wavelength = lpprop.laserWavelength;
            //todo add laserpointer properties

            OpticsRay laserRay = new OpticsRay(new Vector2(relLaserPos.x, relLaserPos.z), new Vector2(relLaserDir.x, relLaserDir.z), intensity, lpprop.laserColor, wavelength);
            opLens.GetComponent<OpticsSim>().calcHitsRecursive(laserRay, currLens, true, ref LaserSegments, loss, refl_vs_refr);

        }
        UpdateLasers();

    }

    void UpdateLasers()
    {

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
        if(useIntensity) Lr.material.SetColor("_TintColor", new Color(opticsSeg.segmentColor.r, opticsSeg.segmentColor.g, opticsSeg.segmentColor.b, opticsSeg.intensity));
    }




    public void setUIUpdateLensRefractiveIndex(int dropdownSelection) //todo maybe move this in extra class
    {

        if (dropdownSelection == 0) return;
        dropdownSelection--;

        List<float> cauchy_As = new List<float>();
        List<float> cauchy_Bs = new List<float>();

        cauchy_As.Add(1.7387f);
        cauchy_As.Add(1.5111f);
        cauchy_As.Add(1.4767f);
        cauchy_As.Add(1.3244f);
        cauchy_As.Add(2.3818f);
        cauchy_As.Add(1.7522f);

        cauchy_Bs.Add(0.0159f);
        cauchy_Bs.Add(0.00425f);
        cauchy_Bs.Add(0.0048f);
        cauchy_Bs.Add(0.0031f);
        cauchy_Bs.Add(0.0121f);
        cauchy_Bs.Add(0.0055f);

        inner_RI.Value = cauchy_As[dropdownSelection];
        outer_RI.Value = cauchy_Bs[dropdownSelection];
    }
}

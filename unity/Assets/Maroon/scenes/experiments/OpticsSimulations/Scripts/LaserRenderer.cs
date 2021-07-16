using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.Physics;

//[ExecuteInEditMode]
public class LaserRenderer : MonoBehaviour
{

    [SerializeField]
    private LineRenderer[] _lineRenderers = new LineRenderer[100];
    public Material LaserMaterial;
    private List<OpticsSegment> _laserSegments;

    public GameObject[] LaserPointers;

    [SerializeField]
    private float _laserWidth = 0.003f;
    /* values to change */
    [SerializeField]
    [Range(1.0f, 10f)]
    private float _lensOuterRI = 1.3f;
    [SerializeField]
    [Range(0.0f, 10f)]
    private float _lensInnerRI = 0.0f;

    public QuantityFloat CauchyA;
    
    public QuantityFloat CauchyB;

    public QuantityFloat lensradius1;
    public QuantityFloat lensradius2;
    public QuantityFloat lensrad3;

    public OpticsLens CurrentLens;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float _reflectionVsRefraction = 0.3f;

    public QuantityFloat refl_vs_refr;

    [SerializeField]
    [Range(0.0f, 0.99f)]
    private float _loss = 0.9f;

    private GameObject _thisLens;

    void Start()
    {
        // add line renderers to parent object
        //find all laserpointers we wanna use

        _laserSegments = new List<OpticsSegment>();
        _thisLens = GameObject.FindGameObjectWithTag("Lens");

        //foreach (Transform child in transform)// destroy all lasers that belong to the lens, to make everything work in editmode
        //{
        //    DestroyImmediate(child.gameObject);
        //}

        for (int i = 0; i < _lineRenderers.Length; i++)
        {
            _lineRenderers[i] = new GameObject().AddComponent<LineRenderer>();
            _lineRenderers[i].material = LaserMaterial;
            _lineRenderers[i].transform.parent = gameObject.transform;

            _lineRenderers[i].startWidth = _laserWidth;
            _lineRenderers[i].endWidth = _laserWidth;
            _lineRenderers[i].numCapVertices = 5;
            _lineRenderers[i].useWorldSpace = false;

        }
    }


    void Update()
    {
        //update what the renderers showed, and change if it differs now.
        if(_laserSegments == null)
        {
            _laserSegments = new List<OpticsSegment>();
        }
        _laserSegments.Clear();

        //get all laserpointers in scene 

        LaserPointers = GameObject.FindGameObjectsWithTag("LaserPointer");
        GameObject opLens = GameObject.FindGameObjectWithTag("Lens");
        OpticsLens currLens = opLens.GetComponent<LensMeshGenerator>().thisLensLens; // todo this chould be changed, not pretty

        lensradius1.Value = currLens.leftCircle.radius;
        lensradius2.Value = currLens.rightCircle.radius;
        currLens.radius = opLens.GetComponent<LensMeshGenerator>().lensRadius;
        currLens.cauchyA = CauchyA;
        currLens.cauchyB = CauchyB;
        CurrentLens = currLens;

        lensrad3.Value = 0.0f;
        (float left, float right ) = opLens.GetComponent<OpticsSim>().GetBoundsHit(currLens);
        lensrad3.Value = (right - left) + opLens.GetComponent<LensMeshGenerator>().radcalc.Value - opLens.GetComponent<LensMeshGenerator>().radcalc2.Value;

        foreach (var laserp in LaserPointers)
        {
            Vector3 relLaserPos = gameObject.transform.InverseTransformPoint(laserp.transform.position);
            Vector3 relLaserDir = gameObject.transform.InverseTransformDirection(laserp.transform.up);
            var lpprop = laserp.GetComponent<LPProperties>();

            float intensity = lpprop.LaserIntensity;
            float wavelength = lpprop.LaserWavelength;
            //todo add laserpointer properties

            OpticsRay laserRay = new OpticsRay(new Vector2(relLaserPos.x, relLaserPos.z), new Vector2(relLaserDir.x, relLaserDir.z), intensity, lpprop.LaserColor, wavelength);
            opLens.GetComponent<OpticsSim>().CalculateHitsRecursive(laserRay, currLens, true, ref _laserSegments, _loss, refl_vs_refr);

        }
        UpdateLasers();

    }

    void UpdateLasers()
    {

        for(int i = 0; i < _lineRenderers.Length; i++)
        {
            if(i < _laserSegments.Count)
            {
                _lineRenderers[i].enabled = true;
                UpdateLR(_laserSegments[i], _lineRenderers[i]);
            }
            else
            {
                _lineRenderers[i].enabled = false;
            }
        }
    }

    void UpdateLR(OpticsSegment opticsSeg, LineRenderer Lr, bool useIntensity = true)
    {
        Lr.SetPosition(0, new Vector3( opticsSeg.p1.x, 0.0f, opticsSeg.p1.y)*_thisLens.transform.localScale.x + _thisLens.transform.position); // mby works with vec2? 
        Lr.SetPosition(1, new Vector3( opticsSeg.p2.x, 0.0f, opticsSeg.p2.y)*_thisLens.transform.localScale.x + _thisLens.transform.position);
        if(useIntensity) Lr.material.SetColor("_TintColor", new Color(opticsSeg.segmentColor.r, opticsSeg.segmentColor.g, opticsSeg.segmentColor.b, opticsSeg.intensity));
    }




    public void SetCauchyVariables(int dropdownSelection) //todo maybe move this in extra class
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

        CauchyA.Value = cauchy_As[dropdownSelection];
        CauchyB.Value = cauchy_Bs[dropdownSelection];
    }
}

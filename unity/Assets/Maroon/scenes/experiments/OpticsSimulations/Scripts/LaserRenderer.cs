//
//Author: Tobias Stöckl
//
using System.Collections.Generic;
using UnityEngine;
using Maroon.Physics;

public class LaserRenderer : MonoBehaviour
{

    [SerializeField]
    private LineRenderer[] _lineRenderers = new LineRenderer[500];
    [SerializeField]
    private Material LaserMaterial;
    private List<OpticsSegment> _laserSegments;
    [SerializeField]
    private GameObject[] LaserPointers;

    [SerializeField]
    private float _laserWidth = 0.003f;

    public QuantityFloat CauchyA;
    public QuantityFloat CauchyB;

    public QuantityFloat LensRadius1;
    public QuantityFloat LensRadius2;
    public QuantityFloat LensDiameter;
    private float _unitMultiplier = 10.0f;

    public OpticsLens CurrentLens;

    private LensMeshGenerator _lensMeshGenerator;
    

    public QuantityFloat ReflectionVsRefraction;

    [SerializeField]
    [Range(0.0f, 0.99f)]
    private float _loss = 0.9f;
    private GameObject _thisLens;
    private OpticsSim _thisLensOpticsSim;

    private void Start()
    {
        // add line renderers to parent object
        _laserSegments = new List<OpticsSegment>();
        _thisLens = GameObject.Find("LensObject");
        _thisLensOpticsSim = _thisLens.GetComponent<OpticsSim>();
        _lensMeshGenerator = _thisLens.GetComponent<LensMeshGenerator>();


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


    private void Update()
    {
        //update what the renderers showed, and change if it differs now
        if (_laserSegments == null)
        {
            _laserSegments = new List<OpticsSegment>();
        }
        _laserSegments.Clear();

        //get all laserpointers in scene 

        //LaserPointers = GameObject.FindGameObjectsWithTag("LaserPointer");

        Maroon.Physics.LaserPointer[] LaserPointers2 = FindObjectsOfType<Maroon.Physics.LaserPointer>();
        
        OpticsLens currLens = _lensMeshGenerator.ThisLensLens;

        LensRadius1.Value = _unitMultiplier*currLens.LeftCircle.Radius;
        LensRadius2.Value = _unitMultiplier*currLens.RightCircle.Radius;
        currLens.Radius = _lensMeshGenerator.LensRadius;
        currLens.CauchyA = CauchyA;
        currLens.CauchyB = CauchyB;
        CurrentLens = currLens; //make current lens struct accessible to other scripts

        LensDiameter.Value = 0.0f;
        (float left, float right ) = _thisLensOpticsSim.GetBoundsHit(currLens);
        LensDiameter.Value = _unitMultiplier *((right - left) + _lensMeshGenerator.RadiusInput1.Value - _lensMeshGenerator.RadiusInput2.Value); //correcting for units

        foreach (var laserPointerScript in LaserPointers2)
        {
            var laserPointer = laserPointerScript.gameObject;
            Vector3 relLaserPos = gameObject.transform.InverseTransformPoint(laserPointer.transform.position);
            Vector3 relLaserDir = gameObject.transform.InverseTransformDirection(laserPointer.transform.up);
            var laserPointerProperties = laserPointer.GetComponent<Maroon.Physics.LaserPointer>();

            float intensity = laserPointerProperties.LaserIntensity;
            float wavelength = laserPointerProperties.LaserWavelength;

            OpticsRay laserRay = new OpticsRay(new Vector2(relLaserPos.x, relLaserPos.z), new Vector2(relLaserDir.x, relLaserDir.z), intensity, laserPointerProperties.LaserColor, wavelength);
            _thisLensOpticsSim.CalculateHitsRecursive(laserRay, currLens, true, ref _laserSegments, _loss, ReflectionVsRefraction);

        }
        UpdateLasers();

    }

    private void UpdateLasers()
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

    private void UpdateLR(OpticsSegment opticsSeg, LineRenderer Lr, bool useIntensity = true)
    {
        Lr.SetPosition(0, new Vector3( opticsSeg.P1.x, 0.0f, opticsSeg.P1.y)*_thisLens.transform.localScale.x + _thisLens.transform.position); 
        Lr.SetPosition(1, new Vector3( opticsSeg.P2.x, 0.0f, opticsSeg.P2.y)*_thisLens.transform.localScale.x + _thisLens.transform.position);
        if(useIntensity) Lr.material.SetColor("_TintColor", new Color(opticsSeg.SegmentColor.r, opticsSeg.SegmentColor.g, opticsSeg.SegmentColor.b, opticsSeg.Intensity));
    }

    public void SetCauchyVariables(int dropdownSelection) //todo maybe move this in extra class
    {
        if (dropdownSelection == 0) return;
        dropdownSelection--;
        Cauchy currMat = PhysicalConstants.CauchyValues[(CauchyMaterial)dropdownSelection];
        CauchyA.Value = currMat.A;
        CauchyB.Value = currMat.B;
    }
}

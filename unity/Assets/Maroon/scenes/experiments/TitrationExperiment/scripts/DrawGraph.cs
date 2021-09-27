using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Maroon.Physics;

public class DrawGraph : MonoBehaviour, IResetObject
{
    [SerializeField] private AcidTitration acidTitration;
    [SerializeField] private OpenBurette burette;

    private Dictionary<double, double> _result = new Dictionary<double, double>();
    private Dictionary<double, double> _equivalenzPoint = new Dictionary<double, double>();

    private RectTransform _rect;

    private const float MaxMl = 100.0f;
    private const float Ph = 14.0f;

    private int _fluidRestrictionSpeed = 10;
    private int _counter;

    private LineRenderer[] _lineRenderers;
    private LineRenderer _equivalenzLine;
    private LineRenderer _titrationCurveLine;

    private double _equivalenzPointKey;
    private ShowFluid _showFluidScript;

    // Display Panel values
    private QuantityFloat _volumeAddedPh = new QuantityFloat();
    private QuantityFloat _volumeAddedMl = new QuantityFloat();
    
    private float Height => _rect.rect.height;
    private float Width => _rect.rect.width;

    private void Start()
    {
        _showFluidScript = ShowFluid.Instance;
        _rect = GetComponent<RectTransform>();

        _lineRenderers = GetComponentsInChildren<LineRenderer>();
        _titrationCurveLine = _lineRenderers[0];
        _equivalenzLine = _lineRenderers[1];
    }

    public void Initialise()
    {
        _result = acidTitration.GetResultDictionary();
        _equivalenzPoint = acidTitration.GetEquivalenzPointDictionary();
        _equivalenzLine.positionCount = 3;


        if (_equivalenzPoint.Count > 0)
        {
            var temp = (int)(_equivalenzPoint.Keys.First() * 10);

            if ((_equivalenzPoint.Keys.First() * 10) % 1 != 0)
                temp += 1;

            _equivalenzPointKey = temp / 10.00;
        }
    }

    public void ResetObject()
    {
        _titrationCurveLine.positionCount = 0;

        if (_equivalenzLine != null)
            _equivalenzLine.positionCount = 0;

        _counter = 0;
        _volumeAddedMl.Value = 0f;
        _volumeAddedPh.Value = 0f;

        _result.Clear();
        _equivalenzPoint.Clear();
    }

    public IEnumerator DrawLine()
    {
        var prevCounter = 0;

        if (_result.Count > 0)
        {
            // Titration curve
            foreach (var entry in _result)
            {
                if (!burette.open)
                    yield return new WaitUntil(() => burette.open);

                if (_counter != 0)
                {
                    // Add the ml correctly: Possib. values to add [0.1 ml, 1 ml, 10 ml]
                    if (_fluidRestrictionSpeed != 1)
                    {
                        if (_counter % (_fluidRestrictionSpeed + prevCounter) == 0)
                        {
                            prevCounter = _counter;
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                    else
                    {
                        if (_counter % (_fluidRestrictionSpeed) == 0)
                        {
                            prevCounter = _counter;
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                }

                var tmpMl = ((float)entry.Key / MaxMl) * Width;
                var tmpPh = ((float)entry.Value / Ph) * Height;

                // Renders line
                _titrationCurveLine.positionCount = _counter + 1;
                _titrationCurveLine.SetPosition(_counter, new Vector3(tmpMl, tmpPh, 0));
                _counter++;

                _volumeAddedMl.Value = (float)entry.Key;
                _volumeAddedPh.Value = (float)entry.Value;

                _showFluidScript.DetermineAnalyteColor((float)entry.Value);

                if (_equivalenzPoint.Count > 0)
                {
                    if (entry.Key.Equals(_equivalenzPointKey))
                    {
                        // Equivalence point horizontal line
                        foreach (KeyValuePair<double, double> entryEqu in _equivalenzPoint)
                        {
                            var tmpMl_ = ((float)entryEqu.Key / MaxMl) * Width;
                            var tmpPh_ = ((float)entryEqu.Value / Ph) * Height;

                            var equivalenzlinewidth = (5 / MaxMl) * Width;

                            _equivalenzLine.SetPosition(0, new Vector3(tmpMl_ - equivalenzlinewidth, tmpPh_, 0));
                            _equivalenzLine.SetPosition(1, new Vector3(tmpMl_, tmpPh_, 0));
                            _equivalenzLine.SetPosition(2, new Vector3(tmpMl_ + equivalenzlinewidth, tmpPh_, 0));
                            break;
                        }
                    }
                }
            }
        }
    }

    public void SetStreamspeed(float value)
    {
        switch (value)
        {
            case 0:
                _fluidRestrictionSpeed = 1;
                break;
            case 1:
                _fluidRestrictionSpeed = 10;
                break;
            case 2:
                _fluidRestrictionSpeed = 100;
                break;
            default:
                break;
        }
    }
}



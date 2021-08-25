using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Maroon.Physics;
using TMPro;

[RequireComponent(typeof(TitrationController))]
public class AcidTitration : MonoBehaviour
{
    private readonly Dictionary<string, double> _weakAcids = new Dictionary<string, double>
    {
        {"CH3CO2H", 4.75}
    };

    private readonly Dictionary<string, double> _weakBases = new Dictionary<string, double>
    {
        {"NH3", 9.25}
    };

    private readonly Dictionary<double, double> result = new Dictionary<double, double>();
    private readonly Dictionary<double, double> equivalenzPoint = new Dictionary<double, double>();

    private TitrationController _titrationController;

    private double _maxRange;
    private bool _checkH2So4;
    private bool _weakAcid;
    private bool _weakBase;
    private bool _checkEquivalencePoint = true;
    private bool _baseToggleTitrant = true;
    private string _dropddownAcidText = "HCl";
    private string _dropdownBaseText = "NaOH";

    public string analyteText = "";

    private QuantityFloat _equivalenzPointPh = new QuantityFloat();
    private QuantityFloat _equivalenzPointMl = new QuantityFloat();

    private void Start()
    {
        _titrationController = GetComponent<TitrationController>();
    }

    public void Calculation(double molTitrant, double mlTitrant, double molAnalyte, double mlAnalyte)
    {
        _baseToggleTitrant = _titrationController.GetBaseToggleTitrant();

        _maxRange = mlTitrant;

        if (_checkH2So4)
        {
            if (_baseToggleTitrant)
                molAnalyte *= 2;
            else
                molTitrant *= 2;
        }

        var i = 0.00;

        while (i <= _maxRange)
        {
            var concentrationPreequivalence = ((mlAnalyte*0.001) * molAnalyte) - ((i*0.001) * molTitrant);
            concentrationPreequivalence /= ((mlAnalyte + i)*0.001);

            // equivalence point reached
            double ph;
            if (concentrationPreequivalence <= 0.0) 
            {

                if (_checkEquivalencePoint)
                {
                    ph = 7.0;
                    if (_weakAcid && !_weakBase) 
                    {
                        var pKb = 14.0 - _weakAcids[_dropddownAcidText]; // pKs + pKb = pKw
                        var X = ((i*0.001) * molTitrant) / ((mlAnalyte + i)*0.001);
                        ph = 0.5 * (pKb -Math.Log10(X));

                        ph = 14 - ph; // ph = 14 - pOH
                    }
                    if (_weakBase && !_weakAcid)
                    {
                        var X = ((i*0.001) * molTitrant) / ((mlAnalyte + i)*0.001);
                        ph = 0.5 * (_weakBases[_dropdownBaseText] - Math.Log10(X));
                    }
                    equivalenzPoint.Add(i, ph);

                    _equivalenzPointPh.Value = (float)ph;
                    _equivalenzPointMl.Value = (float)i;

                    _checkEquivalencePoint = false;
                }
                else // after equivalence point
                {
                    if((_weakAcid && !_baseToggleTitrant) || (_weakBase && _baseToggleTitrant))
                    {
                        if (!_baseToggleTitrant) // weak acid remains
                        {
                            ph = WeakTitrCalculation(i, molTitrant, molAnalyte, mlAnalyte, _weakAcids[_dropddownAcidText]);
                        }
                        else // weak base remains
                        {
                            double pKb = 14.0 - _weakBases[_dropdownBaseText];
                            ph = WeakTitrCalculation(i, molTitrant, molAnalyte, mlAnalyte, pKb);
                        }
                    }
                    else
                    {
                        // strong base or strong acid
                        var concentrationPostequivalence = ((i * 0.001) * molTitrant) - ((mlAnalyte * 0.001) * molAnalyte);
                        concentrationPostequivalence /= ((mlAnalyte + i) * 0.001);

                        ph = -Math.Log10(concentrationPostequivalence);
                    }

                    if (_baseToggleTitrant)
                        ph = 14 - ph;

                }
            } 
            else // before equivalence point
            {
                if (_weakAcid && _baseToggleTitrant) // weak acid with strong base
                {
                    if (i == 0) 
                    {
                        ph = 0.5 * (_weakAcids[_dropddownAcidText] - Math.Log10(molAnalyte)); // ph = 1/2 * (pKs - lg(c0))
                    }
                    else 
                    {
                        ph = WeakTitrCalculation(mlAnalyte, molAnalyte, molTitrant, i, _weakAcids[_dropddownAcidText]);
                    }
                }
                else if (_weakBase && !_baseToggleTitrant) 
                {
                    if (i == 0)
                    {
                        var pKb = 14.0 - _weakBases[_dropdownBaseText];
                        ph = 0.5 * (pKb - Math.Log10(molAnalyte)); // pOH = 1/2 * (pKb - lg(c0))
                    }
                    else 
                    {
                        var pKb = 14.0 - _weakBases[_dropdownBaseText];
                        ph = WeakTitrCalculation(mlAnalyte, molAnalyte, molTitrant, i, pKb);
                    }
                }
                else // strong acid  with strong base
                {
                    ph = -Math.Log10 (concentrationPreequivalence);
                }

                if (!_baseToggleTitrant)
                    ph = 14 - ph; // ph = 14 - pOH

            }

            if (ph < 0)
                ph = 0;

            if ((i * 10) % 1 == 0)
            {
                result.Add(i, ph);
            }

            i += 0.01D;
            i = Math.Round(i, 2);
        }
    }

    public double WeakTitrCalculation(double mlAnalyte_, double molAnalyte_, double molTitrant_, double i_, double pK)
    {
        var HX = ((mlAnalyte_ * 0.001) * molAnalyte_) - ((i_ * 0.001) * molTitrant_);
        HX /= ((mlAnalyte_ + i_) * 0.001);
        var X = ((i_ * 0.001) * molTitrant_) / ((mlAnalyte_ + i_) * 0.001);
        // double ph_ = pK - Math.Log10(HX / X); // ph = pKs - lg(HX / X) ---> Approximation

        var Ks = Math.Pow(10, -pK);
        var Kw = Math.Pow(10, -14.0);
        var x_ = 0.0012;
        var counter = 0;

        // iterative method of finding H+ in solution
        while (counter < 50)
        {
            x_ = (Ks * (HX - x_ + (Kw/x_))) / (X + x_ - (Kw/x_)); // yields exact titration curve
            counter++;
        }
        var ph_ = -Math.Log10(x_);


        if (_weakAcid && _weakBase)
        {
            var X2 = ((mlAnalyte_ * 0.001) * molAnalyte_) - ((i_ * 0.001) * molTitrant_);
            var tmp_ph = 0.5 * ((14 - pK) - Math.Log10(X2)); // c0 << Ks

            if (ph_ >= tmp_ph)
                ph_ = tmp_ph;

            if (X2 <= Ks)
            {
                var Ks_tmp = Math.Pow(10, -(pK - 0.01));
                ph_ = 0.5 * ((14-pK) - Math.Log10(Ks_tmp));
            }
        }

        return ph_;
    }


    public void ValidateBaseDropdown(TMP_Text label)
    {
        _dropdownBaseText = label.text;
        _weakBase = false;

        switch (_dropdownBaseText) {
            case "NaOH":
                break;
            case "KOH":
                break;
            case "NH3":
                _weakBase = true;
                break;
            default:
                break;
        }

        if (!_baseToggleTitrant)
            analyteText = _dropdownBaseText;
    }

    public void ValidateAcidDropdown(TMP_Text label)
    {
        _dropddownAcidText = label.text;
        _weakAcid = false;
        _checkH2So4 = false;

        switch (_dropddownAcidText)
        {
            case "HCl":
                break;
            case "HNO3":
                break;
            case "H2SO4":
                _checkH2So4 = true;
                break;
            case "CH3CO2H":
                _weakAcid = true;
                break;
            default:
                break;
        }

        if (_baseToggleTitrant)
            analyteText = _dropddownAcidText;
    }

    public void ChangeAnalyteText(bool param)
    {
        analyteText = param ? _dropddownAcidText : _dropdownBaseText;
    }

    public Dictionary<double, double> GetResultDictionary()
    {
        return result;
    }

    public Dictionary<double, double> GetEquivalenzPointDictionary()
    {
        return equivalenzPoint;
    }

    public void ResetEverything()
    {
        result.Clear();
        equivalenzPoint.Clear();

        _checkH2So4 = false;
        _weakAcid = false;
        _weakBase = false;
        _checkEquivalencePoint = true;
        analyteText = "HCl";
    }
}

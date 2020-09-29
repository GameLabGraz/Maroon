using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using PlatformControls.PC;
using System.Linq;
using System;

public class AcidTitration : MonoBehaviour
{
	
	Dictionary<string, int> strongBases = new Dictionary<string, int>() {
		{"NaOH", 0}, 
		{"KOH", 1} };
	Dictionary<string, int> strongAcids = new Dictionary<string, int>() {
		{"HCl", 0},
		{"HNO3", 1},
		{"H2SO4", 2} };
	Dictionary<string, double> weakAcids = new Dictionary<string, double>() {
		{"CH3CO2H", 4.75} };

	Dictionary<string, double> weakBases = new Dictionary<string, double>() {
		{"NH3", 9.25} };

	Dictionary<double, double> result = new Dictionary<double, double>();
	Dictionary<double, double> equivalenzPoint = new Dictionary<double, double>();

	private double maxRange = 0;
	private bool checkH2SO4 = false;
	private bool weakAcid = false;
	private bool weakBase = false;
    private bool checkEquivalencePoint = true;
	private bool baseToggleTitrant = true;
	private string dropddownAcidText = "HCl";
	private string dropdownBaseText = "NaOH";

	public string analytText = "";

	public void initialise(double molTitrant, double mlTitrant, double molAnalyt, double mlAnalyt)
	{
		baseToggleTitrant = gameObject.GetComponent<GameController>().getBaseToggleTitrant();
		Calculation(molTitrant, mlTitrant, molAnalyt, mlAnalyt);
	}

	public void Calculation(double molTitrant, double mlTitrant, double molAnalyt, double mlAnalyt)
  {
        double concentration_preequivalence = 0;
		double concentration_postequivalence = 0;
		double ph = 0;
        maxRange = mlTitrant + mlAnalyt;


		double prev_concentration_preequivalence = 0;

		if (checkH2SO4)
		{
			if (baseToggleTitrant)
				molAnalyt *= 2;
			else
				molTitrant *= 2;
		}

		double i = 0.00;

		while ( i <= maxRange)
		{
			concentration_preequivalence = ((mlAnalyt*0.001) * molAnalyt) - ((i*0.001) * molTitrant);
			concentration_preequivalence /= ((mlAnalyt + i)*0.001);


			// equivalence point reached
			if (concentration_preequivalence <= 0.0) 
			{

                if (checkEquivalencePoint)
				{
					ph = 7.0;
					if (weakAcid && !weakBase) 
					{
						double pKb = 14.0 - weakAcids[dropddownAcidText]; // pKs + pKb = pKw
						double X = ((i*0.001) * molTitrant) / ((mlAnalyt + i)*0.001);
						ph = 0.5 * (pKb -Math.Log10(X));

						ph = 14 - ph; // ph = 14 - pOH
					}
					if (weakBase && !weakAcid)
					{
						double X = ((i*0.001) * molTitrant) / ((mlAnalyt + i)*0.001);
						ph = 0.5 * (weakBases[dropdownBaseText] - Math.Log10(X));
					}
					equivalenzPoint.Add(i, ph);
                    checkEquivalencePoint = false;
                }
				else // after equivalence point
				{
					if((weakAcid && !baseToggleTitrant) || (weakBase && baseToggleTitrant))
					{
						if (!baseToggleTitrant) // weak acid remains
						{
							ph = weakTitrCalculation(i, molTitrant, molAnalyt, mlAnalyt, weakAcids[dropddownAcidText]);
						}
						else // weak base remains
						{
							double pKb = 14.0 - weakBases[dropdownBaseText];
							ph = weakTitrCalculation(i, molTitrant, molAnalyt, mlAnalyt, pKb);
						}
					}
					else
					{
						// strong base or strong acid
						concentration_postequivalence = ((i * 0.001) * molTitrant) - ((mlAnalyt * 0.001) * molAnalyt);
						concentration_postequivalence /= ((mlAnalyt + i) * 0.001);

						ph = -Math.Log10(concentration_postequivalence);
					}

					if (baseToggleTitrant)
						ph = 14 - ph;

				}
				prev_concentration_preequivalence = concentration_preequivalence;
			} 
			else // before equivalence point
			{
				if (weakAcid && baseToggleTitrant) // weak acid with strong base
				{
					if (i == 0) 
					{
						ph = 0.5 * (weakAcids[dropddownAcidText] - Math.Log10(molAnalyt)); // ph = 1/2 * (pKs - lg(c0))
					}
					else 
					{
						ph = weakTitrCalculation(mlAnalyt, molAnalyt, molTitrant, i, weakAcids[dropddownAcidText]);
					}
				}
				else if (weakBase && !baseToggleTitrant) 
				{
					if (i == 0)
					{
						double pKb = 14.0 - weakBases[dropdownBaseText];
						ph = 0.5 * (pKb - Math.Log10(molAnalyt)); // pOH = 1/2 * (pKb - lg(c0))
					}
					else 
					{
						double pKb = 14.0 - weakBases[dropdownBaseText];
						ph = weakTitrCalculation(mlAnalyt, molAnalyt, molTitrant, i, pKb);
					}
				}
				else // strong acid  with strong base
				{
					ph = -Math.Log10 (concentration_preequivalence);
				}

				if (!baseToggleTitrant)
					ph = 14 - ph; // ph = 14 - pOH

			}

			if (ph < 0)
				ph = 0;

            if ((i * 10) % 1 == 0)
            {
                result.Add(i, ph);
				//Debug.Log("ph: " + ph + " ml: " + i);
			}

            i += 0.01D;
			i = Math.Round(i, 2);
		}
	}

	public double weakTitrCalculation(double mlAnalyt_, double molAnalyt_, double molTitrant_, double i_, double pK)
	{
		double HX = ((mlAnalyt_ * 0.001) * molAnalyt_) - ((i_ * 0.001) * molTitrant_);
		HX /= ((mlAnalyt_ + i_) * 0.001);
		double X = ((i_ * 0.001) * molTitrant_) / ((mlAnalyt_ + i_) * 0.001);
		double ph_ = pK - Math.Log10(HX / X); // ph = pKs - lg(HX / X)

		if (weakAcid && weakBase)
		{
			double X2 = ((mlAnalyt_ * 0.001) * molAnalyt_) - ((i_ * 0.001) * molTitrant_);
			double Ks = Math.Pow(10, -pK);
			double tmp_ph = 0.5 * ((14 - pK) - Math.Log10(X2)); // c0 << Ks

			if (ph_ >= tmp_ph)
				ph_ = tmp_ph;

			if (X2 <= Ks)
			{
				double Ks_tmp = Math.Pow(10, -(pK - 0.01));
				ph_ = 0.5 * ((14-pK) - Math.Log10(Ks_tmp));
			}
		}

		return ph_;
	}

	public void validateBaseDropdown(Text label)
	{
		dropdownBaseText = label.text;

		switch (dropdownBaseText) {
			case "NaOH":
				break;
			case "KOH":
				break;
			case "NH3":
				weakBase = true;
				break;
			default:
				break;
		}

		if (!baseToggleTitrant)
			analytText = dropdownBaseText;
	}

	public void validateAcidDropdown(Text label)
	{
		dropddownAcidText = label.text;

		switch (dropddownAcidText)
		{
			case "HCl":
				break;
			case "HNO3":
				break;
			case "H2SO4":
				checkH2SO4 = true;
				break;
			case "CH3CO2H":
				weakAcid = true;
				break;
			default:
				break;
		}

		if (baseToggleTitrant)
			analytText = dropddownAcidText;
	}

	public Dictionary<double, double> getResultDictionary()
	{
		return result;
	}

	public Dictionary<double, double> getEquivalenzPointDictionary()
	{
		return equivalenzPoint;
	}

	public void resetEverything()
	{
		result.Clear();
		equivalenzPoint.Clear();

		checkH2SO4 = false;
		weakAcid = false;
		weakBase = false;
		checkEquivalencePoint = true;
		analytText = "";
	}

	public void getEquivalenzPointPh(MessageArgs args)
	{
		if (equivalenzPoint.Count == 0)
			args.value = 0f;
		else
			args.value = (float)equivalenzPoint.First().Value;
	}

	public void getEquivalenzPointMl(MessageArgs args)
	{
		if (equivalenzPoint.Count == 0)
			args.value = 0f;
		else
			args.value = (float)equivalenzPoint.First().Key;

	}
}

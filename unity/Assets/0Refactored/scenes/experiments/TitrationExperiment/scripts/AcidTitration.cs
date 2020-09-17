using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using PlatformControls.PC;
using System.Linq;

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
		{"NH3", 4.75} };

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
					ph = 7.0f;
					if (weakAcid) 
					{
						double pKb = 14.0 - weakAcids[dropddownAcidText]; // pKs + pKb = pKw
						double X = ((i*0.001) * molTitrant) / ((mlAnalyt + i)*0.001);
						double pOH = 0.5 * (pKb -Mathf.Log10((float)X));
						ph = 14.0 - pOH;

						if (!baseToggleTitrant)
							ph = 14 - ph; // ph = 14 - pOH
					}
					if (weakBase)
					{
						double pKs = 14.0 - weakBases[dropdownBaseText];
						double X = ((i*0.001) * molTitrant) / ((mlAnalyt + i)*0.001);
						ph = 0.5 * (pKs -Mathf.Log10((float)X));

						if (baseToggleTitrant)
							ph = 14 - ph; // ph = 14 - pOH
					}
					equivalenzPoint.Add(i, ph);
                    checkEquivalencePoint = false;
                }
				else // after equivalence point
				{
					concentration_postequivalence = ((i * 0.001) * molTitrant) - ((mlAnalyt * 0.001) * molAnalyt);
					concentration_postequivalence /= ((mlAnalyt + i) * 0.001);

					double pOH = -Mathf.Log10((float)concentration_postequivalence);

					if (baseToggleTitrant)
						ph = 14 - pOH;
					else
						ph = pOH;
				}
				prev_concentration_preequivalence = concentration_preequivalence;
			} 
			else // before equivalence point
			{
				if (weakAcid) // weak acid with strong base
				{
					if (i == 0) 
					{
						ph = 0.5 * (weakAcids[dropddownAcidText] - Mathf.Log10((float)molAnalyt)); // ph = 1/2 * (pKs - lg(c0))
					}
					else 
					{
						ph = weakTitrCalculation(mlAnalyt, molAnalyt, molTitrant, i, dropddownAcidText, weakAcids);
					}
				}
				else if (weakBase) 
				{
					if (i == 0)
					{
						ph = 0.5 * (weakBases[dropdownBaseText] - Mathf.Log10((float)molAnalyt)); // pOH = 1/2 * (pKb - lg(c0))
					}
					else 
					{
						ph = weakTitrCalculation(mlAnalyt, molAnalyt, molTitrant, i, dropdownBaseText, weakBases);
					}
				}
				else // strong acid  with strong base
				{
					ph = -Mathf.Log10 ((float)concentration_preequivalence);
				}

				if (!baseToggleTitrant)
					ph = 14 - ph; // ph = 14 - pOH

			}

			if (ph < 0)
				ph = 0;

            if ((i * 10) % 1 == 0)
            {
                result.Add(i, ph);
            }

            i += 0.01D;
			i = System.Math.Round(i, 2);
		}
	}

	public double weakTitrCalculation(double mlAnalyt_, double molAnalyt_, double molTitrant_, double i_, string dropDownText, Dictionary<string, double> weakDict)
	{
		double HX = ((mlAnalyt_ * 0.001) * molAnalyt_) - ((i_ * 0.001) * molTitrant_);
		HX /= ((mlAnalyt_ + i_) * 0.001);
		double X = ((i_ * 0.001) * molTitrant_);
		X /= ((mlAnalyt_ + i_) * 0.001);
		double ph_ = weakDict[dropDownText] - Mathf.Log10((float)(HX / X)); // ph = pKs - lg(HX / X)

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

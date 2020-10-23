using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
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

	public string analyteText = "";

	public void calculation(double molTitrant, double mlTitrant, double molAnalyte, double mlAnalyte)
  {
		baseToggleTitrant = gameObject.GetComponent<GameController>().getBaseToggleTitrant();

		double concentration_preequivalence = 0;
		double concentration_postequivalence = 0;
		double ph = 0;
        maxRange = mlTitrant;


		double prev_concentration_preequivalence = 0;

		if (checkH2SO4)
		{
			if (baseToggleTitrant)
				molAnalyte *= 2;
			else
				molTitrant *= 2;
		}

		double i = 0.00;

		while ( i <= maxRange)
		{
			concentration_preequivalence = ((mlAnalyte*0.001) * molAnalyte) - ((i*0.001) * molTitrant);
			concentration_preequivalence /= ((mlAnalyte + i)*0.001);


			// equivalence point reached
			if (concentration_preequivalence <= 0.0) 
			{

                if (checkEquivalencePoint)
				{
					ph = 7.0;
					if (weakAcid && !weakBase) 
					{
						double pKb = 14.0 - weakAcids[dropddownAcidText]; // pKs + pKb = pKw
						double X = ((i*0.001) * molTitrant) / ((mlAnalyte + i)*0.001);
						ph = 0.5 * (pKb -Math.Log10(X));

						ph = 14 - ph; // ph = 14 - pOH
					}
					if (weakBase && !weakAcid)
					{
						double X = ((i*0.001) * molTitrant) / ((mlAnalyte + i)*0.001);
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
							ph = weakTitrCalculation(i, molTitrant, molAnalyte, mlAnalyte, weakAcids[dropddownAcidText]);
						}
						else // weak base remains
						{
							double pKb = 14.0 - weakBases[dropdownBaseText];
							ph = weakTitrCalculation(i, molTitrant, molAnalyte, mlAnalyte, pKb);
						}
					}
					else
					{
						// strong base or strong acid
						concentration_postequivalence = ((i * 0.001) * molTitrant) - ((mlAnalyte * 0.001) * molAnalyte);
						concentration_postequivalence /= ((mlAnalyte + i) * 0.001);

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
						ph = 0.5 * (weakAcids[dropddownAcidText] - Math.Log10(molAnalyte)); // ph = 1/2 * (pKs - lg(c0))
					}
					else 
					{
						ph = weakTitrCalculation(mlAnalyte, molAnalyte, molTitrant, i, weakAcids[dropddownAcidText]);
					}
				}
				else if (weakBase && !baseToggleTitrant) 
				{
					if (i == 0)
					{
						double pKb = 14.0 - weakBases[dropdownBaseText];
						ph = 0.5 * (pKb - Math.Log10(molAnalyte)); // pOH = 1/2 * (pKb - lg(c0))
					}
					else 
					{
						double pKb = 14.0 - weakBases[dropdownBaseText];
						ph = weakTitrCalculation(mlAnalyte, molAnalyte, molTitrant, i, pKb);
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

	public double weakTitrCalculation(double mlAnalyte_, double molAnalyte_, double molTitrant_, double i_, double pK)
	{
		double HX = ((mlAnalyte_ * 0.001) * molAnalyte_) - ((i_ * 0.001) * molTitrant_);
		HX /= ((mlAnalyte_ + i_) * 0.001);
		double X = ((i_ * 0.001) * molTitrant_) / ((mlAnalyte_ + i_) * 0.001);
		// double ph_ = pK - Math.Log10(HX / X); // ph = pKs - lg(HX / X) ---> Approximation

		double Ks = Math.Pow(10, -pK);
		double Kw = Math.Pow(10, -14.0);
		double x_ = 0.0012;
		int counter = 0;

		// iterative method of finding H+ in solution
		while (counter < 50)
		{
			x_ = (Ks * (HX - x_ + (Kw/x_))) / (X + x_ - (Kw/x_)); // yields exact titration curve
			counter++;
		}
		double ph_ = -Math.Log10(x_);


		if (weakAcid && weakBase)
		{
			double X2 = ((mlAnalyte_ * 0.001) * molAnalyte_) - ((i_ * 0.001) * molTitrant_);
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
		weakBase = false;

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
			analyteText = dropdownBaseText;
	}

	public void validateAcidDropdown(Text label)
	{
		dropddownAcidText = label.text;
		weakAcid = false;
		checkH2SO4 = false;

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
			analyteText = dropddownAcidText;
	}

	public void changeAnalyteText(bool param)
	{
		if (param)
			analyteText = dropddownAcidText;
		else
			analyteText = dropdownBaseText;
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
		analyteText = "HCl";
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

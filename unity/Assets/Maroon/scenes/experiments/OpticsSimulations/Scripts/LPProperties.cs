using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LPProperties : MonoBehaviour
{

    public Color LaserColor = Color.red;
    public float LaserIntensity = 1.0f;
    public float LaserWavelength = 680f;

    public Color[] spectrumcolors = new Color[20];

    private Color linearInterpolate(Color col1, Color col2, float interpolfactor)
    {
        return col1*(1-interpolfactor) + col2*(interpolfactor);
    }

    public void SetLaserColor()
    {
        SetLaserColor(LaserWavelength);
    }

    public void SetLaserColor(float wavelength)
    {
        //wavelength between 0 and 1
        //float wavelength2 = (wavelength - 400.0f) / 300.0f;
        // 380-720nm
        wavelength = (wavelength - 380.0f) / 340.0f;
        wavelength = Mathf.Clamp(wavelength, 0.0f, 0.9999f);
        // should set color of laser depending on its wavelength.

        List<Color> colors = new List<Color>();

        colors.AddRange(spectrumcolors);

        int toInterpolate = colors.Count - 1;

        float interpolationFactor = wavelength * toInterpolate;
        int firstColorIndex = (int)Mathf.Floor(interpolationFactor);

        float interpolatePercent = interpolationFactor - firstColorIndex;

        Color interpolatedColor = linearInterpolate(colors[firstColorIndex], colors[firstColorIndex + 1], interpolatePercent);

        LaserColor = interpolatedColor;
        return;
    }


    public float GetCauchyForCurrentWavelength()
    {

        OpticsLens currentLens = FindObjectOfType<LaserRenderer>().CurrentLens;

        OpticsSim os = FindObjectOfType<OpticsSim>();

        return os.GetCauchy(currentLens.cauchyA, currentLens.cauchyB, LaserWavelength);
    }

}

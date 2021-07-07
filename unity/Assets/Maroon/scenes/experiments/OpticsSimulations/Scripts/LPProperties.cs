using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LPProperties : MonoBehaviour
{

    public Color laserColor = Color.red;
    public float laserIntensity = 1.0f;
    public float laserWavelength = 680f;

    public Color[] spectrumcolors = new Color[20];

    private Color linearInterpolate(Color col1, Color col2, float interpolfactor)
    {
        return col1*(1-interpolfactor) + col2*(interpolfactor);
    }

    public void setLaserColor()
    {
        setLaserColor(laserWavelength);
    }

    public float getCauchyForCurrentWavelength()
    {

        OpticsLens currLens = (OpticsLens) FindObjectOfType<LaserRenderer>().currentLens;

        OpticsSim os = FindObjectOfType<OpticsSim>();

        return os.getCauchy(currLens.innerRefractiveidx, currLens.outerRefractiveidx, laserWavelength);
    }
    public void setLaserColor(float wavelength)
    {
        //wavelength between 0 and 1
        //float wavelength2 = (wavelength - 400.0f) / 300.0f;
        // 380-720nm
        wavelength = (wavelength - 380.0f) / 340.0f;
        wavelength = Mathf.Clamp(wavelength, 0.0f, 0.9999f);
        // should set color of laser depending on its wavelength.

        List<Color> Colors = new List<Color>();

        Colors.AddRange(spectrumcolors);

        int toInterpolate = Colors.Count - 1;

        float interpolfactor = wavelength * toInterpolate;
        int firstintcolor = (int) Mathf.Floor(interpolfactor);

        float tointerpol = interpolfactor - firstintcolor;

        Color intpold = linearInterpolate(Colors[firstintcolor], Colors[firstintcolor + 1], tointerpol);

        laserColor = intpold;
        return;
    }
}

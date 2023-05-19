using UnityEngine;
using UnityEngine.UI;

public class PlanetInfoDisplay : MonoBehaviour
{
    public Text textPlanetInfo0;
    public Text textPlanetInfo1;
    public Text textPlanetInfo2;
    public Text textPlanetInfo3;
    public Text textPlanetInfo4;
    public Text textPlanetInfo5;
    public Text textPlanetInfo6;
    public Text textPlanetInfo7;
    public Text textPlanetInfo8;
    public Text textPlanetInfo9;
    public Text textPlanetInfo10;
    public Text textPlanetInfo11;
    public Text textPlanetInfo12;
    public Text textPlanetInfo13;
    public Text textPlanetInfo14;
    public Text textPlanetInfo15;
    public Text textPlanetInfo16;
    public Text textPlanetInfo17;
    public Text textPlanetInfo18;
    public Text textPlanetInfo19;
    public Text textPlanetInfo20;
    //... continue this for all your data ...

    public void UpdatePlanetInfo(PlanetInfo planetInfo)
    {
        // Update the text fields using the data from PlanetInfo
        textPlanetInfo0.text += planetInfo.PlanetInformationOf.ToString();
        textPlanetInfo1.text += planetInfo.mass.ToString();
        textPlanetInfo2.text += planetInfo.diameter.ToString();
        textPlanetInfo3.text += planetInfo.density.ToString();
        textPlanetInfo4.text += planetInfo.gravity.ToString();
        textPlanetInfo5.text += planetInfo.escapeVelocity.ToString();
        textPlanetInfo6.text += planetInfo.rotationPeriod.ToString();
        textPlanetInfo7.text += planetInfo.lengthOfDay.ToString();
        textPlanetInfo8.text += planetInfo.distanceFromSun.ToString();
        textPlanetInfo9.text += planetInfo.perihelion.ToString();
        textPlanetInfo10.text += planetInfo.aphelion.ToString();
        textPlanetInfo11.text += planetInfo.orbitalPeriod.ToString();
        textPlanetInfo12.text += planetInfo.orbitalVelocity.ToString();
        textPlanetInfo13.text += planetInfo.orbitalInclination.ToString();
        textPlanetInfo14.text += planetInfo.orbitalEccentricity.ToString();
        textPlanetInfo15.text += planetInfo.obliquityToOrbit.ToString();
        textPlanetInfo16.text += planetInfo.meanTemperature.ToString();
        textPlanetInfo17.text += planetInfo.surfacePressure.ToString();
        textPlanetInfo18.text += planetInfo.numberOfMoons.ToString();
        textPlanetInfo19.text += planetInfo.ringSystem.ToString();
        textPlanetInfo20.text += planetInfo.globalMagneticField.ToString();
    }
}

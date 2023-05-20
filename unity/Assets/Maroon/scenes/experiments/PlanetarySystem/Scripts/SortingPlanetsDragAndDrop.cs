using UnityEngine;
using GEAR.Localization;    //MLG
using TMPro;

public class SortingPlanetsDragAndDrop : MonoBehaviour, IResetObject
{
    public AudioSource audioSource;
    public AudioClip pickUpClip;
    public AudioClip dropClip;

    public Transform sortingPlanetTarget; 
    public float snapDistance;  
    private bool isSnapped = false;
    private readonly float scaleFactor = 1.02f;
    public Light sunLightHalo;
    Vector3 mousePosition;

    public TextMeshProUGUI planetInfoMessageText;
    string createdPlanetInfoMessage;


    /*
     * handle mouse input
     */
    #region MouseInput
    /*
     * get mouse position
     */
    private void OnMouseDown()
    {
        if (!isSnapped)
        {
            audioSource.PlayOneShot(pickUpClip);
            mousePosition = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);

            // Show PlanetInfo data when planet is picked up
            CreatePlanetInfoMessage();
            planetInfoMessageText.text = createdPlanetInfoMessage;

        }
    }


    /*
    * calculate position
    */
    private void OnMouseDrag()
    {
        if (!isSnapped)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
            transform.position = newPosition;
        }
    }


    /*
     * snap to target when distance is smaller han snap distance
     * or snap back to start slot
     */
    private void OnMouseUp()
    {
        if (!isSnapped)
        {
            Vector2 distanceVector = new Vector2(transform.position.x - sortingPlanetTarget.position.x, transform.position.y - sortingPlanetTarget.position.y);
                        
            //Debug.Log("SortingPlanetsDragAndDrop(): distanceVector.magnitude: " + distanceVector.magnitude);
            if (distanceVector.magnitude <= snapDistance)
            {
                SnapToTarget();
            }
            else
            {
                transform.position = transform.parent.position;
                isSnapped = false;
            }
        }
    }
    #endregion MouseInput


    /*
     * snap the planets to target
     */
    #region Snap
    /*
     * increments the snapped planets and displays a Helpi message when all planets except pluto are in its place 
     */
    private void IncrementSnappedPlanetCount()
    {
        int solarSystemPlanet = 10;
        PlanetaryController.Instance.sortedPlanetCount++;

        // check if we have snapped all 10 out of 11 planets, excluding pluto, including moon ad sun
        if (PlanetaryController.Instance.sortedPlanetCount >= solarSystemPlanet)
        {
            PlanetaryController.Instance.DisplayMessageByKey("OrderedSortingGame");
        }
    }


    /*
     * snaps the planets to target by setting its parent and scales them
     */
    private void SnapToTarget()
    {
        transform.SetParent(sortingPlanetTarget);
        transform.position = sortingPlanetTarget.position;
        transform.localScale = new Vector3(1,1,1) * scaleFactor;
        if (sunLightHalo != null)
            sunLightHalo.range = 1.75f;


        isSnapped = true;
        IncrementSnappedPlanetCount();
        audioSource.PlayOneShot(dropClip);
    }
    #endregion Snap


    /*
     * CreatePlanetInfoMessage from PlanetInfo for whole PlanetInfoUI
     */
    #region PlanetInfoMessage
    /*
     * get key from LanguageManager
     */
    string GetMessagByKey(string key)
    {
        return LanguageManager.Instance.GetString(key);
    }


    /*
     * CreatePlanetInfoMessage from PlanetInfo for whole PlanetInfoUI
     * pluto is not a planet anymore
     */
    void CreatePlanetInfoMessage()
    {
        string planetInfoMessage = "";
        PlanetInfo planetInfo = GetComponent<PlanetInfo>();
        string messageTMP;

        //pluto is not a planet
        if (planetInfo.PlanetInformationOf == PlanetInformation.pluto_10)
        {
            messageTMP = GetMessagByKey("PlanetInfo0");
            planetInfoMessage += " " + messageTMP + " " + GetMessagByKey(planetInfo.PlanetInformationOf.ToString()) + "\n\n";

            messageTMP = GetMessagByKey("PlutoNotAPlanet");
            planetInfoMessage += " " + messageTMP + " " + "\n\n";

            messageTMP = GetMessagByKey("PlanetInfo6");
            planetInfoMessage += " " + messageTMP + " " + planetInfo.rotationPeriod + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo15");
            planetInfoMessage += " " + messageTMP + " " + planetInfo.obliquityToOrbit + "\n\n";

            createdPlanetInfoMessage = planetInfoMessage;
            return;
        }

        messageTMP = GetMessagByKey("PlanetInfo0");
        //Debug.Log("SortingPlanetsDragAndDrop: CreatePlanetInfoMessage(): " + messageTMP);
        planetInfoMessage += " " + messageTMP + " " + GetMessagByKey(planetInfo.PlanetInformationOf.ToString()) + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo1");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.mass + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo2");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.diameter + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo3");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.density + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo4");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.gravity + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo5");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.escapeVelocity + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo6");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.rotationPeriod + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo7");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.lengthOfDay + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo8");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.distanceFromSun + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo9");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.perihelion + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo10");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.aphelion + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo11");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.orbitalPeriod + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo12");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.orbitalVelocity + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo13");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.orbitalInclination + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo14");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.orbitalEccentricity + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo15");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.obliquityToOrbit + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo16");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.meanTemperature + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo17");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.surfacePressure + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo18");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.numberOfMoons + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo19");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.ringSystem + "\n\n";
        messageTMP = GetMessagByKey("PlanetInfo20");
        planetInfoMessage += " " + messageTMP + " " + planetInfo.globalMagneticField + "\n";

        //combined message
        createdPlanetInfoMessage = planetInfoMessage;
    }
    #endregion PlanetInfoMessage


    /*
     * reset
     */
    #region Reset
    /*
     * reset sortingPlanet parent, scale, position, sortedPlanetCount
     */
    public void ResetObject()
    {
        createdPlanetInfoMessage = "PlanetDescription";
        planetInfoMessageText.text = GetMessagByKey(createdPlanetInfoMessage);

        transform.localScale = new Vector3(1, 1, 1);
        PlanetaryController.Instance.ResetSortingGame();
        if (sunLightHalo != null)
            sunLightHalo.range = 0.25f;

        isSnapped = false;
        PlanetaryController.Instance.sortedPlanetCount = 0;
    }
    #endregion Reset
}

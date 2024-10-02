using UnityEngine;
using GEAR.Localization;    //MLG
using TMPro;


namespace Maroon.Experiments.PlanetarySystem
{
    public class SortingPlanetsDragAndDrop : MonoBehaviour, IResetObject
    {
        public AudioSource audioSource;
        public AudioClip pickUpClip;
        public AudioClip dropClip;

        public PlanetRotation planetRotation;
        public Transform sortingPlanetTarget;
        public float snapDistance;
        private bool isSnapped = false;
        private readonly float scaleFactor = 1.02f;
        public Light sunLightHalo;
        Vector3 mousePosition;

        public TextMeshProUGUI planetInfoMessageText;
        string createdPlanetInfoMessage;
        string createdNasaDataMessage;


        /*
         * handle mouse input
         */
        #region MouseInput
        /*
         * get mouse position
         */
        private void OnMouseDown()
        {
            audioSource.PlayOneShot(pickUpClip);
            mousePosition = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            // Show PlanetInfo data when planet is clicked on
            CreateUnnamedPlanetInfoMessage();
            planetInfoMessageText.text = createdPlanetInfoMessage;
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
        #region SnapPlanet
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
            transform.localScale = new Vector3(1, 1, 1) * scaleFactor;
            if (sunLightHalo != null)
                sunLightHalo.range = 1.75f;

            planetRotation.SetObliquityToOrbit();

            isSnapped = true;
            IncrementSnappedPlanetCount();
            audioSource.PlayOneShot(dropClip);

            CreatePlanetInfoMessage();
            planetInfoMessageText.text = createdPlanetInfoMessage;
        }
        #endregion SnapPlanet


        /*
         * CreatePlanetInfoMessage from PlanetInfo for whole PlanetInfoUI
         */
        #region PlanetInfoMessage
        /*
         * get key from LanguageManager
         */
        private string GetMessagByKey(string key)

        {
            return LanguageManager.Instance.GetString(key);
        }


        /*
        * CreateNasaDataMessage from PlanetInfo for the rest of the PlanetInfoUI
        */
        private void CreateNasaDataMessage()
        {
            string nasaDataMessage = "";
            PlanetInfo planetInfo = GetComponent<PlanetInfo>();
            string messageTMP;

            //Debug.Log("SortingPlanetsDragAndDrop: CreatePlanetInfoMessage(): " + messageTMP);
            messageTMP = GetMessagByKey("PlanetInfo1");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.mass + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo2");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.diameter + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo3");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.density + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo4");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.gravity + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo5");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.escapeVelocity + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo6");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.rotationPeriod + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo7");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.lengthOfDay + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo8");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.distanceFromSun + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo9");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.perihelion + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo10");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.aphelion + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo11");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.orbitalPeriod + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo12");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.orbitalVelocity + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo13");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.orbitalInclination + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo14");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.orbitalEccentricity + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo15");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.obliquityToOrbit + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo16");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.meanTemperature + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo17");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.surfacePressure + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo18");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.numberOfMoons + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo19");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.ringSystem + "\n\n";
            messageTMP = GetMessagByKey("PlanetInfo20");
            nasaDataMessage += " " + messageTMP + " " + planetInfo.globalMagneticField + "\n";

            createdNasaDataMessage = nasaDataMessage;
        }


        /*
        * CreatePlanetInfoMessage from PlanetInfo for whole PlanetInfoUI without name when not snapped
        * pluto is not a planet anymore
        */
        private void CreateUnnamedPlanetInfoMessage()
        {
            string planetInfoMessage = "";
            PlanetInfo planetInfo = GetComponent<PlanetInfo>();
            string messageTMP;

            if (isSnapped)
            {
                messageTMP = GetMessagByKey("PlanetInfo0");
                planetInfoMessage += " " + messageTMP + " " + GetMessagByKey(planetInfo.PlanetInformationOf.ToString()) + "\n\n";
            }
            //pluto is not a planet
            else if(planetInfo.PlanetInformationOf == PlanetInformation.Pluto)
             {
                messageTMP = GetMessagByKey("PlanetInfo0");
                planetInfoMessage += " " + messageTMP + " " + GetMessagByKey(planetInfo.PlanetInformationOf.ToString()) + "\n\n";

                messageTMP = GetMessagByKey("PlutoNotAPlanet");
                planetInfoMessage += " " + messageTMP + " " + "\n\n";
            }
            // celestial body = ???
            else
            {
                messageTMP = GetMessagByKey("PlanetInfo0");
                planetInfoMessage += " " + messageTMP + " ???\n\n";
            }

            //combined message
            CreateNasaDataMessage();
            createdPlanetInfoMessage = planetInfoMessage + createdNasaDataMessage;
        }


       /*
        * CreatePlanetInfoMessage from PlanetInfo for whole PlanetInfoUI when snapped
        */
        private void CreatePlanetInfoMessage()
        {
            string planetInfoMessage = "";
            PlanetInfo planetInfo = GetComponent<PlanetInfo>();
            string messageTMP;
      
            messageTMP = GetMessagByKey("PlanetInfo0");
            planetInfoMessage += " " + messageTMP + " " + GetMessagByKey(planetInfo.PlanetInformationOf.ToString()) + "\n\n";

            //combined message
            CreateNasaDataMessage();
            createdPlanetInfoMessage = planetInfoMessage + createdNasaDataMessage;
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
            createdNasaDataMessage = "";
            planetInfoMessageText.text = GetMessagByKey(createdPlanetInfoMessage);

            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.identity;
            PlanetaryController.Instance.ResetSortingGame();
            if (sunLightHalo != null)
                sunLightHalo.range = 0.25f;

            PlanetaryController.Instance.sortedPlanetCount = 0;
            isSnapped = false;
        }
        #endregion Reset
    }
}
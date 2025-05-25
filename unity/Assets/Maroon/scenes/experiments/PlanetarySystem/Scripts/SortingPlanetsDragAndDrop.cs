using UnityEngine;
using GEAR.Localization;    //MLG
using TMPro;
using Valve.VR.InteractionSystem;

namespace Maroon.Experiments.PlanetarySystem
{
    public class SortingPlanetsDragAndDrop : MonoBehaviour, IResetObject
    {
        public PlanetSortingGameController planetSortingGameController;
        public TextMeshProUGUI whiteboardText;
        public AudioSource audioSource;
        public AudioClip pickUpClip;
        public AudioClip dropClip;

        public PlanetRotation planetRotation;
        public Transform sortingPlanetTarget;
        public WhiteboardDisplay whiteboardDisplay;
        
        public float snapDistance;
        private bool isSnapped = false;
        private readonly float scaleFactor = 1.02f;
        public Light sunLightHalo;
        Vector3 mousePosition;
        public TextMeshProUGUI planetInfoMessageText;
        string createdPlanetInfoMessage;
        //---------------------------------------------------------------------------------------
        //handle mouse input
        #region MouseInput
        /// <summary>
        /// get mouse position
        /// </summary>
        private void OnMouseDown()
        {
            audioSource.PlayOneShot(pickUpClip);
            mousePosition = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            // Show PlanetInfo data when planet is clicked on

            PlanetInfo planetInfo = GetComponent<PlanetInfo>();
            createdPlanetInfoMessage = planetInfo.CreateUnnamedPlanetInfoMessage();
            planetInfoMessageText.text = createdPlanetInfoMessage;
        }


        /// <summary>
        /// calculate position
        /// </summary>
        private void OnMouseDrag()
        {
            if (!isSnapped)
            {
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
                transform.position = newPosition;
            }
        }


        /// <summary>
        /// snap to target when distance is smaller han snap distance
        /// or snap back to start slot
        /// </summary>
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
                    PlanetInfo planetInfo = GetComponent<PlanetInfo>();
                    planetInfo.IsSnapped = false;
                }
            }
        }
        #endregion MouseInput
        #region VRInput
        private Interactable _interactable;
        private Throwable _throwable;

        private void Awake()
        {
            _interactable = GetComponent<Interactable>();
            _throwable = GetComponent<Throwable>();
        }

        public void OnAttachedToHand(Hand hand)
        {
            audioSource.PlayOneShot(pickUpClip);

            PlanetInfo planetInfo = GetComponent<PlanetInfo>();
            createdPlanetInfoMessage = planetInfo.CreateUnnamedPlanetInfoMessage();
            whiteboardText.text = createdPlanetInfoMessage;
        }

        public void OnDetachedFromHand(Hand hand)
        {
            if (!isSnapped)
            {
                float distance = Vector3.Distance(
                    transform.position,
                    sortingPlanetTarget.position);

                if (distance <= snapDistance)
                {
                    SnapToTarget();
                }
                else
                {
                if (transform.parent != null)
                {
                    transform.position = transform.parent.position;
                }
                    isSnapped = false;
                    PlanetInfo planetInfo = GetComponent<PlanetInfo>();
                    if (planetInfo != null)
                    {
                        planetInfo.IsSnapped = false;
                    }
                }
            }
        }
        #endregion VRInput

        //snap the planets to target
        #region SnapPlanet
        /// <summary>
        /// snaps the planets to target by setting its parent and scales them
        /// </summary>
        private void SnapToTarget()
        {
            transform.SetParent(sortingPlanetTarget);
            transform.position = sortingPlanetTarget.position;
            transform.localScale = new Vector3(1, 1, 1) * scaleFactor;
            if (sunLightHalo != null)
                sunLightHalo.range = 1.75f;

            planetRotation.SetObliquityToOrbit();

            isSnapped = true;
            PlanetInfo planetInfo = GetComponent<PlanetInfo>();
            planetInfo.IsSnapped = true;
            PlanetaryController.Instance.IncrementSnappedPlanetCount();
            audioSource.PlayOneShot(dropClip);

            createdPlanetInfoMessage = planetInfo.CreatePlanetInfoMessage();
            planetInfoMessageText.text = createdPlanetInfoMessage;
            whiteboardText.text = createdPlanetInfoMessage;
        }
        #endregion SnapPlanet


        // reset ResetSortingGame
        #region Reset
        /// <summary>
        /// ResetPlanetInfoMessage
        /// </summary>
        public void ResetPlanetInfoMessage()
        {
            createdPlanetInfoMessage = "PlanetDescription";
            planetInfoMessageText.text = LanguageManager.Instance.GetString(createdPlanetInfoMessage);
            whiteboardText.text = LanguageManager.Instance.GetString(createdPlanetInfoMessage);
        }
    


        /// <summary>
        /// reset sortingPlanet parent, scale, position, sortedPlanetCount
        /// </summary>
        public void ResetObject()
        {
            ResetPlanetInfoMessage();

            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.identity;
            planetSortingGameController.ResetSortingGame();
            if (sunLightHalo != null)
                sunLightHalo.range = 0.25f;

            PlanetaryController.Instance.sortedPlanetCount = 0;
            isSnapped = false;
            PlanetInfo planetInfo = GetComponent<PlanetInfo>();
            planetInfo.IsSnapped = false;
        }
        #endregion Reset
    }
}
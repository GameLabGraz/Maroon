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
        private GameLabGraz.VRInteraction.VRInteractable _vri;
        private GameLabGraz.VRInteraction.VRThrowable    _vrth;
        private Rigidbody _rb;

        private void Awake()
        {
            _vri  = GetComponent<GameLabGraz.VRInteraction.VRInteractable>();
            _vrth = GetComponent<GameLabGraz.VRInteraction.VRThrowable>();
            _rb   = GetComponent<Rigidbody>();
        }


        public void OnAttachedToHand(Hand hand)
        {
            if (isSnapped) { hand.DetachObject(gameObject); return; }
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
            PlanetaryControllerVR.Instance.IncrementSnappedPlanetCount();
            audioSource.PlayOneShot(dropClip);
            if (_vri)  { _vri.interactable = false; _vri.hoverable = false; _vri.highlightOnHover = false; }
            if (_vrth) _vrth.enabled = false;
            if (_rb)
            {
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                _rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            createdPlanetInfoMessage = planetInfo.CreatePlanetInfoMessage();
            planetInfoMessageText.text = createdPlanetInfoMessage;
            if (whiteboardText != null)
            {
                whiteboardText.text = createdPlanetInfoMessage;
            }
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

            PlanetaryControllerVR.Instance.sortedPlanetCount = 0;
            isSnapped = false;
            StartCoroutine(ReenableNextFrame());
            PlanetInfo planetInfo = GetComponent<PlanetInfo>();
            planetInfo.IsSnapped = false;
        }
        
        private System.Collections.IEnumerator ReenableNextFrame()
        {
            yield return null;

            if (_vri)  { _vri.interactable = true; _vri.hoverable = true; _vri.highlightOnHover = true; }
            if (_vrth) _vrth.enabled = true;

            if (_rb)   _rb.constraints = RigidbodyConstraints.None;
        }
        #endregion Reset
    }
}
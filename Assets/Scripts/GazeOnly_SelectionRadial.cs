using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;


    // This class is used to control a radial bar that fills
    // up as the user holds down the Fire1 button.  When it has
    // finished filling it triggers an event.  It also has a
    // coroutine which returns once the bar is filled.
    public class GazeOnly_SelectionRadial : MonoBehaviour
    {
        public event Action OnSelectionComplete;                                                // This event is triggered when the bar has filled.

    public static bool gazeOnlyControl { get; protected set; }

    [SerializeField] private float m_SelectionDuration = 2.0f;                                // How long it takes for the bar to fill.
        [SerializeField] private bool m_HideOnStart = true;                                     // Whether or not the bar should be visible at the start.
        [SerializeField] private Image m_Selection;                                             // Reference to the image who's fill amount is adjusted to display the bar.
        [SerializeField] protected VRStandardAssets.Utils.VRInteractiveItem m_VRInteractiveItem;  // Reference to the VRInput so that input events can be subscribed to.


        private Coroutine m_SelectionFillRoutine;                                               // Used to start and stop the filling coroutine based on input.
    private Coroutine m_WaitForSelectionToFillRoutine;
    private bool m_IsSelectionRadialActive;                                                    // Whether or not the bar is currently useable.
        private bool m_RadialFilled;                                                               // Used to allow the coroutine to wait for the bar to fill.

        public float SelectionDuration { get { return m_SelectionDuration; } }

    private bool m_ActionsSubscribedFlag = false;


    public virtual void OnEnable()
    {
        GazeOnlyToggleButton.VR_ControlToggled += EnableDisableGazeControl;
        EnableDisableGazeControl();

    }
    private void EnableDisableGazeControl()
    {
        if (gazeOnlyControl && m_ActionsSubscribedFlag == false)
        {
            m_ActionsSubscribedFlag = true;
            m_VRInteractiveItem.OnOver += HandleDown;
            m_VRInteractiveItem.OnOut += HandleUp;
        }
        else if(gazeOnlyControl == false && m_ActionsSubscribedFlag)
        {
            m_ActionsSubscribedFlag = false;
            m_VRInteractiveItem.OnOver -= HandleDown;
            m_VRInteractiveItem.OnOut -= HandleUp;
        }
    }

    public virtual void OnDisable()
    {
        GazeOnlyToggleButton.VR_ControlToggled -= EnableDisableGazeControl;
        EnableDisableGazeControl();
    }


        private void Start()
        {
        if (m_Selection == null)
        {
            GameObject temp = GameObject.Find("/Camera/VRCameraExtension/VRCameraUI/GUIReticle/UISelectionBar"); //Hardcoded for Workstation- Scene. A little bit dirty i know. Maybe make Selection bar static or singleton in future.
            if (temp != null)
                m_Selection = temp.GetComponent<Image>();
        }
            // Setup the radial to have no fill at the start and hide if necessary.
            m_Selection.fillAmount = 0f;

            if (m_HideOnStart)
                Hide();
        }


        public void Show()
        {
            m_Selection.gameObject.SetActive(true);
            m_IsSelectionRadialActive = true;
        }


        public void Hide()
        {
            m_Selection.gameObject.SetActive(false);
            m_IsSelectionRadialActive = false;

            // This effectively resets the radial for when it's shown again.
            m_Selection.fillAmount = 0f;
        }


        private IEnumerator FillSelectionRadial()
        {
            // At the start of the coroutine, the bar is not filled.
            m_RadialFilled = false;

            // Create a timer and reset the fill amount.
            float timer = 0f;
            m_Selection.fillAmount = 0f;

            // This loop is executed once per frame until the timer exceeds the duration.
            while (timer < m_SelectionDuration)
            {
                // The image's fill amount requires a value from 0 to 1 so we normalise the time.
                m_Selection.fillAmount = timer / m_SelectionDuration;

                // Increase the timer by the time between frames and wait for the next frame.
                timer += Time.deltaTime;
                yield return null;
            }

            // When the loop is finished set the fill amount to be full.
            m_Selection.fillAmount = 1f;

            // Turn off the radial so it can only be used once.
            m_IsSelectionRadialActive = false;

            // The radial is now filled so the coroutine waiting for it can continue.
            m_RadialFilled = true;

            // If there is anything subscribed to OnSelectionComplete call it.
            if (OnSelectionComplete != null)
                OnSelectionComplete();
        }


        public IEnumerator WaitForSelectionRadialToFill()
        {
          m_RadialFilled = false;
            // Set the radial to not filled in order to wait for it.
  
            // Make sure the radial is visible and usable.
            Show();
        m_SelectionFillRoutine = StartCoroutine(FillSelectionRadial());

        // Check every frame if the radial is filled.
        while (!m_RadialFilled)
            {
            Show();
            yield return null;
            }

            // Once it's been used make the radial invisible.
            Hide();
        }


        protected void HandleDown()
        {
            // If the radial is active start filling it.
            m_WaitForSelectionToFillRoutine = StartCoroutine(WaitForSelectionRadialToFill());

    }


        protected void HandleUp()
        {
            // If the radial is active stop filling it and reset it's amount.

                if (m_SelectionFillRoutine != null)
                    StopCoroutine(m_SelectionFillRoutine);
        if (m_WaitForSelectionToFillRoutine != null)
            StopCoroutine(m_WaitForSelectionToFillRoutine);


            m_Selection.fillAmount = 0f;
            Hide();
        }
    }
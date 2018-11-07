using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using System;
[DisallowMultipleComponent]
[AddComponentMenu("MobileVR/physicalSliderController")]
public class PhysicalSliderController : MonoBehaviour, IResetObject
{
    [Header("Method called when slidervalue changed:")]
    [SerializeField]
    private GameObject invokeObject;

    [SerializeField]
    private string methodName;

    [Space(5)]
    [Header("Physical representative:")]
    [SerializeField]
    private Text ValueText;

   // [SerializeField]
    //private bool isInteger = false;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Transform minimumLimit;

    [SerializeField]
    private Transform maximumLimit;


    [Space(5)]
    [Header("Gaze- Only Control:")]
    [Tooltip("At 1, the slider reacts immediately, at 0.01 the reaction happens very slowly.")]
    [Range(0.01f, 1.0f)]
    [SerializeField]
    private float m_sliderReactSpeed = 0.3f;
    [SerializeField] private Transform m_Camera;            // The transform of the camera.

    [SerializeField] private GazeOnly_SelectionRadial m_selectionSlider;
    private bool m_selectionSliderTriggered = false;


    //private VRTK_Control_UnityEvents controlEvents;

    private Vector3 startPos;

    private Quaternion startRot;

    private const bool m_setPhysicalSliderWhenValueChangedAutomatically = true;



    private float m_sliderGoalvalue = 0.0f;
    private float m_cachedSliderValue = 0.0f;

    private void Start()
    {
        m_sliderGoalvalue = slider.value;
        m_cachedSliderValue = m_sliderGoalvalue;
        //minimumLimit = minimumLimit ? minimumLimit : this.transform;
        //maximumLimit = maximumLimit ? maximumLimit : this.transform;

        // startPos = this.transform.position;
        // startRot = this.transform.rotation;
        /*
    controlEvents = GetComponent<VRTK_Control_UnityEvents>();
    if (controlEvents == null)
    {
        controlEvents = gameObject.AddComponent<VRTK_Control_UnityEvents>();
    }*/

        if (ValueText != null)
        {
            if (slider.wholeNumbers)
                ValueText.text = ((int)slider.value).ToString();
            else
            ValueText.text = slider.value.ToString("0.00");
        }
        slider.onValueChanged.AddListener(delegate { HandleChange(); });
        HandleChange();
        StartCoroutine(LateStart(2));

    }
    private IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        HandleChange();
    }
    /*
    protected override ControlValueRange RegisterValueRange()
    {
        if (options.Count > 0)
        {
            minimumValue = 0;
            maximumValue = options.Count - 1;
        }

        return new ControlValueRange()
        {
            controlMin = minimumValue,
            controlMax = maximumValue
        };
    }
    */
    private void setPhysicalSlider() {
        Debug.Log("[Position]"+transform.position);
        float normValue = (slider.value - slider.minValue) / (slider.maxValue - slider.minValue); //normalize slider to values between 0 and 1

        //Todo: Put the next two lines in
        // Set the target position  to be an interpolation of itself and the UI's position.
        //targetPosition = Vector3.Lerp(m_UIElement.position, targetPosition, m_FollowSpeed * Time.deltaTime);

        transform.position = minimumLimit.position * (1.0f - normValue) + maximumLimit.position * normValue;
        Debug.Log("[Position]" + transform.position);
    }
    private void HandleChange()
    {
        if (ValueText != null)
        {
            if (slider.wholeNumbers)
                ValueText.text = ((int)slider.value).ToString();
            else
                ValueText.text = slider.value.ToString("0.00");
        }

        if (invokeObject != null)
        {
            if (slider.wholeNumbers)
                invokeObject.SendMessage(methodName, (int)slider.value);
            else
                invokeObject.SendMessage(methodName, slider.value);
        }
        if (m_setPhysicalSliderWhenValueChangedAutomatically) setPhysicalSlider();
        if (!GazeOnly_SelectionRadial.gazeOnlyControl)
            m_sliderGoalvalue = slider.value;
    }

    public void resetObject()
    {
       // this.transform.position = startPos;
       // this.transform.rotation = startRot;
        StartCoroutine(LateStart(1));
    }



    
    //Here Gaze-only- Control Part is starting:-------------------------------------------------------------------------
    private void Update()
    {
        if (!GazeOnly_SelectionRadial.gazeOnlyControl ||
            (slider.value - m_sliderGoalvalue < 0.01f && slider.value - m_sliderGoalvalue > -0.01f)) return;
        if (slider.wholeNumbers)
        {
            if ((m_sliderGoalvalue - slider.value) >= 1.0f)
                slider.value += 1.0f;
            else if ((m_sliderGoalvalue - slider.value) <= -1.0f)
                slider.value -= 1.0f;
        }
        else
            slider.value += (m_sliderGoalvalue - slider.value) * m_sliderReactSpeed;
    }

    /// <summary>
    /// ////////////////////////////////////////////////77
    /// </summary>
    /// <param name="delta"></param>
    /// 
    private void moveSlider(float delta)
    {
        float slideroffset = delta * (slider.maxValue - slider.minValue);
        //if (!slider.wholeNumbers) slideroffset *= 0.25f; //For a softer Sliderreaction to the User's Gaze
    Debug.Log("[GearVR_Slider] Slider value: " + delta);
        if (slider.wholeNumbers)
        {
            //m_setPhysicalSliderWhenValueChangedAutomatically = false;
            m_sliderGoalvalue += slideroffset;
            //this.transform.position = testSliderPosition(delta * softGazeSlider); //This line is here to prevent choppy slider behaviour if only whole numbers are allowed
        }
        else
            m_sliderGoalvalue += slideroffset;
    }


    private Vector3 testSliderPosition(ref float offset)
    {
        float normValue;
        if (slider.wholeNumbers)
        {
            
            int offset_int = (int)(offset * (slider.maxValue - slider.minValue));
            offset_int = offset > 0.0f ? (int)offset + 1 : (int)offset - 1;
            normValue = ((m_sliderGoalvalue + (float)offset_int) - slider.minValue) / (slider.maxValue - slider.minValue);
            offset = ((float)offset_int / (slider.maxValue - slider.minValue));
        }
        else
        {
            normValue = ((m_sliderGoalvalue - slider.minValue) / (slider.maxValue - slider.minValue)) + offset; //normalize slider to values between 0 and 1
        }
        return minimumLimit.position * (1.0f - normValue) + maximumLimit.position * normValue;
        //Todo: Put the next two lines in
        // Set the target position  to be an interpolation of itself and the UI's position.
        //targetPosition = Vector3.Lerp(m_UIElement.position, targetPosition, m_FollowSpeed * Time.deltaTime);

    }

    private Vector3 testSliderPosition()
    {
        float normValue;
        if (slider.wholeNumbers)
        {
            normValue = (m_sliderGoalvalue - slider.minValue) / (slider.maxValue - slider.minValue);
        }
        else
        {
            normValue = ((m_sliderGoalvalue - slider.minValue) / (slider.maxValue - slider.minValue)); //normalize slider to values between 0 and 1
        }
        return minimumLimit.position * (1.0f - normValue) + maximumLimit.position * normValue;
        //Todo: Put the next two lines in
        // Set the target position  to be an interpolation of itself and the UI's position.
        //targetPosition = Vector3.Lerp(m_UIElement.position, targetPosition, m_FollowSpeed * Time.deltaTime);

    }


    public void gazeSwipe(VRStandardAssets.Utils.VRInput.SwipeDirection x)
    {
        // Find the direction the camera is looking but on a flat plane.
        // Vector3 targetDirection = Vector3.ProjectOnPlane(m_Camera.forward, Vector3.up).normalized;
        /* Vector3 targetDirection = m_Camera.forward;
         Vector3 targetDir = invokeObject.transform.position - m_Camera.position;
         float angle = Math.Abs(Vector3.Angle(targetDir, targetDirection));*/
        /*
    Vector3 targetDir = invokeObject.transform.position - m_Camera.transform.position;
    Vector3 forward = m_Camera.transform.forward;
    float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);
    if (angle < -0.5F)
    moveSlider(-0.1f);
    else if (angle > 0.5F)
    moveSlider(0.1f);
    else
    return;*/
       moveSlider(getSliderOffset(0.0f, Vector3.Angle(testSliderPosition() - m_Camera.transform.position, m_Camera.transform.forward)));
    }
    private float getSliderOffset(float currentoffset, float currentangle)
    {
        if (currentangle < 0.1f) return currentoffset;
        float newoffset = currentoffset - 0.01f * currentangle;

        float newangle = Vector3.Angle(testSliderPosition(ref newoffset) - m_Camera.transform.position, m_Camera.transform.forward);
        if (newangle < currentangle)
            return getSliderOffset(newoffset, newangle);
        newoffset = currentoffset + 0.01f * currentangle;
        newangle = Vector3.Angle(testSliderPosition(ref newoffset) - m_Camera.transform.position, m_Camera.transform.forward);
        if (newangle < currentangle)
            return getSliderOffset(newoffset, newangle);

        return currentoffset;
    }

    public void selectionSliderAction()
    {
        m_selectionSliderTriggered = true;
    }

    public void GazeHit()
    {
        m_cachedSliderValue = m_sliderGoalvalue;
        m_selectionSliderTriggered = false;
        if(m_selectionSlider != null) m_selectionSlider.OnSelectionComplete += selectionSliderAction;
    }

    public void GazeLeave()
    {
        if (m_selectionSlider != null) m_selectionSlider.OnSelectionComplete -= selectionSliderAction;
        if (m_selectionSliderTriggered)
            m_cachedSliderValue = m_sliderGoalvalue;
        else
          m_sliderGoalvalue = m_cachedSliderValue; //Gaze didn't pointed long enouth on Game Object --> Slider action is reversed



        m_selectionSliderTriggered = false;
    }


}


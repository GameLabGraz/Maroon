using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;
using UnityEngine.UI;

[RequireComponent(typeof(VRInteractiveItem))]
public class GearVR_Slider : VRMenuItem
{
    private enum ValidSwipeDirection { xAxis, yAxis, both };

    [SerializeField]
    private bool m_swiping = false;

    [SerializeField]
    private PhysicalSliderController m_physicalSliderController;

   [SerializeField]
    private float m_swipeSensivity = 0.01f;


    [SerializeField] private ValidSwipeDirection m_Swipedirection = ValidSwipeDirection.xAxis;


    private float m_wohleNumberSliderAccumulation = 0.0f;

    public VRInput vrInput;
    public Slider positionSlider;
    //public MoveLeftRight moveLeftRight;
    private Vector2 m_MousePosition = new Vector2(0.0f, 0.0f);

    protected override void Start()
    {
#if !UNITY_EDITOR
        interactiveItem.OnOver += GazeHit;
        interactiveItem.OnOut += GazeUnHit;
#endif

        if (GazeOnly_SelectionRadial.gazeOnlyControl && m_physicalSliderController == null)
            Debug.LogError("[GearVR_Slider.cs] PhysicalSliderController must be assigned for GazeOnly- Control!");


        if (vrInput == null)
            Debug.LogError("[GearVR_Slider.cs] VrInput- Script not assigned");

        if (positionSlider == null)
        {
            Debug.LogWarning("[GearVR_Slider.cs] Position Slider not assigned");
        }
        base.Start();
    }
    private void OnDestroy()
    {
#if !UNITY_EDITOR
        if (interactiveItem.IsOver)
            vrInput.OnSwipe -= onSwiping;
            interactiveItem.OnOver -= GazeHit;
        interactiveItem.OnOut -= GazeUnHit;
#endif
    }

    void moveSlider(float delta)
    {
        Debug.Log("[GearVR_Slider] Slider value: "+delta);
        if (positionSlider.wholeNumbers)
        {
            m_wohleNumberSliderAccumulation += delta;
            if(!m_swiping) delta = m_wohleNumberSliderAccumulation;
            if (m_swiping)
            {
                positionSlider.value += delta > 0.0f ? 1.0f : -1.0f;
            }
            else
            {
                m_wohleNumberSliderAccumulation += delta;
                if (m_wohleNumberSliderAccumulation < 1.0f && m_wohleNumberSliderAccumulation > -1.0f) return;
                    positionSlider.value += m_wohleNumberSliderAccumulation > 1.0f ? 1.0f : -1.0f;
                    m_wohleNumberSliderAccumulation = 0;
            }
        }
        else
            positionSlider.value += delta;
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!interactiveItem.IsOver) return;
        if (!Input.GetMouseButtonDown(0))
        {
            moveSlider(Input.GetAxis("Mouse ScrollWheel") * 0.4f);
        }
    }
#endif

//#if !UNITY_EDITOR
    void onSwiping(VRInput.SwipeDirection x)
    {

            switch (x)
            {
                case VRInput.SwipeDirection.UP:
                    moveSlider(0.1f);
                    return;
                case VRInput.SwipeDirection.DOWN:
                    moveSlider(-0.1f);
                    return;
                case VRInput.SwipeDirection.LEFT:
                    moveSlider(0.1f);
                    return;
                case VRInput.SwipeDirection.RIGHT:
                    moveSlider(-0.1f);
                    return;
                case VRInput.SwipeDirection.NONE:
                    return;
                default:
                    return;
            }

    }

    void whileSwiping(VRInput.SwipeDirection x)
    {
        if (!interactiveItem.IsOver) return;

            switch (m_Swipedirection) {
            case ValidSwipeDirection.xAxis:
                moveSlider((Input.mousePosition.x - m_MousePosition.x)* m_swipeSensivity);
                m_MousePosition.x = Input.mousePosition.x;
                break;
                case ValidSwipeDirection.yAxis:
                moveSlider((Input.mousePosition.y - m_MousePosition.y)* m_swipeSensivity);
                m_MousePosition.y = Input.mousePosition.y;
                break;
                case ValidSwipeDirection.both:
                moveSlider((Input.mousePosition.x - m_MousePosition.x + Input.mousePosition.y - m_MousePosition.y)* m_swipeSensivity);
                m_MousePosition.x = Input.mousePosition.x;
                m_MousePosition.y = Input.mousePosition.y;
                break;
        };
    }

    private void GazeHit()
    {
        if (GazeOnly_SelectionRadial.gazeOnlyControl)
        {
            m_physicalSliderController.GazeHit();
            vrInput.OnSwipe += m_physicalSliderController.gazeSwipe;
            return;
        }
    if (m_swiping)
    {
        vrInput.OnSwipe += onSwiping;
    }
    else
    {
                m_MousePosition.x = Input.mousePosition.x;
                m_MousePosition.y = Input.mousePosition.y;
        interactiveItem.OnDown += PointerDown;
    }
}
    private void GazeUnHit()
    {
        if (GazeOnly_SelectionRadial.gazeOnlyControl)
        {
            vrInput.OnSwipe -= m_physicalSliderController.gazeSwipe;
            m_physicalSliderController.GazeLeave();
            return;
        }
        if (m_swiping)
        {
            vrInput.OnSwipe -= onSwiping;
        }
        else
        {
                interactiveItem.OnDown -= PointerDown;
            }
    }
    private void PointerDown()
    {
        m_MousePosition.x = Input.mousePosition.x;
        m_MousePosition.y = Input.mousePosition.y;
        vrInput.OnSwipe += whileSwiping;
        interactiveItem.OnUp += PointerUp;
    }

    private void PointerUp()
    {
        interactiveItem.OnUp -= PointerUp;

        vrInput.OnSwipe -= whileSwiping;

            whileSwiping(VRInput.SwipeDirection.NONE);
    }
//#endif
}
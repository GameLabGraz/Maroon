using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetController : MonoBehaviour
{
    public GameObject sun;
    public GameObject mercury;
    public GameObject venus;
    public GameObject earth;
    public GameObject mars;
    public GameObject jupiter;
    public GameObject saturn;
        public GameObject saturn_ring_1;
        public GameObject saturn_ring_2;
    public GameObject uranus;
        public ParticleSystem uranusParticleSystem;
    public GameObject neptune;
    public GameObject moon;


    public TrajectoryDrawer trajectoryDrawer;
    private static PlanetController _instance;


    public static PlanetController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PlanetController>();
            return _instance;
        }
    }


    /*
     * toggle planets visibilty after the UI radiobButton is pressed
     */
    #region toggle planets;
    public void UIToggle0(bool isOn)
    {
        Debug.Log("current checkbox state: sun: " + !isOn);
        sun.GetComponent<Renderer>().enabled = !isOn;
        trajectoryDrawer.ToggleTrajectory(0, !isOn);
    }

    public void UIToggle1(bool isOn)
    {
        Debug.Log("current checkbox state: mercury: " + !isOn);
        mercury.GetComponent<Renderer>().enabled = !isOn;
        trajectoryDrawer.ToggleTrajectory(1, !isOn);
    }

    public void UIToggle2(bool isOn)
    {
        Debug.Log("current checkbox state: venus: " + !isOn);
        venus.GetComponent<Renderer>().enabled = !isOn;
        trajectoryDrawer.ToggleTrajectory(2, !isOn);
    }

    public void UIToggle3(bool isOn)
    {
        Debug.Log("current checkbox state: earth: " + !isOn);
        earth.GetComponent<Renderer>().enabled = !isOn;
        trajectoryDrawer.ToggleTrajectory(3, !isOn);
    }

    public void UIToggle4(bool isOn)
    {
        Debug.Log("current checkbox state: mars: " + !isOn);
        mars.GetComponent<Renderer>().enabled = !isOn;
        trajectoryDrawer.ToggleTrajectory(4, !isOn);
    }

    public void UIToggle5(bool isOn)
    {
        Debug.Log("current checkbox state: jupiter: " + !isOn);
        jupiter.GetComponent<Renderer>().enabled = !isOn;
        trajectoryDrawer.ToggleTrajectory(5, !isOn);
    }

    public void UIToggle6(bool isOn)
    {
        Debug.Log("current checkbox state: saturn: " + !isOn);
        saturn.GetComponent<Renderer>().enabled = !saturn.GetComponent<Renderer>().enabled;
        saturn_ring_1.GetComponent<Renderer>().enabled = !saturn_ring_1.GetComponent<Renderer>().enabled;
        saturn_ring_2.GetComponent<Renderer>().enabled = !saturn_ring_2.GetComponent<Renderer>().enabled;
        trajectoryDrawer.ToggleTrajectory(6, !isOn);
    }

    public void UIToggle7(bool isOn)
    {
        Debug.Log("current checkbox state: uranus: " + !isOn);
        uranus.GetComponent<Renderer>().enabled = !isOn;
        uranusParticleSystem.gameObject.GetComponent<Renderer>().enabled = !isOn;
        trajectoryDrawer.ToggleTrajectory(7, !isOn);
    }
    public void UIToggle8(bool isOn)
    {
        Debug.Log("current checkbox state: neptune: " + !isOn);
        neptune.GetComponent<Renderer>().enabled = !isOn;
        trajectoryDrawer.ToggleTrajectory(8, !isOn);
    }

    public void UIToggle9(bool isOn)
    {
        Debug.Log("current checkbox state: moon: " + !isOn);
        moon.GetComponent<Renderer>().enabled = !isOn;
        trajectoryDrawer.ToggleTrajectory(9, !isOn);
    }
    #endregion toggle planets;
}

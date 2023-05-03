using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public GameObject sun;
    public GameObject mercury;
    public GameObject venus;
    public GameObject earth;
    public GameObject moon;
    public GameObject mars;
    public GameObject jupiter;
    public GameObject saturn;
        public GameObject saturn_ring_1;
        public GameObject saturn_ring_2;
    public GameObject uranus;
        public ParticleSystem uranusParticleSystem;
    public GameObject neptune;

    private static GameHandler _instance;


    public static GameHandler Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameHandler>();
            return _instance;
        }
    }

    /*
     * toggle planets visibilty after the UI radiobButton is pressed
     */
    #region toggle planets;
    public void UIToggle0(bool checkmark)
    {
        Debug.Log("current checkbox state: sun: " + !checkmark);
        sun.GetComponent<Renderer>().enabled = !checkmark;
    }

    public void UIToggle1(bool checkmark)
    {
        Debug.Log("current checkbox state: mercury: " + !checkmark);
        mercury.GetComponent<Renderer>().enabled = !checkmark;
    }

    public void UIToggle2(bool checkmark)
    {
        Debug.Log("current checkbox state: venus: " + !checkmark);
        venus.GetComponent<Renderer>().enabled = !checkmark;
    }

    public void UIToggle3(bool checkmark)
    {
        Debug.Log("current checkbox state: earth: " + !checkmark);
        earth.GetComponent<Renderer>().enabled = !checkmark;
    }

    public void UIToggle31(bool checkmark)
    {
        Debug.Log("current checkbox state: moon: " + !checkmark);
        moon.GetComponent<Renderer>().enabled = !checkmark;
    }

    public void UIToggle4(bool checkmark)
    {
        Debug.Log("current checkbox state: mars: " + !checkmark);
        mars.GetComponent<Renderer>().enabled = !checkmark;
    }

    public void UIToggle5(bool checkmark)
    {
        Debug.Log("current checkbox state: jupiter: " + !checkmark);
        jupiter.GetComponent<Renderer>().enabled = !checkmark;
    }

    public void UIToggle6(bool checkmark)
    {
        Debug.Log("current checkbox state: saturn: " + !checkmark);
        saturn.GetComponent<Renderer>().enabled = !saturn.GetComponent<Renderer>().enabled;
        saturn_ring_1.GetComponent<Renderer>().enabled = !saturn_ring_1.GetComponent<Renderer>().enabled;
        saturn_ring_2.GetComponent<Renderer>().enabled = !saturn_ring_2.GetComponent<Renderer>().enabled;
    }

    public void UIToggle7(bool checkmark)
    {
        Debug.Log("current checkbox state: uranus: " + !checkmark);
        uranus.GetComponent<Renderer>().enabled = !checkmark;
        uranusParticleSystem.gameObject.GetComponent<Renderer>().enabled = !checkmark;
    }
    public void UIToggle8(bool checkmark)
    {
        Debug.Log("current checkbox state: neptune: " + !checkmark);
        neptune.GetComponent<Renderer>().enabled = !checkmark;
    }
    #endregion toggle planets;
}

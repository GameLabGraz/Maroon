using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


    }


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

/*    public void UIToggle0(bool checkmark)
    {
        Debug.Log("current checkbox state: sun: " + !checkmark);
        sun.SetActive(!checkmark);
        Debug.Log("current planet visible: sun " + sun.activeSelf);
    }

    public void UIToggle1(bool checkmark)
    {
        Debug.Log("current checkbox state: mercury: " + !checkmark);
        mercury.SetActive(!checkmark);
        Debug.Log("current planet visible: mercury " + mercury.activeSelf);
    }

    public void UIToggle2(bool checkmark)
    {
        Debug.Log("current checkbox state: venus: " + !checkmark);
        venus.SetActive(!checkmark);
        Debug.Log("current planet visible: venus " + venus.activeSelf);
    }

    public void UIToggle3(bool checkmark)
    {
        Debug.Log("current checkbox state: earth: " + !checkmark);
        earth.SetActive(!checkmark);
        Debug.Log("current planet visible: earth " + earth.activeSelf);
    }

    public void UIToggle31(bool checkmark)
    {
        Debug.Log("current checkbox state: moon: " + !checkmark);
        moon.SetActive(!checkmark);
        Debug.Log("current planet visible: moon " + moon.activeSelf);
    }

    public void UIToggle4(bool checkmark)
    {
        Debug.Log("current checkbox state: mars: " + !checkmark);
        mars.SetActive(!checkmark);
        Debug.Log("current planet visible: mars " + mars.activeSelf);
    }

    public void UIToggle5(bool checkmark)
    {
        Debug.Log("current checkbox state: jupiter: " + !checkmark);
        jupiter.SetActive(!checkmark);
        Debug.Log("current planet visible: jupiter " + jupiter.activeSelf);
    }

    public void UIToggle6(bool checkmark)
    {
        Debug.Log("current checkbox state: saturn: " + !checkmark);
        saturn.SetActive(!checkmark);
        Debug.Log("current planet visible: saturn " + saturn.activeSelf);
    }

    public void UIToggle7(bool checkmark)
    {
        Debug.Log("current checkbox state: uranus: " + !checkmark);
        uranus.SetActive(!checkmark);
        Debug.Log("current planet visible: uranus " + uranus.activeSelf);
    }
    public void UIToggle8(bool checkmark)
    {
        Debug.Log("current checkbox state: neptune: " + !checkmark);
        neptune.SetActive(!checkmark);
        Debug.Log("current planet visible: neptune " + neptune.activeSelf);
    }
*/

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSunlightEffects : MonoBehaviour
{
    private Light sunLight;
    public ParticleSystem sunParticleSystem;


    /*
     * 
     */
    void Start()
    {
        sunLight = GetComponent<Light>();

    }


    /*
     * 
     */
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            sunLight.enabled = !sunLight.enabled;
            Debug.Log("sunLight toggeled: " + sunLight.enabled);

            if (sunParticleSystem.gameObject.activeSelf == false)
            {
                sunParticleSystem.gameObject.SetActive(true);
                Debug.Log("sunParticleSystem Active Self: " + sunParticleSystem.gameObject.activeSelf);
            }
            else
            {
                sunParticleSystem.gameObject.SetActive(false);
                Debug.Log("sunParticleSystem Active Self: " + sunParticleSystem.gameObject.activeSelf);
            }
        }

    }
}

//
//Author: Marcel Lohfeyer
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSunlightEffects : MonoBehaviour
{
    private Light sunLight;
    public ParticleSystem sunParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        sunLight = GetComponent<Light>();

    }

    // Update is called once per frame
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerVR;

    [SerializeField]
    private GameObject PlayerSimulator;

    [SerializeField]
    private bool preferSimulator = false;

    private void Awake()
    {
        if(!preferSimulator && SteamVR.active)
        {
            PlayerVR.SetActive(true);
            PlayerSimulator.SetActive(false);
        }
        else
        {
            PlayerVR.SetActive(false);
            PlayerSimulator.SetActive(true);
        }
    }
}

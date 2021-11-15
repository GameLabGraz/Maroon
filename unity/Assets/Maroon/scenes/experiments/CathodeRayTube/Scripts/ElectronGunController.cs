using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronGunController : MonoBehaviour
{
    public GameObject Electron;
    public float ElectronSpeed;
    private GameObject TempElectron = null;

    // Start is called before the first frame update

    private void Update()
    {
        if (TempElectron == null)
        {
            TempElectron = Instantiate(Electron, transform.position, transform.rotation);
            TempElectron.transform.name = "Electron";
            TempElectron.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, -ElectronSpeed,0));
        }
    }
}

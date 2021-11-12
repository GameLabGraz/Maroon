using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronGunController : MonoBehaviour
{
    public GameObject Electron;
    public float ElectronSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        var electronTransform = transform;
        GameObject electron = Instantiate(Electron, electronTransform.position, electronTransform.rotation);
        electron.transform.name = "Electron";
        electron.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, -ElectronSpeed,0));
    }
}

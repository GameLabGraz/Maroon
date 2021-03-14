using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Laser : MonoBehaviour
{

    private GameObject handle1;
    private GameObject handle2;
    private GameObject beam;
    private Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {

        handle1 = transform.GetChild(0).gameObject;
        handle2 = transform.GetChild(1).gameObject;
        beam = transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (handle2.transform.position - handle1.transform.position).magnitude;
        Vector3 midpoint = (handle1.transform.position + handle2.transform.position) / 2.0f;
        Vector3 rotvec = (handle2.transform.position - handle1.transform.position).normalized;
        //beam.transform.localScale = new Vector3(beam.transform.localScale.z, distance / 2.0f, beam.transform.localScale.x );
        beam.transform.localScale = new Vector3(1.0f, 1.0f, distance / 2.0f);
        beam.transform.position = midpoint;
        rotation = Quaternion.LookRotation(rotvec);
        beam.transform.rotation = rotation;
        

    }
}

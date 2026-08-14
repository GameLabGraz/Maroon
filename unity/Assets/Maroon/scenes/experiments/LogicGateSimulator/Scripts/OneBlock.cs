using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneBlock : MonoBehaviour
{
    GameObject output;
    // Start is called before the first frame update
    void Start()
    {
       output = transform.Find("Output").gameObject;
       output.GetComponent<Output>().OutputValue = 1;
       
    }

    // Update is called once per frame
    void Update()
    {

    }
}

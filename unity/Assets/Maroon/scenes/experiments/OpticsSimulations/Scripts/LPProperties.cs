using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LPProperties : MonoBehaviour
{
    // Start is called before the first frame update

    public Color laserColor = Color.red;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    { // just always set to correct height
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x, 1.88f, gameObject.transform.position.z);
    }
}

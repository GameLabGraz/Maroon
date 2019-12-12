using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerPrefab : MonoBehaviour
{
    Transform RulerStart;
    Transform RulerEnd;
    Transform RulerLine;

    // Start is called before the first frame update
    void Start()
    {
        RulerStart = this.transform.Find("RulerStart");
        RulerEnd = this.transform.Find("RulerEnd");
        RulerLine = this.transform.Find("RulerLine");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            RulerEnd.transform.position = RulerEnd.transform.position + new Vector3(1.0f, 0.0f, 0.0f);
            Debug.Log("My world position = " + RulerStart.transform.position);
        }

        RulerLine.GetComponent<LineRenderer>().SetPosition(0, RulerStart.transform.position);
        RulerLine.GetComponent<LineRenderer>().SetPosition(1, RulerEnd.transform.position);
    }
}

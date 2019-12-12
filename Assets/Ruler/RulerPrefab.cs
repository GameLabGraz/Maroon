using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerPrefab : MonoBehaviour
{
    bool locked = false;
    Vector3 start_to_end;
    Transform RulerEverything;
    Transform RulerStart;
    Transform RulerEnd;
    Transform RulerLine;

    // Start is called before the first frame update
    void Start()
    {
        RulerEverything = this.transform.Find("RulerEverything");
        RulerStart = RulerEverything.transform.Find("RulerStart");
        RulerEnd = RulerEverything.transform.Find("RulerEnd");
        RulerLine = RulerEverything.transform.Find("RulerLine");
    }

    // Update is called once per frame
    void Update()
    {
        RulerLine.GetComponent<LineRenderer>().SetPosition(0, RulerStart.transform.position);
        RulerLine.GetComponent<LineRenderer>().SetPosition(1, RulerEnd.transform.position);
    }
}

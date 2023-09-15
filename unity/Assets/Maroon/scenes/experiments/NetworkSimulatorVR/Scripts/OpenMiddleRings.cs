using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OpenMiddleRings : MonoBehaviour
{
    // Objects to check status
    public DragObject source;
    public DragObject destination;
    public DragObject gateway;

    public UnityEvent animate;
    public UnityEvent stop_animation;

    Animator anim;

    // Update is called once per frame

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if ( (source.source_snapped == true) &&
             (destination.destination_snapped == true) &&
             (gateway.gateway_snapped == true))
        {
            Debug.Log("here it is");
            anim.SetTrigger("trOpen");
            anim.SetTrigger("trShowTable");

        }
        else
        {

            anim.SetTrigger("trClose");
        }
    }



}

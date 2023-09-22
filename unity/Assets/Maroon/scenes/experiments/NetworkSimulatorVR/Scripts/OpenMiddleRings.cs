using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Experiments.NetworkSimulatorVR
{
    public class OpenMiddleRings : MonoBehaviour
    {
        // Objects to check status
        public DragObject source;
        public DragObject destination;
        public DragObject gateway;

        Animator anim;

        public UnityEvent stop_animation;
        public UnityEvent start_animation;
        // Update is called once per frame

        private void Start()
        {
            anim = GetComponent<Animator>();
        }
        void Update()
        {
            if ((source.source_snapped == true) &&
                 (destination.destination_snapped == true) &&
                 (gateway.gateway_snapped == true))
            {
                stop_animation.Invoke();
                //Debug.Log("here it is");
                anim.SetTrigger("trOpen");
                anim.SetTrigger("trShowTable");

            }
            else
            {

                anim.SetTrigger("trClose");
                start_animation.Invoke();
            }
        }



    }
}

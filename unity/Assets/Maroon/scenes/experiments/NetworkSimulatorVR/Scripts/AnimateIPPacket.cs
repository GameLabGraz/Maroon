using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Experiments.NetworkSimulatorVR
{
    public class AnimateIPPacket : MonoBehaviour
    {
        public RightManager rm;
        Animator anim;

        private bool goingToStartPosition = false;
        public float lerpSpeed = 1f;
        Vector3 startPos;
        bool waitOneFrame = false;

        public UnityEvent showDialog;
        private bool yellow_done = false;
   
        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
            startPos = transform.position;

        }

        // Update is called once per frame
        void Update()
        {

            if (goingToStartPosition == true)
            {
                Debug.Log("goingToStartPosition");
                //yellow_done = false;
                //blue_done = false;
                //orange_done = false;

                if (waitOneFrame)
                {
                    waitOneFrame = false;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, startPos, lerpSpeed * Time.deltaTime);
                    goingToStartPosition = false;
                }
            }

        }

        public void SendToStartPosition()
        {
            waitOneFrame = true;
            goingToStartPosition = true;
            yellow_done = false;
            anim.SetTrigger("trRestart");
            // transform.position = Vector3.Lerp(transform.position, startPos, lerpSpeed * Time.deltaTime);

        }

        //Simulation possible only when all the steps are completed


        public void simulateFirst()
        {

            if ((rm.dragObjects[0].source_snapped == true) &&
                (rm.dragObjects[1].destination_snapped == true) &&
                (rm.dragObjects[2].gateway_snapped == true) && yellow_done == false)
            {
                showDialog.Invoke();
                //Debug.Log("first");
                anim.SetTrigger("trSimulate");
                yellow_done = true;
            }

        }

    }
}
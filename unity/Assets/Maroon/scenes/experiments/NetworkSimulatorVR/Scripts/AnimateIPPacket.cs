using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        System.DateTime start_time;

        private bool yellow_done = false;
        private bool blue_done = false;
        private bool orange_done = false;
        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
            startPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if( yellow_done && blue_done && orange_done)
            {
                goingToStartPosition = true;
            }

            if (goingToStartPosition)
            {
                yellow_done = false;
                blue_done = false;
                orange_done = false;

                if (waitOneFrame)
                {
                    waitOneFrame = false;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, startPos, lerpSpeed * Time.deltaTime);
                }
            }

        }

        public void SendToStartPosition()
        {
            waitOneFrame = true;
            goingToStartPosition = true;
        }

        //Simulation possible only when all the steps are completed


        public void simulateFirst()
        {

            if ((rm.dragObjects[0].source_snapped == true) &&
                (rm.dragObjects[1].destination_snapped == true) &&
                (rm.dragObjects[2].gateway_snapped == true) && yellow_done == false)
            {
                anim.SetTrigger("trSimulate");
                start_time = System.DateTime.UtcNow;
                yellow_done = true;
            }

        }

        public void simulateSecond()
        {

            if ((rm.dragObjects[0].source_snapped == true) &&
                (rm.dragObjects[1].destination_snapped == true) &&
                (rm.dragObjects[2].gateway_snapped == true) && yellow_done)
            {
                Debug.Log("second");
                    //anim.SetTrigger("trSimulate");
                    blue_done = true;
                
                
                
            }

        }

        public void simulateThird()
        {

            if ((rm.dragObjects[0].source_snapped == true) &&
                (rm.dragObjects[1].destination_snapped == true) &&
                (rm.dragObjects[2].gateway_snapped == true) && blue_done && yellow_done)
            {
                Debug.Log("third");
                //anim.SetTrigger("trSimulate");
                orange_done = true;
             }
        

        }
    }
}
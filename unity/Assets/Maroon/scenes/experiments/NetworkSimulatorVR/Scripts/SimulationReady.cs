using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.NetworkSimulatorVR
{
    public class SimulationReady : MonoBehaviour
    {
        public LeftManager lm;
        public RightManager rm;

        public GameObject connected;
        public GameObject connecting;
        public GameObject nosignal;

        public GameObject yellow;
        public GameObject green;

        // Start is called before the first frame update
        void Start()
        {
            nosignal.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            if( (lm.dragObjects[0].source_snapped == true) ||
                (lm.dragObjects[1].destination_snapped == true) ||
                (lm.dragObjects[0].gateway_snapped == true))
            {
                nosignal.SetActive(false);
                connecting.SetActive(true);

                if( (rm.dragObjects[0].source_snapped == true) &&
                    (rm.dragObjects[1].destination_snapped == true) &&
                    (rm.dragObjects[2].gateway_snapped == true) )
                {
                    connecting.SetActive(false);
                    connected.SetActive(true);
                }
                else
                {
                    connected.SetActive(false);
                    connecting.SetActive(true);
                }
            }
            else
            {
                connecting.SetActive(false);
                nosignal.SetActive(true);
            }


        }
    }
}

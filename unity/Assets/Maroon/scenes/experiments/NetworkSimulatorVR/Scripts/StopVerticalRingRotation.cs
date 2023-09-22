using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.NetworkSimulatorVR
{
    public class StopVerticalRingRotation : MonoBehaviour
    {
        Vector3 start_position;
        // Start is called before the first frame update
        void Start()
        {
            start_position = transform.position;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ResetPositon()
        {
            transform.position = Vector3.Lerp(transform.position, start_position, 1 * Time.deltaTime);
        }
    }

}

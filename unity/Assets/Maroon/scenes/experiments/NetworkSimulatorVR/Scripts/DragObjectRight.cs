using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Maroon.Experiments.NetworkSimulatorVR
{
    public class DragObjectRight : MonoBehaviour
    {
        public TextMeshPro Text;
        public bool goingToStartPosition = false;
        public DragSlotRight slot;

        public bool source_snapped;
        public bool destination_snapped;
        public bool gateway_snapped;

        public float lerpSpeed = 1f;

        Vector3 startPos;
        bool waitOneFrame = false;

        private void Start()
        {

            startPos = transform.position;
        }

        private void Update()
        {
            if (goingToStartPosition)
            {
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
    }
}

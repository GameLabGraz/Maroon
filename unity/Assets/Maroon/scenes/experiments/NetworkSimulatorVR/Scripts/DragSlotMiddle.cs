using GameLabGraz.VRInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Experiments.NetworkSimulatorVR
{
    public class DragSlotMiddle : MonoBehaviour
    {
        public MiddleManager Manager;

        public VRSnapDropZone snapZone;

        public GameObject HighlighBackground;
        public GameObject DefaultBackground;
        public GameObject CorrectBackground;
        public GameObject WrongBackground;

        public UnityEvent correctSnap;
        public UnityEvent wrongSnap;
        [Space(10)]
        public UnityEvent unsnapEvent;

        public VRSnapDropZone snapDropZone;

        public DragObjectMiddle objectInSlot = null;

        public void Check(bool isOk)
        {

            HighlighBackground.SetActive(true);

        }

        public void OnSnapExit()
        {
            HighlighBackground.SetActive(false);
        }

        public void UnleashSnapEvent(VRSnapDropZone zone, GameObject gameObject)
        {
            var dragObject = gameObject.GetComponent<DragObjectMiddle>();
            if (dragObject != null)
            {
                if (objectInSlot == null)
                {
                    if (dragObject.slot == this)
                    {
                        CorrectBackground.SetActive(true);
                        WrongBackground.SetActive(false);
                        correctSnap.Invoke();


                        if (dragObject.name == "SourceL")
                        {
                            Debug.Log("snapped: " + dragObject.name);
                            Debug.Log("snapped: " + dragObject.source_snapped);
                            dragObject.source_snapped = true;
                            Debug.Log("snapped: " + dragObject.source_snapped);

                        }

                        if (dragObject.name == "DestinationL")
                        {
                            Debug.Log("snapped: " + dragObject.name);
                            dragObject.destination_snapped = true;

                        }
                        if (dragObject.name == "GatewayL")
                        {
                            Debug.Log("snapped: " + dragObject.name);
                            dragObject.gateway_snapped = true;

                        }

                    }
                    else
                    {
                        CorrectBackground.SetActive(false);
                        WrongBackground.SetActive(true);
                        wrongSnap.Invoke();


                    }
                    objectInSlot = dragObject;

                }
            }
            HighlighBackground.SetActive(false);
        }

        public void UnleashUnsnapEvent()
        {
            // Stop particle animations
            unsnapEvent.Invoke();

            WrongBackground.SetActive(false);
            CorrectBackground.SetActive(false);
            DefaultBackground.SetActive(true);
            HighlighBackground.SetActive(false);
            objectInSlot = null;
        }

        public void Restart()
        {
            WrongBackground.SetActive(false);
            CorrectBackground.SetActive(false);
            DefaultBackground.SetActive(true);
            HighlighBackground.SetActive(false);

            snapZone.snappedObject = null;

            if (objectInSlot != null)
            {
                objectInSlot.source_snapped = false;
                objectInSlot.destination_snapped = false;
                objectInSlot.gateway_snapped = false;
                objectInSlot.SendToStartPosition();
            }
            objectInSlot = null;
        }
    }
}

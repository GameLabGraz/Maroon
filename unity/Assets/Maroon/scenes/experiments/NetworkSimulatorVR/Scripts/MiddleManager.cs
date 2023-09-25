using GameLabGraz.VRInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Maroon.Experiments.NetworkSimulatorVR
{
    public class MiddleManager : MonoBehaviour
    {
        // Left objects - for values and confirmation of snap
        public DragObject sourceL;
        public DragObject destinationL;
        public DragObject gatewayL;

        // Set values
        public TextMeshProUGUI incoming;
        public TextMeshProUGUI outgoing;

        public DragObjectMiddle currentObject;

        public List<DragObjectMiddle> dragObjects;
        public List<DragSlotMiddle> slots;

        DragSlotMiddle lastCheckedSlot = null;

        //private int random_value_check;
        //private bool ip_showed = false;

        void Start()
        {
            dragObjects[0].Text.text = "XXX.XXX.XXX.XXX";
            dragObjects[1].Text.text = "XXX.XXX.XXX.XXX";
            dragObjects[2].Text.text = "XXX.XXX.XXX.XXX";
            dragObjects[3].Text.text = "XXX.XXX.XXX.XXX";

        }

        private void Update()
        {
            if ( (sourceL.source_snapped == true) &&
                ( destinationL.destination_snapped == true) &&
                ( gatewayL.gateway_snapped == true) )
            {
                dragObjects[0].Text.text = sourceL.Text.text;
                dragObjects[1].Text.text = destinationL.Text.text;
                dragObjects[2].Text.text = outgoing.text;
                dragObjects[3].Text.text = incoming.text;

            }
            else
            {
                Restart();
                dragObjects[0].Text.text = "XXX.XXX.XXX.XXX";
                dragObjects[1].Text.text = "XXX.XXX.XXX.XXX";
                dragObjects[2].Text.text = "XXX.XXX.XXX.XXX";
                dragObjects[3].Text.text = "XXX.XXX.XXX.XXX";
            }
        }
        public void Check(VRSnapDropZone zone)
        {
            if (currentObject != null)
            {
                var slot = zone.GetComponent<DragSlotMiddle>();
                bool isOk = currentObject.slot == slot;
                slot.Check(isOk);
                if (slot.objectInSlot == null)
                {
                    lastCheckedSlot = slot;
                }
            }
        }

        public void OnPickupObject(DragObjectMiddle dragObject)
        {
            //Debug.Log("OnPickupObject");
            currentObject = dragObject;
            dragObject.goingToStartPosition = false;
            lastCheckedSlot = null;
        }

        public void OnDetach()
        {
            if (currentObject != null && (lastCheckedSlot == null || lastCheckedSlot.objectInSlot == null))
            {
                currentObject.SendToStartPosition();
            }
            currentObject = null;

            for (int i = 0; i < slots.Count; ++i)
            {
                slots[i].HighlighBackground.SetActive(false);
            }
        }

        public void Restart()
        {

            // Assign new addresses
            dragObjects[0].Text.text = sourceL.Text.text;
            dragObjects[1].Text.text = destinationL.Text.text;
            dragObjects[2].Text.text = outgoing.text;
            dragObjects[3].Text.text = incoming.text;

            // Reset object position
            dragObjects[0].slot.Restart();
            dragObjects[1].slot.Restart();
            dragObjects[2].slot.Restart();
            dragObjects[3].slot.Restart();

            // Reset slots
            slots[0].UnleashUnsnapEvent();
            slots[1].UnleashUnsnapEvent();
            slots[2].UnleashUnsnapEvent();
            slots[3].UnleashUnsnapEvent();
        }
    }
}

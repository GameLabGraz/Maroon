using GameLabGraz.VRInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace Maroon.Experiments.NetworkSimulatorVR
{
    public class RightManager : MonoBehaviour
    {
        // Middle objects to check the values
        public DragObjectMiddle sourceM;
        public DragObjectMiddle destinationM;
        public DragObjectMiddle outgoing;
        public DragObjectMiddle incoming;

        public TextMeshProUGUI gateway;

        public GameObject connected;
        public GameObject connecting;
        public GameObject nosignal;

        public DragObjectRight currentObject;

        public List<DragObjectRight> dragObjects;
        public List<DragSlotRight> slots;

        DragSlotRight lastCheckedSlot = null;

        public GameObject InactiveSource;
        public GameObject InactiveDestination;
        public GameObject InactiveGateway;

        public bool unlocked = false;
        public UnityEvent unlockGrid;
        public UnityEvent lockGrid;
        void Start()
        {
            InactiveSource.SetActive(true);
            InactiveDestination.SetActive(true);
            InactiveGateway.SetActive(true);

            dragObjects[0].Text.text = "XXX.XXX.XXX.XXX";
            dragObjects[1].Text.text = "XXX.XXX.XXX.XXX";
            dragObjects[2].Text.text = "XXX.XXX.XXX.XXX";

        }

        void Update()
        {

            if ((sourceM.source_snapped == true) &&
                 (destinationM.destination_snapped == true) &&
                 (outgoing.position_snapped == true) &&
                 (incoming.position_snapped == true))
            {
                //Debug.Log("RM:::: True");
                unlockGrid.Invoke();
                unlocked = true;
                dragObjects[0].Text.text = sourceM.Text.text;
                dragObjects[1].Text.text = destinationM.Text.text;
                dragObjects[2].Text.text = gateway.text;

                connected.SetActive(true);
                connecting.SetActive(false);
                nosignal.SetActive(false);
            }
            else
            {
                Restart();
                lockGrid.Invoke();
                unlocked = false;

                dragObjects[0].Text.text = "XXX.XXX.XXX.XXX";
                dragObjects[1].Text.text = "XXX.XXX.XXX.XXX";
                dragObjects[2].Text.text = "XXX.XXX.XXX.XXX";
                connected.SetActive(false);

                if ((sourceM.source_snapped == true) ||
                    (destinationM.destination_snapped == true) ||
                    (outgoing.position_snapped == true) ||
                    (incoming.position_snapped == true))
                {
                    connecting.SetActive(true);
                    nosignal.SetActive(false);
                }
                else
                {
                    connecting.SetActive(false);
                    nosignal.SetActive(true);
                }

            }

        }

        public void Check(VRSnapDropZone zone)
        {
            if ((currentObject != null) && (unlocked == true))
            {
                var slot = zone.GetComponent<DragSlotRight>();
                bool isOk = currentObject.slot == slot;
                slot.Check(isOk);
                if (slot.objectInSlot == null)
                {
                    lastCheckedSlot = slot;
                }
            }
        }

        public void OnPickupObject(DragObjectRight dragObject)
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

            // Reset object position
            dragObjects[0].slot.Restart();
            dragObjects[1].slot.Restart();
            dragObjects[2].slot.Restart();

            // Reset slots
            slots[0].UnleashUnsnapEvent();
            slots[1].UnleashUnsnapEvent();
            slots[2].UnleashUnsnapEvent();

        }

    }
}

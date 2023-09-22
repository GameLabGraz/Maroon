using GameLabGraz.VRInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Maroon.Experiments.NetworkSimulatorVR
{
    public class LeftManager : MonoBehaviour
    {
        public TextMeshProUGUI IpText;
        public TextMeshProUGUI gateway;
        public DragObjectMiddle clientM;
        public DragObjectMiddle targetM;
        public TextMeshProUGUI hop;

        public DragObject currentObject;

        public List<DragObject> dragObjects;
        public List<DragSlot> slots;

        DragSlot lastCheckedSlot = null;

        private int random_value_check;
        private bool ip_showed = false;

        void Start()
        {

            dragObjects[0].Text.text = SourceIpAddress();
            dragObjects[1].Text.text = DestinationIpAddress();

            //Middle objects
            clientM.Text.text = dragObjects[0].Text.text;
            targetM.Text.text = dragObjects[1].Text.text;

            //Gateways set in SourceIPAddress();

            //Show nothing at the beginning
            IpText.text = "";
            gateway.text = "";

        }


        public void Check(VRSnapDropZone zone)
        {
            if (currentObject != null)
            {
                var slot = zone.GetComponent<DragSlot>();
                bool isOk = currentObject.slot == slot;
                slot.Check(isOk);
                if (slot.objectInSlot == null)
                {
                    lastCheckedSlot = slot;
                }
            }
        }

        public void OnPickupObject(DragObject dragObject)
        {
            Debug.Log("OnPickupObject");
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
            //Reset terminal field
            IpText.text = "";
            gateway.text = "";
            ip_showed = false;

            // Assign new addresses
            dragObjects[0].Text.text = SourceIpAddress();
            dragObjects[1].Text.text = DestinationIpAddress();

            //Middle objects
            clientM.Text.text = dragObjects[0].Text.text;
            targetM.Text.text = dragObjects[1].Text.text;
            

            // Reset object position
            dragObjects[0].slot.Restart();
            dragObjects[1].slot.Restart();
            dragObjects[2].slot.Restart();

            // Reset slots
            slots[0].UnleashUnsnapEvent();
            slots[1].UnleashUnsnapEvent();
            slots[2].UnleashUnsnapEvent();
        }

        // Source Range
        // 10.0.0.1 to 10.255.255.254
        // 172.16.0.0 to 172.31.255.255
        // 192.168.0.0 – 192.168.255.255
        // Gateway: x.x.x.1
        string SourceIpAddress()
        {
            int octet1, octet2, octet3, octet4;

            //Select address range on random
            int random = Random.Range(1, 3);

            random_value_check = random;
            switch (random)
            {
                case 1: //10
                    octet1 = 10;
                    octet2 = Random.Range(0, 255);
                    octet3 = Random.Range(0, 255);
                    octet4 = Random.Range(2, 254);
                    break;
                case 2: //172
                    octet1 = 172;
                    octet2 = Random.Range(16, 31);
                    octet3 = Random.Range(0, 255);
                    octet4 = Random.Range(2, 254);
                    break;
                case 3: // 192
                    octet1 = 192;
                    octet2 = 168;
                    octet3 = Random.Range(0, 255);
                    octet4 = Random.Range(2, 254);
                    break;
                default:
                    octet1 = 0;
                    octet2 = 0;
                    octet3 = 0;
                    octet4 = 0;
                    break;
            }

            //Set gateway
            dragObjects[2].Text.text = octet1 + "." + octet2 + "." + octet3 + "." + "1";

            //Set IP
            string new_ip = octet1 + "." + octet2 + "." + octet3 + "." + octet4;


            return new_ip;
        }

        // Destination Range
        // 10.0.0.1 to 10.255.255.254
        // 172.16.0.0 to 172.31.255.255
        // 192.168.0.0 – 192.168.255.255
        // Gateway: x.x.x.1
        string DestinationIpAddress()
        {
            int octet1, octet2, octet3, octet4;

            //Select address range on random
            int random = Random.Range(1, 3);

            //Check if the random is free,
            // otherwise assign new value
            if (random_value_check == random)
            {
                if (random_value_check == 1)
                {
                    random = Random.Range(2, 3);
                }
                else if (random_value_check == 3)
                {
                    random = Random.Range(1, 2);
                }
                else if (random_value_check == 2)
                {
                    random = 2;
                }
            }

            switch (random)
            {
                case 1: //10
                    octet1 = 10;
                    octet2 = Random.Range(0, 255);
                    octet3 = Random.Range(0, 255);
                    octet4 = Random.Range(2, 254);
                    break;
                case 2: //172
                    octet1 = 172;
                    octet2 = Random.Range(16, 31);
                    octet3 = Random.Range(0, 255);
                    octet4 = Random.Range(2, 254);
                    break;
                case 3: // 192
                    octet1 = 192;
                    octet2 = 168;
                    octet3 = Random.Range(0, 255);
                    octet4 = Random.Range(2, 254);
                    break;
                default:
                    octet1 = 0;
                    octet2 = 0;
                    octet3 = 0;
                    octet4 = 0;
                    break;
            }

            //Set IP
            string new_ip = octet1 + "." + octet2 + "." + octet3 + "." + octet4;

            //Next hop
            hop.text = octet1 + "." + octet2 + "." + octet3 + ".0";


            return new_ip;
        }

        public void ShowIP()
        {
            IpText.text =  "IPv4: " + dragObjects[0].Text.text;
            gateway.text = "Gateway: " + dragObjects[2].Text.text;

            ip_showed = true;

        }
    }
}

using GameLabGraz.VRInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class RightManager : MonoBehaviour
{
    public DragObject sourceL;
    public DragObject destinationL;
    public DragObject gatewayL;
    public DragObjectRight currentObject;

    public List<DragObjectRight> dragObjects;
    public List<DragSlotRight> slots;

    DragSlotRight lastCheckedSlot = null;

    public GameObject InactiveSource;
    public GameObject InactiveDestination;
    public GameObject InactiveGateway;

    private int random_value_check;
    public bool unlocked = false;
    public UnityEvent unlockGrid;
    public UnityEvent lockGrid;
    void Start()
    {
        InactiveSource.SetActive(true);
        InactiveDestination.SetActive(true);
        InactiveGateway.SetActive(true);

        dragObjects[0].Text.text = SourceIpAddress();
        dragObjects[1].Text.text = DestinationIpAddress();

        //Gateways set in SourceIPAddress();

    }

    void Update()
    {
        
        if ( (sourceL.source_snapped == true) &&
             (destinationL.destination_snapped == true) &&
             (gatewayL.gateway_snapped == true) )
        {
            //Debug.Log("RM:::: True");
            unlockGrid.Invoke();
            unlocked = true;
        }
        else
        {
            lockGrid.Invoke();
            unlocked = false;
        }
        
    }

    public void Check(VRSnapDropZone zone)
    {
        if (currentObject != null)
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

        // Assign new addresses
        dragObjects[0].Text.text = SourceIpAddress();
        dragObjects[1].Text.text = DestinationIpAddress();

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


        return new_ip;
    }

    public void ShowIP()
    {


    }
}

using GameLabGraz.VRInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DragManager : MonoBehaviour
{
    public TextMeshProUGUI IpText;
    public TextMeshProUGUI gateway;

    public DragObject currentObject;
    public List<DragObject> dragObjects;
    public List<DragSlot> slots;

    DragSlot lastCheckedSlot = null;
   

    private void Start()
    {
        
        dragObjects[0].Text.text = SourceIpAddress();
        dragObjects[1].Text.text = DestinationIpAddress();
       
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

    //Private Range A:
    // 10.0.0.1 to 10.255.255.254
    //Gateway: 10.x.x.1
    string SourceIpAddress()
    {

        int octect2 = Random.Range(0, 255);
        int octect3 = Random.Range(0, 255);
        int octect4 = Random.Range(2, 254);

        //Set gateway
        dragObjects[2].Text.text = "10." + octect2 + "." + octect3 + "." + "1";

        //Set IP
        string new_ip = "10." + octect2 + "." + octect3 + "." + octect4;

        return new_ip;
    }

    //Private Range B
    // 172.16.0.0 to 172.31.255.255
    string DestinationIpAddress()
    {
        string new_ip = "172." + Random.Range(16, 31) + "." + Random.Range(0, 255) + "." + Random.Range(0, 253);

        return new_ip;
    }

    public void ShowIP()
    {
        IpText.text = "IPv4: " + dragObjects[0].Text.text;
        gateway.text = "Gateway: " + dragObjects[2].Text.text;
    }
}

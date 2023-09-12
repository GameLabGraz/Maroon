using GameLabGraz.VRInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DragManager : MonoBehaviour
{
    public TextMeshProUGUI IpText;

    public DragObject currentObject;
    public List<DragObject> dragObjects;
    public List<DragSlot> slots;

    DragSlot lastCheckedSlot = null;

    string source_ = "";
    string destination_ = "";


    private void Start()
    {
        //IpText.text = NewIpAddress();
        source_ = NewIpAddress();
        destination_ = NewIpAddress();
        //currentObject.Text.text = source_;
        dragObjects[0].Text.text = source_;
        //Show nothing at the beginning
        IpText.text = "";
        //Restart();
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

        //Assign new values
        source_ = NewIpAddress();
        destination_ = NewIpAddress();

        //reset postion
    }

    string NewIpAddress()
    {
        string new_ip = Random.Range(1, 254) + "." + Random.Range(1, 254) + "." + Random.Range(1, 254) + "." + Random.Range(1, 254);
       
        return new_ip;
    }

    public void ShowIP()
    {
        IpText.text = source_;
    }
}

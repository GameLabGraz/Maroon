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

    string ip = "";
    
    public List<string> fixedIpList0 = new List<string>();
    public List<string> fixedIpList1 = new List<string>();
    public List<string> fixedIpList2 = new List<string>();
    public List<string> fixedIpList3 = new List<string>(); 

    private void Start()
    {
        IpText.text = "";
        Restart();
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
        IpText.text = "";

        List<DragObject> shuffled = new List<DragObject>();
        while (shuffled.Count < dragObjects.Count)
        {
            int r = Random.Range(0, dragObjects.Count);
            if (!shuffled.Contains(dragObjects[r]))
            {
                shuffled.Add(dragObjects[r]);
            }
        }

        int chosenList = Random.Range(0, 4);
        List<string> chosenFixedIpList = null;
        switch(chosenList)
        {
            case 0:
                chosenFixedIpList = fixedIpList0;
                break;
            case 1:
                chosenFixedIpList = fixedIpList1;
                break;
            case 2:
                chosenFixedIpList = fixedIpList2;
                break;
            case 3:
                chosenFixedIpList = fixedIpList3;
                break;
        }
        //because first 3 elements in list are correct
        
        /*
        for (int i = 0; i < shuffled.Count; ++i)
        {
            if (i == 0)
            {
                ip = chosenFixedIpList[i];
            }
            shuffled[i].Text.text = chosenFixedIpList[i];
            if (i < 3)
            {
                shuffled[i].slot = slots[i];
            } else
            {
                shuffled[i].slot = null;
            }
        }*/

        for (int i = 0; i < shuffled.Count; ++i)
        {
            string generatedIP = "";
            if (i < 3)
            {
                if (i == 0)
                {
                    ip = chosenFixedIpList[i];
                    //generatedIP = make correct for source
                    generatedIP = "correctSource";
                }
                else if (i == 1)
                {
                    //generatedIP = make correct for destination
                }
                else if (i == 2)
                {
                    //generatedIP = make correct for interface
                }
                shuffled[i].Text.text = generatedIP;
                shuffled[i].slot = slots[i];
            }
            else
            {
                //put anything here in ip
                generatedIP = NewIpAddress();
                shuffled[i].slot = null;
                shuffled[i].Text.text = generatedIP;
            }
        }

        for (int i = 0; i < slots.Count; ++i)
        {
            slots[i].Restart();
        }
    }

    string NewIpAddress()
    {
       // string x = Random.Range(100,200) + "." + Random
        string s = "192.168.0.1";
        return s;
    }

    public void ShowIP()
    {
        IpText.text = ip;
    }
}

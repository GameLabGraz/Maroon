using GameLabGraz.VRInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragManager : MonoBehaviour
{
    public DragObject currentObject;
    public DragSlot[] slots;

    DragSlot lastCheckedSlot = null;

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

        for (int i = 0; i < slots.Length; ++i)
        {
            slots[i].HighlighBackground.SetActive(false);
        }
    }
}

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
    public TextMeshProUGUI hop;
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
        
        if ( (sourceL.source_snapped == true) &&
             (destinationL.destination_snapped == true) &&
             (gatewayL.gateway_snapped == true) )
        {
            //Debug.Log("RM:::: True");
            unlockGrid.Invoke();
            unlocked = true;
            dragObjects[0].Text.text = gatewayL.Text.text;
            dragObjects[1].Text.text = destinationL.Text.text;
            dragObjects[2].Text.text = hop.text;
        }
        else
        {
            lockGrid.Invoke();
            unlocked = false;
            dragObjects[0].Text.text = "XXX.XXX.XXX.XXX";
            dragObjects[1].Text.text = "XXX.XXX.XXX.XXX";
            dragObjects[2].Text.text = "XXX.XXX.XXX.XXX";
        }
        
    }

    public void Check(VRSnapDropZone zone)
    {
        if ( ( currentObject != null) && (unlocked == true))
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
        dragObjects[0].Text.text = gatewayL.Text.text;
        dragObjects[1].Text.text = destinationL.Text.text;
        dragObjects[2].Text.text = hop.text;

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

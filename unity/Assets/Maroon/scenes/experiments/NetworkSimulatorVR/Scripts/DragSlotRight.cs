using GameLabGraz.VRInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DragSlotRight : MonoBehaviour
{
    public RightManager Manager;

    public VRSnapDropZone snapZone;

    public GameObject HighlighBackground;
    public GameObject DefaultBackground;
    public GameObject CorrectBackground;
    public GameObject WrongBackground;
    //public GameObject LockedBackground;

    public UnityEvent correctSnap;
    public UnityEvent wrongSnap;
    [Space(10)]
    public UnityEvent unsnapEvent;

    public VRSnapDropZone snapDropZone;

    public DragObjectRight objectInSlot = null;



    public void Check(bool isOk)
    {
        HighlighBackground.SetActive(true);
        if (isOk)
        {
            Debug.Log("is OK");
        }
        else
        {
            Debug.Log("is bad");
        }
    }


    public void OnSnapExit()
    {
        HighlighBackground.SetActive(false);
    }

    public void UnleashSnapEvent(VRSnapDropZone zone, GameObject gameObject)
    {
        var dragObject = gameObject.GetComponent<DragObjectRight>();
        if (dragObject != null)
        {
            if (objectInSlot == null)
            {
                if (dragObject.slot == this)
                {
                    CorrectBackground.SetActive(true);
                    WrongBackground.SetActive(false);
                    correctSnap.Invoke();


                    if (dragObject.name == "SourceR")
                    {
                        Debug.Log("snapped: " + dragObject.name);
                        dragObject.source_snapped = true;

                    }

                    if (dragObject.name == "DestinationR")
                    {
                        Debug.Log("snapped: " + dragObject.name);
                        dragObject.destination_snapped = true;

                    }
                    if (dragObject.name == "GatewayR")
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
        if (objectInSlot.name == "SourceR")
        {
            Debug.Log("UNsnapped: " + objectInSlot.name);
            objectInSlot.source_snapped = false;

        }

        if (objectInSlot.name == "DestinationR")
        {
            Debug.Log("UNsnapped: " + objectInSlot.name);
            objectInSlot.destination_snapped = false;

        }
        if (objectInSlot.name == "GatewayR")
        {
            Debug.Log("UNsnapped: " + objectInSlot.name);
            objectInSlot.gateway_snapped = false;

        }

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
            objectInSlot.SendToStartPosition();
        }
        objectInSlot = null;
    }
}

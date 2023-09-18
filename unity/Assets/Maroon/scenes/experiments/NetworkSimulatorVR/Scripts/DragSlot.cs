using GameLabGraz.VRInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DragSlot : MonoBehaviour
{
    public LeftManager Manager;

    public VRSnapDropZone snapZone;

    public GameObject HighlighBackground;
    public GameObject DefaultBackground;
    public GameObject CorrectBackground;
    public GameObject WrongBackground;

    public UnityEvent correctSnap;
    public UnityEvent wrongSnap;
    [Space(10)]
    public UnityEvent unsnapEvent;

    public VRSnapDropZone snapDropZone;

    public DragObject objectInSlot = null;

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
        var dragObject = gameObject.GetComponent<DragObject>();
        if (dragObject != null)
        {
            if (objectInSlot == null)
            {
                if (dragObject.slot == this)
                {
                    CorrectBackground.SetActive(true);
                    WrongBackground.SetActive(false);
                    correctSnap.Invoke();


                    if (dragObject.name == "SourceL")
                    {
                        Debug.Log("snapped: " + dragObject.name);
                        dragObject.source_snapped = true;

                    }

                    if (dragObject.name == "DestinationL")
                    {
                        Debug.Log("snapped: " + dragObject.name);
                        dragObject.destination_snapped = true;

                    }
                    if (dragObject.name == "GatewayL")
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
        if (objectInSlot.name == "SourceL")
        {
            Debug.Log("UNsnapped: " + objectInSlot.name);
            objectInSlot.source_snapped = false;
            
        }

        if (objectInSlot.name == "DestinationL")
        {
            Debug.Log("UNsnapped: " + objectInSlot.name);
            objectInSlot.destination_snapped = false;

        }
        if (objectInSlot.name == "GatewayL")
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

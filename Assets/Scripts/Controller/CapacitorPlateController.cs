using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CapacitorPlateController : VRTK_InteractableObject
{
    private GameObject scaleHeightObject1;
    private GameObject scaleHeightObject2;
    private GameObject scaleWidthObject1;
    private GameObject scaleWidthObject2;

    private float scaleObjectSize = 0.02f;

    private void Start()
    {
        Vector3 size =  GetComponent<Renderer>().bounds.size;
        Vector3 offset_x = new Vector3(size.x / 2, 0, 0);
        Vector3 offset_y = new Vector3(0, size.y / 2, 0);

        scaleWidthObject1 = CreateScaleObject(transform.position + offset_x);
        scaleWidthObject2 = CreateScaleObject(transform.position - offset_x);
        scaleHeightObject1 = CreateScaleObject(transform.position + offset_y);
        scaleHeightObject2 = CreateScaleObject(transform.position - offset_y);
        EnableScaleObjects(false);
    }

    private GameObject CreateScaleObject(Vector3 position)
    {
        GameObject scaleObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        scaleObject.transform.position = position;
        scaleObject.transform.localScale = new Vector3(scaleObjectSize, scaleObjectSize, scaleObjectSize);
        scaleObject.GetComponent<Renderer>().material.color = Color.black;
        scaleObject.GetComponent<Collider>().isTrigger = true;
        Physics.IgnoreCollision(GetComponent<Collider>(), scaleObject.GetComponent<Collider>(), true);
        return scaleObject;
    }

    private void EnableScaleObjects(bool value)
    {
        scaleWidthObject1.SetActive(value);
        scaleWidthObject2.SetActive(value);
        scaleHeightObject1.SetActive(value);
        scaleHeightObject2.SetActive(value);
    }

    public override void StartTouching(GameObject currentTouchingObject)
    {
        base.StartTouching(currentTouchingObject);

        EnableScaleObjects(true);
    }

    
}

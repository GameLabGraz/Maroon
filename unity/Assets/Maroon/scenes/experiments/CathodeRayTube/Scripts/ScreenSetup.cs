using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ScreenSetup : MonoBehaviour
{

    public GameObject Pixel;
    private List<GameObject> pixelList = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        var screenTransform = transform;
        Vector3 screenPosition = screenTransform.position;
        Vector3 screenScale = screenTransform.localScale;


        var localScale = Pixel.transform.localScale;
        float pixelHeight = localScale.y;
        float pixelWidth = localScale.z;
        
        for (float y = screenPosition.y - screenScale.y / 2; 
            y <= screenPosition.y + screenScale.y / 2; 
            y += pixelHeight)
        {
            for (float z = screenPosition.z - screenScale.z / 2;
                z <= screenPosition.z + screenScale.z / 2;
                z += pixelWidth)
            {
                GameObject pixel = Instantiate(Pixel, new Vector3(screenPosition.x, y, z), Pixel.transform.rotation);
                pixel.transform.name = "pixel_" + y + "_" + z;
                pixel.GetComponent<MeshRenderer>().enabled = false;
                pixelList.Add(pixel);
            }
        }  
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
        Vector3 contactPoint = collision.GetContact(0).point;
        ActivatePixel(contactPoint);
    }

    private void ActivatePixel(Vector3 contactPoint)
    {
        GameObject closestPixel = pixelList[0];
        float closestDistance = float.MaxValue;

        foreach (var pixel in pixelList)
        {
            float tempDistance = Vector3.Distance(contactPoint, pixel.transform.position);
            if (!(tempDistance < closestDistance)) continue;
            closestDistance = tempDistance;
            closestPixel = pixel;
        }

        closestPixel.GetComponent<MeshRenderer>().enabled = true;
    }
}
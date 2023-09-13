using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics
{
  public class MeasurementManager : MonoBehaviour
  {
    public Camera main_camera;
    public Camera zoom_camera;
    public List<MeasurableObject> measurableObjects;
    public MeasurableObject measuredObject;
    private bool measuring = false;
    // Start is called before the first frame update
    void Start()
    {
      
      getAllMeasurableObjects();
    }

    // Update is called once per frame
    void Update()
    {
      if(measuredObject && !measuring)
      {
        measuring = true;
        startMeasuringMode();
      }
      else if (!measuredObject && measuring)
      {
        measuring = false;
        endMeasuringMode();
      }
    }

    void startMeasuringMode()
    {
      main_camera.gameObject.SetActive(false);
      zoom_camera.transform.position = new Vector3(measuredObject.transform.position.x, measuredObject.transform.position.y, zoom_camera.transform.position.z);
      fitObjectToCamera();
      zoom_camera.enabled = true;
    }

    void endMeasuringMode()
    {
      main_camera.gameObject.SetActive(true);
      zoom_camera.enabled = false;
    }

    void fitObjectToCamera()
    {
      var objectHeight = measuredObject.transform.localScale.y;
      var objectWidth = measuredObject.transform.localScale.x;

      if(objectHeight >= objectWidth)
      {
        zoom_camera.orthographicSize = objectHeight;
      }
      else
      {
        zoom_camera.orthographicSize = objectWidth;
      }

    }

    void resetCamera()
    {

    }

    void getAllMeasurableObjects()
    {
      Object[] foundObjects = Object.FindObjectsOfType<MeasurableObject>();

      measurableObjects = new List<MeasurableObject>();

      foreach (Object obj in foundObjects)
      {
        measurableObjects.Add(obj as MeasurableObject);
      }
    }
  }
}
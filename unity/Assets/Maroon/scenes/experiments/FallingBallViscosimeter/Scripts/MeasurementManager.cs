using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics
{
  public class MeasurementManager : MonoBehaviour
  {
    public Caliper caliperPrefab;
    private Caliper current_caliper = null;
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
      if(current_caliper != null)
      {
        Destroy(current_caliper);
      }
      main_camera.gameObject.SetActive(false);
      fitObjectToCamera();
      zoom_camera.transform.position = new Vector3(measuredObject.transform.position.x + zoom_camera.orthographicSize * 0.5f, measuredObject.transform.position.y, zoom_camera.transform.position.z);

      current_caliper = Instantiate(caliperPrefab,
                                    new Vector3(measuredObject.transform.position.x + 0.25f,
                                                measuredObject.transform.position.y + 0.05f,
                                                measuredObject.transform.position.z),
                                    Quaternion.identity);
      zoom_camera.enabled = true;
    }

    void endMeasuringMode()
    {
      Destroy(current_caliper);
      current_caliper = null;
      main_camera.gameObject.SetActive(true);
      zoom_camera.enabled = false;
    }

    void fitObjectToCamera()
    {
      var objectHeight = measuredObject.transform.localScale.y;
      var objectWidth = measuredObject.transform.localScale.x;

      if(objectHeight >= objectWidth)
      {
        zoom_camera.orthographicSize = objectHeight * 1.6f;
      }
      else
      {
        zoom_camera.orthographicSize = objectWidth * 1.6f;
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
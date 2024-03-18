using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Maroon.Physics
{
  public class MeasurementManager : MonoBehaviour
  {
    public static MeasurementManager Instance;
    public Caliper caliperPrefab;
    private Caliper current_caliper = null;
    public Camera main_camera;
    public Camera zoom_camera;
    public List<MeasurableObject> measurableObjects;
    public MeasurableObject measuredObject;
    public MeasurementState measurementState = MeasurementState.Off;

    public Button startButton;
    public Button endButton;
    public GameObject uiText;

    private bool measuring = false;
    private float minSize = 0.2f;

    private void Awake()
    { 
      if(Instance == null)
      {
        Instance = this;
      }
      getAllMeasurableObjects();
    }

    // Start is called before the first frame update
    void Start()
    {
    }


    void startMeasuringMode()
    {
      measurementState = MeasurementState.Measuring;
      startButton.gameObject.SetActive(false);
      uiText.gameObject.SetActive(false);
      endButton.gameObject.SetActive(true);
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
      startButton.interactable = false;
      endButton.interactable = true;
    }

    public void endMeasuringMode()
    {
      endButton.gameObject.SetActive(false);
      uiText.gameObject.SetActive(false);
      startButton.gameObject.SetActive(true);
      Destroy(current_caliper.gameObject);
      current_caliper = null;
      main_camera.gameObject.SetActive(true);
      zoom_camera.enabled = false;
      startButton.interactable = true;
      endButton.interactable = false;
      measurementState = MeasurementState.Off;
    }

    void fitObjectToCamera()
    {
      var objectHeight = measuredObject.transform.localScale.y;
      var objectWidth = measuredObject.transform.localScale.x;

      if (objectHeight * 1.6f < 0.2f && objectWidth * 1.6f < 0.2f)
      {
        zoom_camera.orthographicSize = 0.2f;
        return;
      }
      
      if(objectHeight >= objectWidth)
      {
        zoom_camera.orthographicSize = objectHeight * 1.6f;
      }
      else
      {
        zoom_camera.orthographicSize = objectWidth * 1.6f;
      }
      
      

    }

    public void chooseMeasuredObject()
    {
      measurementState = MeasurementState.ChooseObject;
      setAllChooseable(true);
    }


    private void setAllChooseable(bool chooseable)
    {
      foreach (MeasurableObject mObject in measurableObjects)
      {
        mObject.setChooseable(chooseable);
      }
      uiText.SetActive(chooseable);
    }

    public void setChosenObject(MeasurableObject mObject)
    {
      measuredObject = mObject;
      setAllChooseable(false);
      startMeasuringMode();
    }

    void resetCamera()
    {

    }

    void getAllMeasurableObjects()
    {
      MeasurableObject[] foundObjects = Object.FindObjectsOfType<MeasurableObject>();

      measurableObjects = new List<MeasurableObject>();

      foreach (MeasurableObject obj in foundObjects)
      {
        measurableObjects.Add(obj);
      }
    }
  }
  
  
  public enum MeasurementState
  {
    Off,
    ChooseObject,
    Measuring
  }
}
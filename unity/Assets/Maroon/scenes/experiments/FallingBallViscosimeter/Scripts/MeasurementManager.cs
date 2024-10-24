using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.Physics.Viscosimeter
{
    public class MeasurementManager : MonoBehaviour
    {
        public static MeasurementManager Instance;
        public GameObject caliperPrefab;
        private GameObject current_caliper = null;
        public Camera main_camera;
        public Camera zoom_camera;
        public List<MeasurableObject> measurableObjects;
        public MeasurableObject measuredObject;
        public MeasurementState measurementState = MeasurementState.Off;

        public Button startButton;
        public Button endButton;
        public GameObject uiText;
        public GameObject hintText;

        private void Awake()
        { 
            if(Instance == null)
            {
                Instance = this;
            }
            GetAllMeasurableObjects();
        }


        public void ToggleMeasuringMode(bool isMeasuring)
        {
            measurementState = isMeasuring ? MeasurementState.Measuring : MeasurementState.Off;
    
            startButton.gameObject.SetActive(!isMeasuring);
            uiText.gameObject.SetActive(!isMeasuring);
            endButton.gameObject.SetActive(isMeasuring);
    
            hintText.gameObject.SetActive(isMeasuring);
            startButton.interactable = !isMeasuring;
            endButton.interactable = isMeasuring;
    
            main_camera.gameObject.SetActive(!isMeasuring);
            main_camera.enabled = !isMeasuring;
    
            zoom_camera.gameObject.SetActive(isMeasuring);
            zoom_camera.enabled = isMeasuring;
    
            Collider measuredObjectCollider = measuredObject.gameObject.GetComponent<Collider>();
            measuredObjectCollider.enabled = !isMeasuring;

            if (isMeasuring)
            {
                if (current_caliper != null)
                {
                    Destroy(current_caliper);
                }

                FitObjectToCamera();

                zoom_camera.transform.position = new Vector3(
                    measuredObject.transform.position.x + zoom_camera.orthographicSize * 0.5f,
                    measuredObject.transform.position.y,
                    zoom_camera.transform.position.z
                );

                GameObject caliperSource = measuredObject.measurement_device != null
                    ? measuredObject.measurement_device
                    : caliperPrefab;

                current_caliper = Instantiate(caliperSource,
                    new Vector3(measuredObject.transform.position.x + 0.25f,
                        measuredObject.transform.position.y + 0.05f,
                        measuredObject.transform.position.z),
                    Quaternion.identity).gameObject;

                current_caliper.transform.rotation = Quaternion.Euler(0.0f, 0.0f, measuredObject.device_rotation);
            }
            else
            {
                if (current_caliper != null)
                {
                    Destroy(current_caliper.gameObject);
                    current_caliper = null;
                }
            }
        }

        private void FitObjectToCamera()
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

        public void ChooseMeasuredObject()
        {
            measurementState = MeasurementState.ChooseObject;
            SetAllChooseable(true);
        }


        private void SetAllChooseable(bool chooseable)
        {
            foreach (MeasurableObject mObject in measurableObjects)
            {
                mObject.SetChooseable(chooseable);
                DragDrop dragDrop = mObject.GetComponent<DragDrop>();
                if (dragDrop)
                {
                    dragDrop.dragAndDropEnabled = !chooseable;
                }

                MeasurementBox measurementBox = mObject.GetComponent<MeasurementBox>();
                if (measurementBox)
                {
                    mObject.gameObject.SetActive(chooseable);
                }
            }
            uiText.SetActive(chooseable);
        }

        public void SetChosenObject(MeasurableObject mObject)
        {
            measuredObject = mObject;
      
            SetAllChooseable(false);
            DragDrop dragDrop = mObject.GetComponent<DragDrop>();
            if (dragDrop)
            {
                dragDrop.dragAndDropEnabled = false;
            }
       
            ToggleMeasuringMode(true);
        }

        private void GetAllMeasurableObjects()
        {
            MeasurableObject[] foundObjects = FindObjectsOfType<MeasurableObject>();

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
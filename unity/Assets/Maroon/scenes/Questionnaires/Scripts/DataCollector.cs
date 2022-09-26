using System.Collections;
using UnityEngine;

namespace LimeSurveyData
{
    public abstract class DataCollector : MonoBehaviour
    {
        public enum MeasurementArea { ExitDoor, Experiment, WhiteBoard, QuestManager }
        [SerializeField] protected MeasurementArea _userPosition = MeasurementArea.ExitDoor;

        protected ExperimentMeasurements _measurements;
        protected float _startTime = 0f;
        protected float _teleportStartTime = 0f;
        protected float _measurementAreaStartTime;

        protected virtual void Start()
        {
            _startTime = _teleportStartTime = Time.time;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += SceneUnloadedHandler;


            //StartCoroutine(LoadScene());
        }

        private IEnumerator LoadScene()
        {
            yield return new WaitForSeconds(10);
            UnityEngine.SceneManagement.SceneManager.LoadScene("HuygensPrinciple.vr");
        }

        private void SceneUnloadedHandler(UnityEngine.SceneManagement.Scene scene)
        {
            _measurements.TotalTime = Time.time - _startTime;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= SceneUnloadedHandler;
        }

        public void EnterMeasurementArea(MeasurementArea measurementArea)
        {
            _measurementAreaStartTime = Time.time;
        }

        public void LeaveMeasurementArea(MeasurementArea measurementArea)
        {
            Debug.Log(Time.time);
            switch (measurementArea)
            {
                case MeasurementArea.Experiment:
                    _measurements.ExperimentTime += Time.time - _measurementAreaStartTime; 
                    break;
                case MeasurementArea.WhiteBoard: 
                    _measurements.WhiteboardTime += Time.time - _measurementAreaStartTime; 
                    break;
                case MeasurementArea.QuestManager: 
                    _measurements.QuestManagerTime += Time.time - _measurementAreaStartTime; 
                    break;
            }
        }

        public void OnSimulationStarted()
        {
            _measurements.PressedStartSimulation.Add(new TimeMeasurementBool(true));
        }
        
        public void OnSimulationStopped()
        {
            _measurements.PressedStartSimulation.Add(new TimeMeasurementBool(false));
        }
        
        public void OnSimulationReset()
        {
            _measurements.PressedResetSimulation.Add(Time.time);
        }
        
        public void OnFloorChanged(int newValue)
        {
            _measurements.FloorChanges.Add(new TimeMeasurementInt(newValue));
        }
                
        public void OnWallChanged(int newValue)
        {
            _measurements.WallChanges.Add(new TimeMeasurementInt(newValue));
        }
                
        public void OnDecoChanged(bool newValue)
        {
            _measurements.DecoChanges.Add(new TimeMeasurementBool(newValue));
        }
        
        public void OnPlantsChanged(bool newValue)
        {
            _measurements.PlantChanges.Add(new TimeMeasurementBool(newValue));
        }
        
        public void OnPicturesChanged(int newValue)
        {
            _measurements.PictureChanges.Add(new TimeMeasurementInt(newValue));
        }

        public void OnHandModelChanged(int newValue)
        {
            _measurements.HandModelChanges.Add(new TimeMeasurementInt(newValue));
        }

        public void OnHandControllerChanged(bool newValue)
        {
            _measurements.HandControllerChanges.Add(new TimeMeasurementBool(newValue));
        }

    }
}

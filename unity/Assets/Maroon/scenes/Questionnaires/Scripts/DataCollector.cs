using System;
using System.Collections;
using UnityEngine;

namespace LimeSurveyData
{
    public abstract class DataCollector : MonoBehaviour
    {
        protected enum Position { ExitDoor, Experiment, WhiteBoard, QuestManager }
        [SerializeField] protected Position _userPosition = Position.ExitDoor;

        protected ExperimentMeasurements _measurements;
        protected float _startTime = 0f;
        protected float _teleportStartTime = 0f;

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

        
        public void TeleportToExperiment()
        {
            ChangePosition(Position.Experiment);
        }
        
        public void TeleportToQuestManager()
        {
            ChangePosition(Position.QuestManager);
        }
        
        public void TeleportToWhiteboard()
        {
            ChangePosition(Position.WhiteBoard);
        }
        
        public void TeleportToExitDoor()
        {
            ChangePosition(Position.ExitDoor);
        }
        
        private void ChangePosition(Position newPosition)
        {
            if (newPosition == _userPosition) return; 
            
            var current = Time.time;
            switch (_userPosition) {
                case Position.ExitDoor: _measurements.ExitDoorTime += current - _teleportStartTime; break;
                case Position.Experiment: _measurements.ExperimentTime += current - _teleportStartTime; break;
                case Position.WhiteBoard: _measurements.WhiteboardTime += current - _teleportStartTime; break;
                case Position.QuestManager: _measurements.QuestManagerTime += current - _teleportStartTime; break;
                default: throw new ArgumentOutOfRangeException();
            }
            _teleportStartTime = current;
            _userPosition = newPosition;
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

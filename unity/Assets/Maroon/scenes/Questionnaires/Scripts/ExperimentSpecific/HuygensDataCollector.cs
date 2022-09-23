using System;
using UnityEngine;


namespace LimeSurveyData
{
    public class HuygensDataCollector : MonoBehaviour
    {
        [SerializeField] private ExperimentData _surveyDataCollector;
        
        private enum Position { ExitDoor, Experiment, WhiteBoard, QuestManager }
        
        private HuygensPrincipleMeasurements _info;
        
        private float _startTime = 0f;
        private float _teleportStartTime = 0f;
        [SerializeReference] private Position _userPosition = Position.ExitDoor;

        private bool _leftScene = false;
        
        private void Start()
        {
            _info.SceneName = Maroon.GlobalEntities.SceneManager.Instance.ActiveSceneName;
            _startTime = _teleportStartTime = Time.time;
        }

        private void OnDestroy()
        {
            LeaveScene();
        }

        public void LeaveScene()
        {
            if (_leftScene) return;

            var exitTime = Time.time;
            ChangePosition(_userPosition == Position.ExitDoor? Position.Experiment : Position.ExitDoor); //just change it to something so that the time gets updated

            _info.TotalTime = exitTime - _startTime;
            _leftScene = true;
            
            if (_surveyDataCollector != null)
                _surveyDataCollector.AddSceneInfo(_info);
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
            if (newPosition == _userPosition || _leftScene) return; 
            
            var current = Time.time;
            switch (_userPosition) {
                case Position.ExitDoor: _info.ExitDoorTime += current - _teleportStartTime; break;
                case Position.Experiment: _info.ExperimentTime += current - _teleportStartTime; break;
                case Position.WhiteBoard: _info.WhiteboardTime += current - _teleportStartTime; break;
                case Position.QuestManager: _info.QuestManagerTime += current - _teleportStartTime; break;
                default: throw new ArgumentOutOfRangeException();
            }
            _teleportStartTime = current;
            _userPosition = newPosition;
        }
        
        public void OnSimulationStarted()
        {
            _info.PressedStartSimulation.Add(new TimeMeasurementBool(true));
        }
        
        public void OnSimulationStopped()
        {
            _info.PressedStartSimulation.Add(new TimeMeasurementBool(false));
        }
        
        public void OnSimulationReset()
        {
            _info.PressedResetSimulation.Add(Time.time);
        }
        
        public void OnFloorChanged(int newValue)
        {
            _info.FloorChanges.Add(new TimeMeasurementInt(newValue));
        }
                
        public void OnWallChanged(int newValue)
        {
            _info.WallChanges.Add(new TimeMeasurementInt(newValue));
        }
                
        public void OnDecoChanged(bool newValue)
        {
            _info.DecoChanges.Add(new TimeMeasurementBool(newValue));
        }
        
        public void OnPlantsChanged(bool newValue)
        {
            _info.PlantChanges.Add(new TimeMeasurementBool(newValue));
        }
        
        public void OnPicturesChanged(int newValue)
        {
            _info.PictureChanges.Add(new TimeMeasurementInt(newValue));
        }

        public void OnHandModelChanged(int newValue)
        {
            _info.HandModelChanges.Add(new TimeMeasurementInt(newValue));
        }

        public void OnHandControllerChanged(bool newValue)
        {
            _info.HandControllerChanges.Add(new TimeMeasurementBool(newValue));
        }
        
        public void OnSlitNumberChanged(int newValue)
        {
            _info.SlitNumberChanges.Add(new TimeMeasurementInt(newValue));
        }
        
        public void OnSlitWidthChanged(float newValue)
        {
            _info.SlitWidthChanges.Add(new TimeMeasurementFloat(newValue));
        }
        
        public void OnWaveAmplitudeChanged(float newValue)
        {
            _info.WaveAmplitudeChanges.Add(new TimeMeasurementFloat(newValue));
        }       
        
        public void OnWaveLengthChanged(float newValue)
        {
            _info.WaveLengthChanges.Add(new TimeMeasurementFloat(newValue));
        }
        
        public void OnWaveFrequencyChanged(float newValue)
        {
            _info.WaveFrequencyChanges.Add(new TimeMeasurementFloat(newValue));
        }
        
        public void OnPropagationModeChanged(int newValue)
        {
            _info.PropagationModeChanges.Add(new TimeMeasurementInt(newValue));
        }
    }
}

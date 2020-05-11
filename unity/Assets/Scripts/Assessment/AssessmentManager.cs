using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Antares.Evaluation;
using Antares.Evaluation.Engine;
using Antares.Evaluation.Util;
using Maroon.Assessment.Handler;
using Maroon.Physics;

namespace Maroon.Assessment
{
    [RequireComponent(typeof(AssessmentFeedbackHandler))]
    public class AssessmentManager : MonoBehaviour
    { 
        [SerializeField]
        private string amlFile;

        private Evaluator _evalService;

        private EventBuilder _eventBuilder;

        private AssessmentFeedbackHandler _feedbackHandler;

        private readonly List<AssessmentObject> _objectsInRange = new List<AssessmentObject>();

        public bool IsConnected { get; private set; }

        private static AssessmentManager _instance;

        private EventBuilder EventBuilder => _eventBuilder ?? (_eventBuilder = EventBuilder.Event());


        public static AssessmentManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<AssessmentManager>();
                return _instance;
            }
        }

        private void Awake()
        {
            _feedbackHandler = FindObjectOfType<AssessmentFeedbackHandler>();

            IsConnected = ConnectToAssessmentSystem();
        }

        private void Start()
        {
            Debug.Log("AssessmentManager::Send Enter Event");
            
            EventBuilder.Action("enter");
        }

        private void LateUpdate()
        {
            if (_eventBuilder == null) return;

            SendGameEvent(_eventBuilder);
            _eventBuilder = null;
        }

        private bool ConnectToAssessmentSystem()
        {
            try
            {
                Debug.Log("AssessmentManager: Connecting to Assessment Service...");
                _evalService = new Evaluator();
                Debug.Log("AssessmentManager: Successfully connected to Assessment Service.");

                Debug.Log($"AssessmentManager: Loading {amlFile} into evaluation engine ...");
                _evalService.LoadAmlFile(Path.Combine(Application.streamingAssetsPath, amlFile));
                Debug.Log("AssessmentManager: Assessment model loaded.");

                _evalService.FeedbackReceived += delegate (object sender, FeedbackEventArgs args)
                {
                  _feedbackHandler.HandleFeedback(args);
                };
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"AssessmentManager: An error occurred while connecting to the Assessment service.: {ex.Message} {ex.StackTrace}");
                return false;
            }
        }

        public void RegisterAssessmentObject(AssessmentObject assessmentObject)
        {
            Debug.Log($"AssessmentManager::RegisterAssessmentObject: {assessmentObject.ObjectID}");

			_objectsInRange.Add(assessmentObject);

            EventBuilder
                .PerceiveObject(assessmentObject.ObjectID)
                .Set("class", assessmentObject.ClassType.ToString());

            foreach (var watchValue in assessmentObject.WatchValues)
                EventBuilder.Set(watchValue.PropertyName, watchValue.GetValue());
        }

        public void RegisterAssessmentObject(AssessmentObjectCompressed assessmentObject)
        {
            Debug.Log($"AssessmentManager::RegisterAssessmentObject: {assessmentObject.ObjectID}");

            EventBuilder
                .PerceiveObject(assessmentObject.ObjectID)
                .Set("class", assessmentObject.ClassType.ToString());

            foreach (var watchValue in assessmentObject.WatchedValues) {
                Debug.Log("AssessmentManager::" + assessmentObject.ObjectID + ": " + watchValue.GetName());
                EventBuilder.Set(watchValue.GetName(), watchValue.GetValue());
            }
        }

        public void DeregisterAssessmentObject(AssessmentObject assessmentObject)
        {
            Debug.Log($"AssessmentManager::DeregisterAssessmentObject: {assessmentObject.ObjectID}");

            EventBuilder.UnlearnObject(assessmentObject.ObjectID);
			
			_objectsInRange.Remove(assessmentObject);
        }
        
        public void DeregisterAssessmentObject(AssessmentObjectCompressed assessmentObject)
        {
            Debug.Log($"AssessmentManager::DeregisterAssessmentObject: {assessmentObject.ObjectID}");

            EventBuilder.UnlearnObject(assessmentObject.ObjectID);
        }

        public void SendUserAction(string actionName, string objectId=null)
        {
            Debug.Log($"AssessmentManager::SendUserAction: {objectId}.{actionName}");

            EventBuilder.Action(actionName, objectId);

            foreach (AssessmentObject assessmentObject in _objectsInRange)
            {
                EventBuilder.UpdateDataOf(assessmentObject.ObjectID);
                foreach (var watchValue in assessmentObject.gameObject.GetComponents<AssessmentWatchValue>())
                {
                    if (watchValue.IsDynamic)
                    {
                        EventBuilder.Set(watchValue.PropertyName, watchValue.GetValue());
                    }
                }
            }
        }

        public void SendDataUpdate(string objectId, string propertyName, object value)
        {
            // Debug.Log($"AssessmentManager::SendDataUpdate: {objectId}.{propertyName}={value}");

            EventBuilder.UpdateDataOf(objectId).Set(propertyName, value);
        }

        private void SendGameEvent(GameEvent gameEvent)
        {
            if (IsConnected)
            {
                _evalService.ProcessEvent(gameEvent);
            }
            else
            {
                Debug.LogWarning("AssessmentManager::SendGameEvent: Assessment service is not running");
            }
        }
    }
}

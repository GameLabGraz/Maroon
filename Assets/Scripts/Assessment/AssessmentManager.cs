using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Antares.Evaluation;
using Antares.Evaluation.Engine;
using Antares.Evaluation.Util;

namespace Maroon.Assessment
{
    [RequireComponent(typeof(AssessmentFeedbackHandler))]
    public class AssessmentManager : MonoBehaviour
    { 
        [SerializeField]
        private string amlFile;

        private Evaluator _evalService;

        private AssessmentFeedbackHandler _feedbackHandler;

        private readonly List<IAssessmentValue> _assessmentValues = new List<IAssessmentValue>();

        public bool IsConnected { get; private set; }

        private static AssessmentManager _instance;

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


        private bool ConnectToAssessmentSystem()
        {
            try
            {
                Debug.Log("AssessmentManager: Connecting to Assessment Service...");
                _evalService = new Evaluator();
                Debug.Log("AssessmentManager: Successfully connected to Assessment Service.");

                Debug.Log($"AssessmentManager: Loading {amlFile} into evaluation engine ...");
                _evalService.LoadAmlFile(Application.dataPath + amlFile);
                Debug.Log("AssessmentManager: Assessment model loaded.");

                _evalService.FeedbackReceived += delegate (object sender, FeedbackEventArgs args)
                {
                  _feedbackHandler.HandleFeedback(args);
                };
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"AssessmentManager: An error occurred while connecting to the Assessment service.: {ex.Message}");
                return false;
            }
        }

        public void RegisterAssessmentObject(AssessmentObject assessmentObject)
        {
            Debug.Log($"AssessmentManager::RegisterAssessmentObject: {assessmentObject.ObjectID}");

            var assessmentEvent = EventBuilder.Event()
                .PerceiveObject(assessmentObject.ObjectID)
                .Set("class", assessmentObject.assessmentClass.ToString());

            foreach (var watchValue in assessmentObject.WatchValues)
                assessmentEvent.Set(watchValue.PropertyName, watchValue.GetValue());

            SendGameEvent(assessmentEvent);
        }

        public void SendUserAction(string actionName, string objectId=null)
        {
            Debug.Log($"AssessmentManager::SendUserAction: {objectId}.{actionName}");

            var assessmentEvent = EventBuilder.Event().Action(actionName, objectId);
            foreach (var watchValue in GetComponents<AssessmentWatchValue>())
            {
                if (watchValue.IsDynamic)
                {
                    assessmentEvent.UnlearnObject(watchValue.ObjectID)
                        .Set(watchValue.PropertyName, watchValue.GetValue());
                }
            }
        }

        public void SendDataUpdate(string objectId, string propertyName, object value)
        {
            Debug.Log($"AssessmentManager::SendDataUpdate: {objectId}.{propertyName}={value}");

            SendGameEvent(EventBuilder.Event().UpdateDataOf(objectId).Set(propertyName, value));
        }

        private void SendGameEvent(GameEvent gameEvent)
        {
            if (IsConnected)
                _evalService.ProcessEvent(gameEvent);
            else
                Debug.LogWarning("AssessmentManager::SendGameEvent: Assessment service is not running");
        }
    }
}

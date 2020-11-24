using System;
using System.Collections.Generic;
using UnityEngine;
using Antares.Evaluation;
using Antares.Evaluation.Util;
using Maroon.Assessment.Handler;

using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Maroon.Assessment
{
    [Serializable]
    public enum AssessmentIntegrationMode
    {
        AntaresRemote,
        AntaresLocal
    }

    [RequireComponent(typeof(AssessmentFeedbackHandler))]
    public class AssessmentManager : MonoBehaviour
    {
        [SerializeField]
        private AssessmentIntegrationMode mode;

        [SerializeField]
        private string antaresUrl = null; // TODO: remove default parameter

        [SerializeField]
        private string amlFile; // depricated amlUrl

        [SerializeField] 
        private bool showDebugMessages = true;

        private AntaresClient _evalService;

        private EventBuilder _eventBuilder;

        private List<GameEvent> _eventBuffer = new List<GameEvent>();

        private AssessmentFeedbackHandler _feedbackHandler;

        private readonly Dictionary<string, AssessmentObject> _objectsInRange = new Dictionary<string, AssessmentObject>();

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

        private async void Awake()
        {
            AssessmentLogger.Enable = showDebugMessages;

            _feedbackHandler = FindObjectOfType<AssessmentFeedbackHandler>();

            await ConnectToAssessmentSystem();
        }

        private void Start()
        {
            AssessmentLogger.Log("AssessmentManager: Raising enter event ...");
            EventBuilder.Action("enter"); // TODO: this is actually a workaround - it is universal, depends on the agreed semantics and should be fired by the spawning avatar
        }

        private void Update()
        {
            _evalService?.DoWork();
        }

        private void LateUpdate()
        {
            if (_eventBuilder == null) return;

            SendGameEvent(_eventBuilder);
            _eventBuilder = null;
        }

        private string GetAntaresUrl()
        {
            return AppParams.Instance["antares_connect"] ?? antaresUrl;
        }

        private async Task ConnectToAssessmentSystem()
        {
            try
            {
                AssessmentLogger.Log("AssessmentManager: Connecting to Assessment Service...");

                var url = GetAntaresUrl();
                if(url == null)
                {
                    AssessmentLogger.Log("AssessmentManager: Missing Antares URL configuration (expecting parameter or component configuration)");
                    return;
                }

                _evalService = new AntaresClient(url);
                _evalService.FeedbackReceived += delegate (object sender, FeedbackEventArgs args)
                {
                    AssessmentLogger.Log("AssessmentManager: Feedback received");

                    // hack: this is for the experiment: will be removed once common simulation events are implemented ...
                    foreach(var feedback in args.FeedbackCommands)
                    {
                        if(feedback is DisplayTextMessage message && message.Message.Trim() == "#!GoToQuestionnaire")
                        {
                            AntaresExecuteUserAgent("GoToQuestionnaire");
                        }
                    }
                    // end hack

                    _feedbackHandler.HandleFeedback(args);
                };

                _evalService.ErrorReported += delegate (object sender, Antares.Evaluation.ErrorEventArgs args)
                {
                    Debug.LogError(
                        "Antares Server Error: " + args.Message + Environment.NewLine +
                        "--" + Environment.NewLine +
                        "Message: " + args.Message + Environment.NewLine +
                        "Internal message: " + args.InternalMessage + Environment.NewLine +
                        "Source URI: " + args.SourceUri + Environment.NewLine +
                        "Line: " + args.Line + Environment.NewLine +
                        "Column: " + args.Column);
                };

                _evalService.Connected += delegate (object sender, EventArgs args)
                {
                    AssessmentLogger.Log("AssessmentManager: Connection established");
                    IsConnected = true;
                    FlushEventBuffer();
                };

                await _evalService.ConnectAndRun();    
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"AssessmentManager: An error occurred while connecting to the Assessment service.: {ex.Message} {ex.StackTrace}");
            }
        }

        private void FlushEventBuffer()
        {
            foreach(var gameEvent in _eventBuffer)
            {
                _evalService.ProcessEvent(gameEvent);
            }
            _eventBuffer.Clear();
        }

        public AssessmentObject GetObject(string id)
        {
            return _objectsInRange[id];
        }

        public void RegisterAssessmentObject(AssessmentObject assessmentObject)
        {
            AssessmentLogger.Log($"AssessmentManager::RegisterAssessmentObject: {assessmentObject.ObjectID}");

            _objectsInRange.Add(assessmentObject.ObjectID, assessmentObject);

            EventBuilder
                .PerceiveObject(assessmentObject.ObjectID)
                .Set("class", assessmentObject.ClassType.ToString());

            foreach (var watchValue in assessmentObject.WatchedValues)
            {
                AssessmentLogger.Log(("AssessmentManager::" + assessmentObject.ObjectID + ": " + watchValue.GetName()));
                EventBuilder.Set(watchValue.GetName(), watchValue.GetValue().ToAntaresValue());
            }
        }
        
        public void DeregisterAssessmentObject(AssessmentObject assessmentObject)
        {
            AssessmentLogger.Log($"AssessmentManager::DeregisterAssessmentObject: {assessmentObject.ObjectID}");

            EventBuilder.UnlearnObject(assessmentObject.ObjectID);
            _objectsInRange.Remove(assessmentObject.ObjectID);
        }

        public void SendUserAction(string actionName, string objectId=null)
        {
            AssessmentLogger.Log($"AssessmentManager::SendUserAction: {objectId}.{actionName}");

            EventBuilder.Action(actionName, objectId);

            foreach (var assessmentObject in _objectsInRange.Values)
            {
                EventBuilder.UpdateDataOf(assessmentObject.ObjectID);
                foreach (var watchValue in assessmentObject.WatchedValues)
                {
                    if (watchValue.IsDynamic())
                    {
                        EventBuilder.Set(watchValue.GetName(), watchValue.GetValue().ToAntaresValue());
                    }
                }
            }
        }

        public void SendDataUpdate(string objectId, string propertyName, object value)
        {
            AssessmentLogger.Log($"AssessmentManager::SendDataUpdate: {objectId}.{propertyName}={value}");

            EventBuilder.UpdateDataOf(objectId).Set(propertyName, value.ToAntaresValue());
        }

        private void SendGameEvent(GameEvent gameEvent)
        {
            if (IsConnected)
            {
                _evalService.ProcessEvent(gameEvent);
            }
            else
            {
                _eventBuffer.Add(gameEvent);
                Debug.LogWarning("AssessmentManager::SendGameEvent: sending event to buffer");
            }
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void AntaresExecuteUserAgent(string command);
#else
        private static void AntaresExecuteUserAgent(string command)
        {
            Debug.Log("Requested UserAgent Command: " + command);
        }
#endif
    }
}

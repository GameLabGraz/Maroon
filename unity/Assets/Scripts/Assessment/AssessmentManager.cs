using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Antares.Evaluation;
using Antares.Evaluation.Engine;
using Antares.Evaluation.Util;
using Maroon.Assessment.Handler;
using Maroon.Physics;
using Newtonsoft.Json;

using System.Threading.Tasks;

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
        /*
        private class AntaresWebResourceProvider : IResourceProvider
        {
            public class ResourceLoader : IResourceLoader
            {
                private readonly UnityWebRequest _request;

                public ResourceLoader(string uri)
                {
                    Debug.Log("AntaresWebResourceManager: loading '" + uri + "'");
                    _request = UnityWebRequest.Get(uri);
                }

                public Stream GetInputStream()
                {
                    Debug.Log("AntaresWebResourceManager: input stream ready");
                    return new MemoryStream(_request.downloadHandler.data);
                }

                public object Load()
                {
                    Debug.Log("AntaresWebResourceManager: 'sending web request for '" + _request.url + "'");
                    return _request.SendWebRequest();
                }
            }

            public IResourceLoader CreateLoader(string uri) => new ResourceLoader(uri);
        }
        */

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

        private readonly List<AssessmentObject> _objectsInRange = new List<AssessmentObject>();

        public bool IsConnected { get; private set; }

        private static AssessmentManager _instance;

        private EventBuilder EventBuilder => _eventBuilder ?? (_eventBuilder = EventBuilder.Event());

        private static object ConvertToAntaresValue(object input)
        {
            if(input is Vector3 vector)
            {
                return new Antares.Evaluation.Vector3D(vector.x, vector.y, vector.z);
            }
            return input;
        }

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
            _feedbackHandler = FindObjectOfType<AssessmentFeedbackHandler>();

            await ConnectToAssessmentSystem();
        }

        private void Start()
        {
            Debug.Log("AssessmentManager: Raising enter event ...");
            EventBuilder.Action("enter");
        }

        private void Update()
        {
            _evalService.DoWork();
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
                if (showDebugMessages)
                    Debug.Log("AssessmentManager: Connecting to Assessment Service...");

                string url = GetAntaresUrl();
                if(url == null)
                {
                    Debug.Log("AssessmentManager: Missing Antares URL configuration (expecting parameter or component configuration)");
                    return;
                }

                _evalService = new AntaresClient(url);
                _evalService.FeedbackReceived += delegate (object sender, FeedbackEventArgs args)
                {
                    if (showDebugMessages)
                        Debug.Log("AssessmentManager: Feedback received");
                    _feedbackHandler.HandleFeedback(args);
                };

                _evalService.Connected += delegate (object sender, EventArgs args)
                {
                    if (showDebugMessages)
                        Debug.Log("AssessmentManager: Connection established");
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
            foreach(GameEvent gameEvent in _eventBuffer)
            {
                _evalService.ProcessEvent(gameEvent);
            }
            _eventBuffer.Clear();
        }
        
        public void RegisterAssessmentObject(AssessmentObject assessmentObject)
        {
            if(showDebugMessages)
                Debug.Log($"AssessmentManager::RegisterAssessmentObject: {assessmentObject.ObjectID}");

            _objectsInRange.Add(assessmentObject);
            
            EventBuilder
                .PerceiveObject(assessmentObject.ObjectID)
                .Set("class", assessmentObject.ClassType.ToString());

            foreach (var watchValue in assessmentObject.WatchedValues) {
                if(showDebugMessages)
                    Debug.Log("AssessmentManager::" + assessmentObject.ObjectID + ": " + watchValue.GetName());
                EventBuilder.Set(watchValue.GetName(), ConvertToAntaresValue(watchValue.GetValue()));
            }
        }
        
        public void DeregisterAssessmentObject(AssessmentObject assessmentObject)
        {
            if(showDebugMessages)
                Debug.Log($"AssessmentManager::DeregisterAssessmentObject: {assessmentObject.ObjectID}");

            EventBuilder.UnlearnObject(assessmentObject.ObjectID);
            _objectsInRange.Remove(assessmentObject);
        }

        public void SendUserAction(string actionName, string objectId=null)
        {
            if(showDebugMessages)
                Debug.Log($"AssessmentManager::SendUserAction: {objectId}.{actionName}");

            EventBuilder.Action(actionName, objectId);

            foreach (var assessmentObject in _objectsInRange)
            {
                EventBuilder.UpdateDataOf(assessmentObject.ObjectID);
                foreach (var watchValue in assessmentObject.WatchedValues)
                {
                    if (watchValue.IsDynamic())
                    {
                        EventBuilder.Set(watchValue.GetName(), ConvertToAntaresValue(watchValue.GetValue()));
                    }
                }
            }
        }

        public void SendDataUpdate(string objectId, string propertyName, object value)
        {
            if(showDebugMessages)
                Debug.Log($"AssessmentManager::SendDataUpdate: {objectId}.{propertyName}={value}");

            EventBuilder.UpdateDataOf(objectId).Set(propertyName, ConvertToAntaresValue(value));
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
    }
}

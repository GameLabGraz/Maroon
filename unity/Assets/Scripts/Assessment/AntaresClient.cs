using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using NativeWebSocket;

using System.Threading.Tasks;
using Antares.Evaluation;

namespace Maroon.Assessment
{

    public class AntaresClient : IEvaluationService
    {
        private WebSocket _socket = null;
        private JsonSerializer _serializer;

        public AntaresClient(string url)
        {
            Debug.Log("AntaresClient: enter ctor");
            _socket = new WebSocket(url);
            _serializer = JsonSerializer.Create(new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
            Debug.Log("AntaresClient: exit ctor");
        }

        public delegate void ConnectedEvent(object sender, EventArgs args);

        public event FeedbackEvent FeedbackReceived;

        public event ConnectedEvent Connected;

        public async Task ConnectAndRun()
        {
            _socket.OnOpen += delegate()
            {
                Connected.Invoke(this, new EventArgs());
            };
            
            _socket.OnMessage += delegate (byte[] data)
            {
                Debug.Log("AntaresClient: Incoming data: " + Encoding.UTF8.GetString(data));

                FeedbackCommand[] feedbackCommands = _serializer.Deserialize<FeedbackCommand[]>(
                    new JsonTextReader(
                        new StreamReader(
                            new MemoryStream(data))));

                Debug.Log("AntaresClient: Feedback received");
                FeedbackReceived.Invoke(this, new FeedbackEventArgs(feedbackCommands));
            };

            _socket.OnError += delegate (string errorMessage)
            {
                Debug.Log("AntaresClient:: error: " + errorMessage);
            };

            Debug.Log("AntaresClient: Connecting and running ...");
            await _socket.Connect();
        }

        public void ProcessEvent(GameEvent currentEvent)
        {
            Debug.Log("AntaresClient: Sending event through web socket ...");
            using (StringWriter result = new StringWriter())
            {
                _serializer.Serialize(result, currentEvent);
                Debug.Log("AntaresClient: outgoing message: " + result.ToString());
                _socket.Send(Encoding.UTF8.GetBytes(result.ToString()));
                Debug.Log("AntaresClient: message sent");
            }
        }

        public void DoWork()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _socket.DispatchMessageQueue();
#endif
        }

    }
}

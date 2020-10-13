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
        private readonly WebSocket _socket;
        private readonly JsonSerializer _serializer;

        public AntaresClient(string url)
        {
            AssessmentLogger.Log("AntaresClient: enter ctor");
            _socket = new WebSocket(url);
            _serializer = JsonSerializer.Create(new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
            AssessmentLogger.Log("AntaresClient: exit ctor");
        }

        public delegate void ConnectedEvent(object sender, EventArgs args);

        public event FeedbackEvent FeedbackReceived;

        public event ConnectedEvent Connected;

        public async Task ConnectAndRun()
        {
            _socket.OnOpen += delegate()
            {
                Connected?.Invoke(this, new EventArgs());
            };
            
            _socket.OnMessage += delegate (byte[] data)
            {
                AssessmentLogger.Log("AntaresClient: Incoming data: " + Encoding.UTF8.GetString(data));

                var feedbackCommands = _serializer.Deserialize<FeedbackCommand[]>(
                    new JsonTextReader(
                        new StreamReader(
                            new MemoryStream(data))));

                AssessmentLogger.Log("AntaresClient: Feedback received");
                FeedbackReceived?.Invoke(this, new FeedbackEventArgs(feedbackCommands));
            };

            _socket.OnError += delegate (string errorMessage)
            {
                AssessmentLogger.Log("AntaresClient:: error: " + errorMessage);
            };

            AssessmentLogger.Log("AntaresClient: Connecting and running ...");
            await _socket.Connect();
        }

        public void ProcessEvent(GameEvent currentEvent)
        {
            AssessmentLogger.Log("AntaresClient: Sending event through web socket ...");
            using (var result = new StringWriter())
            {
                _serializer.Serialize(result, currentEvent);
                AssessmentLogger.Log("AntaresClient: outgoing message: " + result);
                _socket.Send(Encoding.UTF8.GetBytes(result.ToString()));
                AssessmentLogger.Log("AntaresClient: message sent");
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

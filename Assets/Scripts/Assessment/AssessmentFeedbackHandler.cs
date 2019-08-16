
using Antares.Evaluation;
using Maroon.UI;
using UnityEngine;

namespace Maroon.Assessment
{
    public class AssessmentFeedbackHandler : MonoBehaviour
    {
        [SerializeField]
        private DialogueManager dialogue;

        public void HandleFeedback(FeedbackEventArgs feedbackArgs)
        {
            if (feedbackArgs == null || feedbackArgs.FeedbackCommands.Length == 0)
                return;

            foreach (var feedbackCommand in feedbackArgs.FeedbackCommands)
              HandleFeedbackCommand(feedbackCommand);
        }

        private void HandleFeedbackCommand(FeedbackCommand feedbackCommand)
        {
            switch (feedbackCommand)
            {
                case DisplayTextMessage displayTextMessage:
                  HandleDisplayTextMessage(displayTextMessage);
                  break;
                default:
                    Debug.LogWarning(
                        $"AssessmentFeedbackHandler::HandleFeedbackEntry: Unable to handle feedback command");
                    break;
            }
        }

        private void HandleDisplayTextMessage(DisplayTextMessage displayTextMessage)
        {
            var message = new Message(displayTextMessage.Message, Color.black);

            switch (displayTextMessage.Type)
            {
                case FeedbackType.Success:
                    message.Color = Color.green;
                    break;
                case FeedbackType.Hint:
                    message.Color = Color.black;
                    break;
                case FeedbackType.Warning:
                    message.Color = Color.yellow;
                    break;
                case FeedbackType.Mistake:
                    message.Color = Color.red;
                    break;
                default:
                    Debug.LogWarning($"AssessmentFeedbackHandler::HandleDisplayTextMessage: Unknown Feedback Type {displayTextMessage.Type}");
                    break;
            }
            dialogue.ShowMessage(message);

        }
    }
}

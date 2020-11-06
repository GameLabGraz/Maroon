using Antares.Evaluation;
using Maroon.UI;
using UnityEngine;

namespace Maroon.Assessment.Handler
{
    public abstract class AssessmentFeedbackHandler : MonoBehaviour
    {
        private DialogueManager dialogue;
        private AssessmentDisplay display;

        private void Awake()
        {
            dialogue = FindObjectOfType<DialogueManager>();
            display = FindObjectOfType<AssessmentDisplay>();
        }

        public void HandleFeedback(FeedbackEventArgs feedbackArgs)
        {
            if (feedbackArgs == null || feedbackArgs.FeedbackCommands.Length == 0)
                return;

            foreach (var feedbackCommand in feedbackArgs.FeedbackCommands)
              HandleFeedbackCommand(feedbackCommand);
        }

        protected void HandleFeedbackCommand(FeedbackCommand feedbackCommand)
        {
            switch (feedbackCommand)
            {
                case DisplayTextMessage displayTextMessage:
                  HandleDisplayTextMessage(displayTextMessage);
                  break;
                case DisplaySlide displaySlide:
                    HandleDisplaySlide(displaySlide);
                    break;
                case ManipulateObject manipulateObject:
                    HandleObjectManipulation(manipulateObject);
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
                    message.Color = new Color(0f, 0.6f, 0f);
                    message.Icon = MessageIcon.MI_Ok;
                    break;
                case FeedbackType.Hint:
                    message.Color = Color.black;
                    message.Icon = MessageIcon.MI_Hint;
                    break;
                case FeedbackType.Warning:
                    message.Color = new Color(0.8f, 0.5f, 0f);
                    message.Icon = MessageIcon.MI_Warning;
                    break;
                case FeedbackType.Mistake:
                    message.Color = new Color(0.8f, 0f, 0f);
                    message.Icon = MessageIcon.MI_Error;
                    break;
                default:
                    Debug.LogWarning($"AssessmentFeedbackHandler::HandleDisplayTextMessage: Unknown Feedback Type {displayTextMessage.Type}");
                    break;
            }
            dialogue.ShowMessage(message);
        }

        private void HandleObjectManipulation(ManipulateObject manipulateObject)
        {
            AssessmentObject assessmentObject;

            switch (manipulateObject.UpdateType)
            {
                case ObjectUpdateType.Create:
                    assessmentObject = HandleObjectCreation(manipulateObject);
                    HandleObjectUpdate(assessmentObject, manipulateObject);
                    break;
                
                case ObjectUpdateType.Update:
                    assessmentObject = AssessmentManager.Instance?.GetObject(manipulateObject.ID);
                    HandleObjectUpdate(assessmentObject, manipulateObject);
                    break;

                case ObjectUpdateType.Destroy:
                    assessmentObject = AssessmentManager.Instance?.GetObject(manipulateObject.ID);
                    HandleObjectDelete(assessmentObject);
                    break;
                
                default:
                    Debug.LogWarning($"AssessmentFeedbackHandler::HandleObjectManipulation: Unknown Object Update Type {manipulateObject.UpdateType}");
                    break;
            }
        }

        private void HandleDisplaySlide(DisplaySlide displaySlide)
        {
            display.LoadSlide(displaySlide.Slide);
        }

        private void HandleObjectUpdate(AssessmentObject obj, ManipulateObject manipulateObject)
        {
            if (obj == null) return;

            foreach (var dataUpdate in manipulateObject.DataUpdates)
            {
                if (dataUpdate.UpdateType != DataUpdateType.SetValue) continue;

                var watchValue = obj.GetWatchValue(dataUpdate.PropertyName);
                watchValue?.SystemSetsQuantity(dataUpdate.Value.ToUnityValue());
            }
        }

        protected virtual AssessmentObject HandleObjectCreation(ManipulateObject manipulateObject)
        {
            return null;
        }
        
        protected virtual void HandleObjectDelete(AssessmentObject obj)
        {
            Destroy(obj.gameObject);
        }
    }
}

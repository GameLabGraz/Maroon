using Antares.Evaluation.LearningContent;
using UnityEngine;

namespace Maroon.Assessment.Content
{
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class AssessmentText : AssessmentContent
    {
        public override void LoadContent(Node content)
        {
            if (!(content is Antares.Evaluation.LearningContent.Text text))
                return;

            var textComponent = GetComponent<UnityEngine.UI.Text>();
            textComponent.text = text.Content;
            textComponent.resizeTextForBestFit = true;

            switch (text.Type)
            {
                case Antares.Evaluation.LearningContent.TextType.Heading:
                    textComponent.fontSize = textComponent.resizeTextMaxSize = 48;
                    textComponent.fontStyle = FontStyle.Bold;
                    break;
                case Antares.Evaluation.LearningContent.TextType.Subheading:
                    textComponent.fontSize = textComponent.resizeTextMaxSize = 36;
                    textComponent.fontStyle = FontStyle.Bold;
                    break;
                case Antares.Evaluation.LearningContent.TextType.Text:
                    textComponent.fontSize = textComponent.resizeTextMaxSize = 28;
                    break;
                default:
                    Debug.LogWarning($"Unable to handle text type: {text.Type}");
                    break;
            }
        }
    }
}

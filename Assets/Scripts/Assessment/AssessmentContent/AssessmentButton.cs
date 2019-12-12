using Antares.Evaluation.LearningContent;
using UnityEngine;

namespace Maroon.Assessment.Content
{
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class AssessmentButton : AssessmentContent
    {
        public override void LoadContent(Node content)
        {
            if (!(content is Antares.Evaluation.LearningContent.Button button))
                return;

            var buttonComponent = GetComponent<UnityEngine.UI.Button>();
            buttonComponent.gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = button.Caption;
            buttonComponent.onClick.AddListener(() =>
            {
                AssessmentManager.Instance.SendUserAction(button.ActionName, ObjectId);
            });
        }
    }
}

using System.Linq;
using Antares.Evaluation.LearningContent;
using UnityEngine;

namespace Maroon.Assessment.Content
{
    [RequireComponent(typeof(UnityEngine.UI.InputField))]
    public class AssessmentInput : AssessmentContent
    {
        public string PropertyName { get; set; }

        public override void LoadContent(Node content)
        {
            if (!(content is Antares.Evaluation.LearningContent.Input input))
                return;

            PropertyName = input.PropertyName;

            var inputField = GetComponent<UnityEngine.UI.InputField>();
            inputField.lineType = UnityEngine.UI.InputField.LineType.MultiLineNewline;
            inputField.onValueChanged.AddListener((value) =>
            {
                if (input.IsArray)
                {
                    var valueArray = FindObjectsOfType<AssessmentInput>()
                        .Where(item => item.ObjectId == ObjectId && item.PropertyName == PropertyName)
                        .Select(item => item.GetComponent<UnityEngine.UI.InputField>().text).ToArray();

                    AssessmentManager.Instance.SendDataUpdate(ObjectId, PropertyName, valueArray);
                }
                else
                {
                    AssessmentManager.Instance.SendDataUpdate(ObjectId, PropertyName, value);
                }
            });
        }
    }
}

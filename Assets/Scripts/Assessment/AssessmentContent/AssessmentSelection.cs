using System.Linq;
using Antares.Evaluation.LearningContent;
using UnityEngine;

namespace Maroon.Assessment.Content
{
    [RequireComponent(typeof(UnityEngine.UI.Dropdown))]
    public class AssessmentSelection : AssessmentContent
    {
        public string PropertyName { get; set; }

        public override void LoadContent(Node content)
        {
            if (!(content is Antares.Evaluation.LearningContent.Selection selection))
                return;

            PropertyName = selection.PropertyName;

            var dropDownComponent = GetComponent<UnityEngine.UI.Dropdown>();
            dropDownComponent.AddOptions(
                selection.Options.Select(option => new UnityEngine.UI.Dropdown.OptionData(option.Label)).ToList());

            dropDownComponent.onValueChanged.AddListener((value) =>
            {
                if (selection.IsArray)
                {
                    var valueArray = FindObjectsOfType<AssessmentSelection>()
                        .Where(item => item.ObjectId == ObjectId && item.PropertyName == PropertyName)
                        .Select(item => item.GetComponent<UnityEngine.UI.Dropdown>().value).ToArray();

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

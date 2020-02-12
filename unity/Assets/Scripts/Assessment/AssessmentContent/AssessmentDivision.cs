using System.Collections.Generic;
using Antares.Evaluation.LearningContent;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.Assessment.Content
{
    public class AssessmentDivision : AssessmentContent
    {
        private List<AssessmentContent> _content = new List<AssessmentContent>();

        private void SetLayout(Antares.Evaluation.LearningContent.LayoutMode mode)
        {
            switch (mode)
            {
                case Antares.Evaluation.LearningContent.LayoutMode.Rows:
                    var verticalLayout = gameObject.AddComponent<VerticalLayoutGroup>();
                    verticalLayout.childControlHeight = verticalLayout.childControlWidth = true;
                    verticalLayout.padding = new RectOffset(10, 10, 10, 10);
                    verticalLayout.spacing = 10;
                    break;
                case Antares.Evaluation.LearningContent.LayoutMode.Columns:
                    var horizontalLayout = gameObject.AddComponent<HorizontalLayoutGroup>();
                    horizontalLayout.childControlHeight = horizontalLayout.childControlWidth = true;
                    horizontalLayout.spacing = 10;
                    break;
                case Antares.Evaluation.LearningContent.LayoutMode.Coordinates:
                default:
                    Debug.LogWarning($"Unable to handle layout mode: {mode}");
                    break;
            }
        }

        public override void LoadContent(Node content)
        {
            if (!(content is Antares.Evaluation.LearningContent.Division div))
                return;

            SetLayout(div.Layout);

            foreach (var item in div.Items)
            {
                AssessmentContent assessmentContent = null;
                switch (item)
                {
                    case Antares.Evaluation.LearningContent.Division division:
                        var panelObject = Object.Instantiate(Resources.Load(PanelPrefab), transform, false) as GameObject;
                        assessmentContent = panelObject?.AddComponent<AssessmentDivision>();
                        break;
                    case Antares.Evaluation.LearningContent.Table table:
                        var tableObject = Object.Instantiate(Resources.Load(TablePrefab), transform, false) as GameObject;
                        assessmentContent = tableObject?.AddComponent<AssessmentTable>();
                        break;
                    case Antares.Evaluation.LearningContent.Text text:
                        var textObject = Object.Instantiate(Resources.Load(TextPrefab), transform, false) as GameObject;
                        assessmentContent = textObject?.AddComponent<AssessmentText>();
                        break;
                    case Antares.Evaluation.LearningContent.Selection selection:
                        var dropDownObject = Object.Instantiate(Resources.Load(DropDownPrefab), transform, false) as GameObject;
                        assessmentContent = dropDownObject?.AddComponent<AssessmentSelection>();
                        break;
                    case Antares.Evaluation.LearningContent.Input input:
                        var inputObject = Object.Instantiate(Resources.Load(InputPrefab), transform, false) as GameObject;
                        assessmentContent = inputObject?.AddComponent<AssessmentInput>();
                        break;
                    case Antares.Evaluation.LearningContent.Button button:
                        var buttonObject = Object.Instantiate(Resources.Load(ButtonPrefab), transform, false) as GameObject;
                        assessmentContent = buttonObject?.AddComponent<AssessmentButton>();
                        break;
                    case Antares.Evaluation.LearningContent.Image image:
                        var imageObject = Object.Instantiate(Resources.Load(ImagePrefab), transform, false) as GameObject;
                        assessmentContent = imageObject?.AddComponent<AssessmentImage>();
                        break;
                }

                if (assessmentContent == null) continue;

                assessmentContent.ObjectId = ObjectId;
                assessmentContent.LoadContent(item);
                _content.Add(assessmentContent);
            }
        }
    }
}

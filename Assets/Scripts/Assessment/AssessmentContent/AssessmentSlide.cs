using Antares.Evaluation.LearningContent;

namespace Maroon.Assessment.Content
{
    public class AssessmentSlide : AssessmentObject
    {
        protected override void Start()
        {
            classType = AssessmentClass.Slide;
        }

        public void LoadContent(Slide content)
        {
            ObjectID = content.ID.ToString();
            AssessmentManager.Instance.RegisterAssessmentObject(this);

            var divisionObject = gameObject.AddComponent<AssessmentDivision>();
            divisionObject.ObjectId = ObjectID;
            divisionObject.LoadContent(content);
        }
    }
}
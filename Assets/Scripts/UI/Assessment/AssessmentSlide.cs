using UnityEngine;
using Antares.Evaluation.LearningContent;
using Antares.Evaluation.Util;

namespace Maroon.UI.Assessment
{
  public class AssessmentSlide : MonoBehaviour
  {
    private void Start()
    {
      LoadSlide(TestSlides.Test());
    }

    public void LoadSlide(Slide slide)
    {
      slide.LoadUIElement(transform);
      Canvas.ForceUpdateCanvases();
    }
  }
}


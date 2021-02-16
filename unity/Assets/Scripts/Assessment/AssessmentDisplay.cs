using System;
using System.Threading;
using Antares.Evaluation.LearningContent;
using Maroon.Assessment.Content;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Maroon.Assessment
{
    public class AssessmentDisplay : AssessmentObject
    {
        private SynchronizationContext syncContext;

        protected override void Awake()
        {
            base.Awake();
            syncContext = SynchronizationContext.Current;
        }

        private void Start()
        {
            if (AssessmentManager.Instance == null || !AssessmentManager.Instance.enabled)
                gameObject.SetActive(false);
        }

        public void LoadSlide(Slide slide)
        {
            syncContext.Send(
                d =>
                {
                    Clear();

                    var panelObject = Object.Instantiate(Resources.Load(AssessmentContent.PanelPrefab), transform, false) as GameObject;
                    if (panelObject == null) return;

                    var slideObject = panelObject.AddComponent<AssessmentSlide>();
                    slideObject?.LoadContent(slide);

                    Canvas.ForceUpdateCanvases();
                }, null);
        }

        public void Clear()
        {
            for (var i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
        }
    }
}

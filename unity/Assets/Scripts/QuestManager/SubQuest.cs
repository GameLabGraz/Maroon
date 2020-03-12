using UnityEngine;

namespace QuestManager
{
    public class SubQuest : IQuest
    {
        [SerializeField] private GameObject finishLine;

        [SerializeField] private GameObject infoLogoObject;

        private bool hasAdditionalInformation;

        public delegate bool QuestCheck();
        public QuestCheck questCheck;

        private void Start()
        {
            OnQuestFinished.AddListener(() =>
            {
                if (finishLine != null)
                {
                    finishLine.SetActive(true);
                    finishLine.GetComponent<Renderer>().enabled = !IsHidden;
                }
            });
        }

        public bool HasAdditionalInformation
        {
            get => hasAdditionalInformation;
            set
            {
                hasAdditionalInformation = value;
                if (infoLogoObject != null) infoLogoObject.SetActive(true);
            }
        }

        protected override bool IsDone()
        {
            return questCheck != null && questCheck();
        }
    }
}
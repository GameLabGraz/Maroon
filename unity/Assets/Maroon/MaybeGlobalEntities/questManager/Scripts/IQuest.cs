using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QuestManager
{
    public abstract class IQuest : MonoBehaviour
    {
        [Header("Status Lamp")]
        [SerializeField] private GameObject ActiveLamp;
        [SerializeField] private GameObject DeactiveLamp;

        public bool IsFinished { get; set; }
        public UnityEvent OnQuestFinished = new UnityEvent();

        protected List<IQuest> SubQuests = new List<IQuest>();

        public string Text { get; set; }

        public GameObject QuestHint { get; set; }
        public GameObject QuestAchievement { get; set; }

        public bool IsHidden { get; set; }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            { 
                _isActive = value;
                QuestHint?.SetActive(value);

                if (DeactiveLamp != null)
                {
                    DeactiveLamp.SetActive(!value);
                    DeactiveLamp.GetComponent<Renderer>().enabled = !IsHidden;
                }

                if (ActiveLamp != null)
                {
                    ActiveLamp.SetActive(value);
                    ActiveLamp.GetComponent<Renderer>().enabled = !IsHidden;
                }
            }
        }
        protected abstract bool IsDone();

        protected virtual void OnEnable()
        {
            IsActive = _isActive;
        }

        protected virtual void Update()
        {
            if (!IsActive)
                return;

            if (IsDone())
            {
                IsFinished = true;
                IsActive = false;

                QuestAchievement?.SetActive(true);
                OnQuestFinished.Invoke();
            }
        }

        public void AddSubQuest(IQuest subQuest)
        {
            SubQuests.Add(subQuest);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class ReadInstructionCheck : IQuestCheck
    {
        private WhiteboardController _whiteboardController;
        private List<Lecture> _lectures;

        protected override void InitCheck()
        {
            _whiteboardController = FindObjectOfType<WhiteboardController>();
        }

        protected override bool CheckCompliance()
        {
            return _whiteboardController.CurrentContentIndex == _whiteboardController.SelectedLecture.Contents.Count-1;
        }
    }
}

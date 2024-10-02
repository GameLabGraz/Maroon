using System.Collections.Generic;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class ReadInstructionCheck : QuestCheck
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

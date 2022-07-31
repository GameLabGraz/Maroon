using System;
using QuestManager;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MoveVrControlsPanel : IQuestCheck
    {
        private CatalystVrControlPanel _catalystVrControlPanel;
        private Vector3 _initialPosition;
        protected override void InitCheck()
        {
            _catalystVrControlPanel = FindObjectOfType<CatalystVrControlPanel>();
            if (!_catalystVrControlPanel)
                throw new NullReferenceException("no vr controls panel in scene!");
            Vector3 pos = _catalystVrControlPanel.gameObject.transform.position;
            _initialPosition = new Vector3(pos.x, pos.y, pos.z);
        }

        protected override bool CheckCompliance()
        {
            // todo deal with different setup for study!
            return Vector3.Distance(_initialPosition, _catalystVrControlPanel.transform.position) > 0.5f;
        }

    }
}
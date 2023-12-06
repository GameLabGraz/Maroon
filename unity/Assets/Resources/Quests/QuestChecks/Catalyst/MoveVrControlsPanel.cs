using System;
using UnityEngine;
using GameLabGraz.QuestManager;

#if !UNITY_WEBGL
using Maroon.Chemistry.Catalyst.VR;
#endif

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class MoveVrControlsPanel : QuestCheck
    {
#if !UNITY_WEBGL
        private CatalystVrControlPanel _catalystVrControlPanel;
        private Vector3 _initialPosition;
#endif
        protected override void InitCheck()
        {
#if !UNITY_WEBGL
            _catalystVrControlPanel = FindObjectOfType<CatalystVrControlPanel>();
            if (!_catalystVrControlPanel)
                throw new NullReferenceException("no vr controls panel in scene!");
            Vector3 pos = _catalystVrControlPanel.gameObject.transform.position;
            _initialPosition = new Vector3(pos.x, pos.y, pos.z);
#endif
        }

        protected override bool CheckCompliance()
        {
#if !UNITY_WEBGL
            // todo deal with different setup for study!
            return Vector3.Distance(_initialPosition, _catalystVrControlPanel.transform.position) > 0.5f;
#else
            return false;
#endif
        }
    }
}
using System;
using UnityEngine;
using GameLabGraz.QuestManager;

#if !UNITY_WEBGL
using GameLabGraz.VRInteraction;
#endif

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class MoveToOtherLocation : QuestCheck
    {
#if !UNITY_WEBGL
        private VRPlayer _vrPlayer;
        private GameObject _boxSpawnTransform;
        private GameObject _labSpawnTransform;
#endif
        protected override void InitCheck()
        {
#if !UNITY_WEBGL
            _vrPlayer = FindObjectOfType<VRPlayer>();
            if (!_vrPlayer)
                throw new NullReferenceException("no vr player in scene!");
            _boxSpawnTransform = GameObject.Find("PlayerSpawnCatalystBox");
            _labSpawnTransform = GameObject.Find("PlayerSpawnLab");
            if (!_boxSpawnTransform)
                throw new NullReferenceException("catalyst box player spawn missing!");
            if (!_labSpawnTransform)
                throw new NullReferenceException("lab player spawn missing!");
#endif
        }

        protected override bool CheckCompliance()
        {
#if !UNITY_WEBGL
            // todo deal with different setup for study!
            return Vector3.Distance(_boxSpawnTransform.transform.position, _vrPlayer.transform.position) > 2f;
#else
            return false;
#endif
        }

    }
}
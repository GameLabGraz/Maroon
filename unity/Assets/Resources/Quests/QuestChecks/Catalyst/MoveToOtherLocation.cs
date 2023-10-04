using System;
using GameLabGraz.VRInteraction;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class MoveToOtherLocation : QuestCheck
    {
        private VRPlayer _vrPlayer;
        private GameObject _boxSpawnTransform;
        private GameObject _labSpawnTransform;
        protected override void InitCheck()
        {
            _vrPlayer = FindObjectOfType<VRPlayer>();
            if (!_vrPlayer)
                throw new NullReferenceException("no vr player in scene!");
            _boxSpawnTransform = GameObject.Find("PlayerSpawnCatalystBox");
            _labSpawnTransform = GameObject.Find("PlayerSpawnLab");
            if (!_boxSpawnTransform)
                throw new NullReferenceException("catalyst box player spawn missing!");
            if (!_labSpawnTransform)
                throw new NullReferenceException("lab player spawn missing!");
        }

        protected override bool CheckCompliance()
        {
            // todo deal with different setup for study!
            return Vector3.Distance(_boxSpawnTransform.transform.position, _vrPlayer.transform.position) > 2f;
        }

    }
}
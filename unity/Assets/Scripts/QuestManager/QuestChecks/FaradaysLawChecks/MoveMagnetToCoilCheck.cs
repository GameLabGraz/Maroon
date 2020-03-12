using System;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MoveMagnetToCoilCheck : IQuestCheck
    {
        private Magnet _magnet;
        private Coil _coil;

        protected override void InitCheck()
        {
            _magnet = FindObjectOfType<Magnet>();
            _coil = FindObjectOfType<Coil>();

            if (_magnet == null) throw new NullReferenceException("There is no magnet in the scene.");
            if (_coil == null) throw new NullReferenceException("There is no coil in the scene.");
        }

        protected override bool CheckCompliance()
        {
            return Vector3.Distance(_magnet.transform.position, _coil.transform.position) < 3f;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    // TODO How to get VRInteractable in here D:
    // Why is there no Maroon.Experiments.SemicunductorExpermiment.csproj 
    [RequireComponent(typeof(Quest))]
    public class PickUpToFCameraCheck : QuestCheck
    {
        private GameObject _toFCamera;
        private Vector3 _toFCameraStartPosition;
        
        protected override void InitCheck()
        {
            _toFCamera = GameObject.FindGameObjectWithTag("ToFCamera");
            _toFCameraStartPosition = _toFCamera.transform.position;
            if (_toFCamera == null) throw new NullReferenceException("There is no ToFCamera with VRInteractable in the scene.");
        }

        protected override bool CheckCompliance()
        {
            return _toFCameraStartPosition != _toFCamera.transform.position;
        }
    }
}
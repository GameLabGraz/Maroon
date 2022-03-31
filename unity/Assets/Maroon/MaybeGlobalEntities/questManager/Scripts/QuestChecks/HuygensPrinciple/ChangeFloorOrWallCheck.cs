using System;
using System.Collections;
using System.Collections.Generic;
using QuestManager;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class ChangeFloorOrWallCheck : IQuestCheck
    {
        private MeshRenderer _floorMeshRenderer;
        private MeshRenderer _wallMeshRenderer;

        private Material _initFloor;
        private Material _initWall;

        private bool _wasChanged = false;

        protected override void InitCheck()
        {
            var objFloors = GameObject.FindGameObjectsWithTag("Floor");
            if(objFloors.Length != 1) throw new NullReferenceException("There is no floor in the scene.");

            _floorMeshRenderer = objFloors[0].GetComponent<MeshRenderer>();
            if(!_floorMeshRenderer) throw new NullReferenceException("There is no floor in the scene.");

            var objWalls = GameObject.FindGameObjectsWithTag("Wall");
            if(objWalls.Length == 0) throw new NullReferenceException("There are no walls in the scene.");

            _wallMeshRenderer = objWalls[0].GetComponent<MeshRenderer>();
            if(!_wallMeshRenderer) throw new NullReferenceException("There are no walls in the scene.");

            _initFloor = _floorMeshRenderer.material;
            _initWall = _wallMeshRenderer.material;
        }

        protected override bool CheckCompliance()
        {
            _wasChanged = _wasChanged || _initFloor != _floorMeshRenderer.material || _initWall != _wallMeshRenderer.material;
            return _wasChanged;
        }
        
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class ChangeDecoCheck : IQuestCheck
    {
        class CheckObj
        {
            public GameObject _object;
            public bool _initState;

            public CheckObj(GameObject obj)
            {
                _object = obj;
                _initState = obj.activeInHierarchy;
            }
            
            public bool HasChanged()
            {
                return _initState != _object.activeInHierarchy;
            }
        }

        private List<CheckObj> _decoObjects = new List<CheckObj>();
        private bool _wasChanged = false;
        
        protected override void InitCheck()
        {
            var decoCollection = FindObjectOfType<DecoObjectCollection>();
            if(decoCollection == null) throw new NullReferenceException("Deco Collection not found.");

            var objs = decoCollection._decoObjects;

            if(objs.Count == 0) throw new NullReferenceException("No deco objects have been found.");

            foreach (var obj in objs)
            {
                _decoObjects.Add(new CheckObj(obj));
            }
        }
        
        protected override bool CheckCompliance()
        {
            if (_wasChanged)
                return true;

            foreach (var obj in _decoObjects)
            {
                if (obj.HasChanged())
                {
                    _wasChanged = true;
                    break;
                }
            }

            return _wasChanged;
        }
    }
}

using System;
using UnityEngine;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public class BattleModeController : MonoBehaviour
    {
        [Header("ConvexHulls")]
        [SerializeField] private ConvexHull3D leftHull;
        [SerializeField] private ConvexHull3D rightHull;

        public ConvexHull3D LeftHull           => leftHull;
        public Transform    LeftHullTransform  => leftHull.transform;
        public GameObject   LeftHullObject     => leftHull.gameObject;

        public ConvexHull3D RightHull          => rightHull;
        public Transform    RightHullTransform => rightHull.transform;
        public GameObject   RightHullObject    => rightHull.gameObject;

        private bool _leftFinished;
        private bool _rightFinished;
        
        public event Action OnRaceComplete = delegate { };

        //----------------------------------------------------------------------------------

        private void Start()
        {
            leftHull.OnHullComplete  += OnLeftHullComplete;
            rightHull.OnHullComplete += OnRightHullComplete;
        }

        private void OnDestroy()
        {
            leftHull.OnHullComplete  -= OnLeftHullComplete;
            rightHull.OnHullComplete -= OnRightHullComplete;
        }

        //----------------------------------------------------------------------------------

        public void Play()
        {
            leftHull.Play();
            rightHull.Play();
        }

        public void Pause()
        {
            leftHull.Pause();
            rightHull.Pause();
        }

        public void Reset()
        {
            _leftFinished = false;
            _rightFinished = false;
            leftHull.GeneratePoints();
            rightHull.GenerateFromPoints(leftHull.Points);
        }

        public void Enter() => Reset();

        public void SetSpeed(float value)
        {
            leftHull.AnimationSpeed = value;
            rightHull.AnimationSpeed = value;
        }

        private void OnLeftHullComplete()
        {
            _leftFinished = true;
            CheckRaceComplete();
        }

        private void OnRightHullComplete()
        {
            _rightFinished = true;
            CheckRaceComplete();
        }

        private void CheckRaceComplete()
        {
            if(_leftFinished && _rightFinished)
            {
                OnRaceComplete.Invoke();
            }
        }
    }
}
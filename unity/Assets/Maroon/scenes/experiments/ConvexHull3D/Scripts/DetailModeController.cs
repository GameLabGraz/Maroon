using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public class DetailModeController : MonoBehaviour
    {
        [Header("ConvexHull")]
        [SerializeField] private ConvexHull3D centerHull;

        public ConvexHull3D CenterHull              => centerHull;
        public Transform CenterHullTransform        => centerHull.transform;
        public GameObject CenterHullObject          => centerHull.gameObject;

        public event Action OnDetailComplete = delegate { };

        //----------------------------------------------------------------------------------

        private void Start()
        {
            centerHull.OnHullComplete += OnCenterHullComplete;
        }

        private void OnDestroy()
        {
            centerHull.OnHullComplete -= OnCenterHullComplete;
        }

        //----------------------------------------------------------------------------------

        public void Play()                  => centerHull.Play();
        public void Pause()                 => centerHull.Pause();
        public void Reset()                 => centerHull.ClearHull();
        public void NextStep()              => centerHull.NextStep();
        public void PreviousStep()          => centerHull.PreviousStep();

        public void Enter()                 => centerHull.GeneratePoints();

        public void SetPointCount(float value)
        {
            centerHull.NumberOfPoints = (int)value;
        }

        public void SetSpeed(float value)
        {
            centerHull.AnimationSpeed = value;
        }

        private void OnCenterHullComplete() => OnDetailComplete.Invoke();
    }
}
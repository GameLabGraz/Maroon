using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public class ConvexHull3D : MonoBehaviour
    {
        [Header("Visualization Objects")]
        [SerializeField] private HullPointManager         _pointManager;
        [SerializeField] private HullVisualizationManager _visualizer;

        [Header("Visualization Settings")]
        [SerializeField] private float animationSpeed = 1f;
        [SerializeField] private bool showSearchLines = true;

        private IHullAlgorithm[] _algorithms;
        private int _selectedAlgorithm = 0;


        private class HullSnapshot
        {
            public int PseudocodeLine;
            public List<List<int>> HullFaces;
            public List<Color> FaceColors;
            public Color[] PointColors;
            public (Vector3 from, Vector3 to, Color color)[] HighlightLines;
            public (Vector3 from, Vector3 to, Color color)? SearchLine;
        }

        private List<HullSnapshot> _snapshots = new List<HullSnapshot>();
        
        private int _currentStep = -1;
        private int _currentPseudoLine;

        public event Action<int> OnStep         = delegate { };
        public event Action OnSimulationReset   = delegate { };
        public event Action OnHullComplete      = delegate { };

        private bool _isPlaying;
        private float _nextStepTime;

        //----------------------------------------------------------------------------------

        private void Awake()
        {
            _algorithms = new IHullAlgorithm[]
            {
                new GiftWrappingAlgorithm(),
                new IncrementalHullAlgorithm(),
                new QuickHullAlgorithm(),
            };
        }

        private void Update()
        {
            if(!_isPlaying || Time.time < _nextStepTime) return;

            _nextStepTime = Time.time + 0.5f / animationSpeed;

            if(_currentStep < _snapshots.Count - 1)
            {
                LoadSnapshot(++_currentStep);
            }
            else
            {
                _isPlaying = false;
                OnHullComplete.Invoke();
            }
        }
        
        //----------------------------------------------------------------------------------

        public float AnimationSpeed
        {
            get { return animationSpeed; }
            set { animationSpeed = value; }
        }

        public bool ShowSearchLines
        {
            get { return showSearchLines; }
            set { showSearchLines = value; }
        }

        public bool ShowFaces
        {
            get { return _visualizer.ShowFaces; }
            set { _visualizer.ToggleFaces(value); }
        }

        public int NumberOfPoints
        {
            get { return _pointManager.NumberOfPoints; }
            set { _pointManager.NumberOfPoints = value; }
        }


        public IHullAlgorithm CurrentAlgorithm         => _algorithms[_selectedAlgorithm];
        public IHullAlgorithm[] Algorithms             => _algorithms;
        public int SelectedAlgorithmIndex              => _selectedAlgorithm;
        public List<Vector3> Points                    => _pointManager.Points;

        private void SetAlgorithm(int index)
        {
            _selectedAlgorithm = index;
        }

        public void SetAlgorithmFromDropdown(int index)
        {
            SetAlgorithm(index);
            ClearHull();
        }

        public void GeneratePoints()
        {
            ClearHullAndPoints();
            _pointManager.GenerateRandomPoints();
        }

        public void GenerateFromPoints(List<Vector3> sourcePoints)
        {
            ClearHullAndPoints();
            _pointManager.GenerateFromPoints(sourcePoints);
        }

        private void ClearHullAndPoints()
        {
            ClearHull();
            _pointManager.ClearPoints();
        }

        public int StepCount => _currentStep + 1;
        public int HullFaceCount => _visualizer.HullFaceCount;

        public int CurrentPseudoLine
        {
            get { return _currentPseudoLine; }
            set { _currentPseudoLine = value; }
        }

        private void GenerateHullSnapshots()
        {
            if(_snapshots.Count > 0) return;

            ResetVisualization();
            OnSimulationReset.Invoke();

            _algorithms[_selectedAlgorithm].Run(this);

            ResetVisualization();
            _currentStep = -1;
        }

        private void LoadSnapshot(int index)
        {
            var currentSnapShot = _snapshots[index];

            _visualizer.ClearHullVisualization();

            for(int i = 0; i < currentSnapShot.HullFaces.Count; i++)
            {
                var face = currentSnapShot.HullFaces[i];
                _visualizer.AddHullFace(face);
                _visualizer.DrawHullFace(face);
                _visualizer.SetFaceColor(face[0], face[1], face[2], currentSnapShot.FaceColors[i]);
            }

            _pointManager.SetPointColors(currentSnapShot.PointColors);

            foreach(var (from, to, color) in currentSnapShot.HighlightLines)
            {
                _visualizer.AddHighlightLine(from, to, color);
            }

            if(showSearchLines && currentSnapShot.SearchLine.HasValue)
            {
                var searchLine = currentSnapShot.SearchLine.Value;
                _visualizer.UpdateSearchLine(searchLine.from, searchLine.to, searchLine.color);
            }

            CurrentPseudoLine = currentSnapShot.PseudocodeLine;

            OnStep.Invoke(currentSnapShot.PseudocodeLine);
        }

        public void CaptureStep(int lineNumber)
        {
            var faces = new List<List<int>>();
            var faceColors = new List<Color>();

            foreach(var face in _visualizer.HullFaceList)
            {
                faces.Add(new List<int>(face));
                faceColors.Add(_visualizer.GetFaceColor(face[0], face[1], face[2]));
            }

            _snapshots.Add(new HullSnapshot
            {
                PseudocodeLine = lineNumber,
                HullFaces = faces,
                FaceColors = faceColors,

                PointColors = _pointManager.GetPointColors(),

                HighlightLines = _visualizer.GetHighlightLinesData(),
                SearchLine = _visualizer.GetSearchLineData(),
            });
        }

        public void ClearHull()
        {
            _isPlaying = false;
            _snapshots.Clear();
            _currentStep = -1;
            CurrentPseudoLine = 0;
            ResetVisualization();

            OnSimulationReset.Invoke();
        }

        public void Play()
        {
            GenerateHullSnapshots();

            _isPlaying = true;
            _nextStepTime = Time.time + 0.5f / animationSpeed;
        }

        public void Pause()
        {
            _isPlaying = false;
        }

        public void NextStep()
        {
            Pause();

            GenerateHullSnapshots();

            if(_currentStep < _snapshots.Count - 1) 
            {
                LoadSnapshot(++_currentStep);
            }  
        }

        public void PreviousStep()
        {
            Pause();

            GenerateHullSnapshots();

            if(_currentStep > 0)
            {
                LoadSnapshot(--_currentStep);
            }
        }

        private void ResetVisualization()
        {
            _pointManager.ResetAllPointColors();
            _visualizer.ClearHullVisualization();
        }

        public Color NormalPointColor   => _pointManager.NormalPointColor;
        public Color HullPointColor     => _pointManager.HullPointColor;
        public Color CurrentPointColor  => _pointManager.CurrentPointColor;
        public Color VisibleFaceColor   => _visualizer.VisibleFaceColor;
        public Color HorizonEdgeColor   => _visualizer.HorizonEdgeColor;
        public Color NewFaceColor       => _visualizer.NewFaceColor;
        public Color ActiveEdgeColor    => _visualizer.ActiveEdgeColor;
        public Color AssignedPointColor => _visualizer.AssignedPointColor;


        public void ResetAllPointColors()                                           => _pointManager.ResetAllPointColors();
        public void SetPointColor(int index, Color color)                           => _pointManager.SetPointColor(index, color);

        public void AddHullFace(List<int> face)                                     => _visualizer.AddHullFace(face);
        public void RemoveHullFace(List<int> face)                                  => _visualizer.RemoveHullFace(face);
        public void DrawHullFace(List<int> face)                                    => _visualizer.DrawHullFace(face);

        public void HighlightHullFaces(List<List<int>> faces, Color color)          => _visualizer.HighlightHullFaces(faces, color);
        public void ResetHullFaceColors()                                           => _visualizer.ResetHullFaceColors();

        public void UpdateSearchLine(Vector3 from, Vector3 to)                      => _visualizer.UpdateSearchLine(from, to, _pointManager.CurrentPointColor);
        public void ClearSearchLine()                                               => _visualizer.ClearSearchLine();

        public void AddHighlightLine(Vector3 from, Vector3 to, Color color)         => _visualizer.AddHighlightLine(from, to, color);
        public void ClearHighlightLines()                                           => _visualizer.ClearHighlightLines();
    }
}
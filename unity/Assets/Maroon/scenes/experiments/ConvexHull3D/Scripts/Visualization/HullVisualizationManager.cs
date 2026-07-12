using System.Collections.Generic;
using UnityEngine;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public class HullVisualizationManager : MonoBehaviour
    {
        [Header("Visualization Objects")]
        [SerializeField] private HullPointManager _pointManager;

        [Header("Visualization")]
        [SerializeField] private Color lineColor;
        [SerializeField] private float lineWidth = 0.05f;
        [SerializeField] private Color faceColor;
        [SerializeField] private Color visibleFaceColor;
        [SerializeField] private Color horizonEdgeColor;
        [SerializeField] private Color newFaceColor;
        [SerializeField] private Color activeEdgeColor;
        [SerializeField] private Color assignedPointColor;

        [Header("Materials")]
        [SerializeField] private Material faceMaterial;


        private List<List<int>> _hullFaces = new List<List<int>>();
        private Dictionary<(int, int, int), GameObject> _hullMeshMap = new Dictionary<(int, int, int), GameObject>();
        private Dictionary<(int, int), GameObject> _hullEdgeMap = new Dictionary<(int, int), GameObject>();


        private GameObject       _currentSearchLine;
        private List<GameObject> _tempHighlightLines = new List<GameObject>();

        private bool _showFaces = true;

        private (int, int) EdgeKey(int a, int b) => a < b ? (a, b) : (b, a);

        //-------------------------------------------------------------------------

        public Color VisibleFaceColor                   => visibleFaceColor;
        public Color HorizonEdgeColor                   => horizonEdgeColor;
        public Color NewFaceColor                       => newFaceColor;
        public Color ActiveEdgeColor                    => activeEdgeColor;
        public Color AssignedPointColor                 => assignedPointColor;

        public int HullFaceCount                        => _hullFaces.Count;
        public bool ShowFaces                           => _showFaces;

        public IReadOnlyList<List<int>> HullFaceList    => _hullFaces;

        //-------------------------------------------------------------------------

        public void AddHullFace(List<int> face)
        {
            _hullFaces.Add(face);
        }

        public void DrawHullFace(List<int> face)
        {
            CreateHullEdge(face[0], face[1]);
            CreateHullEdge(face[1], face[2]);
            CreateHullEdge(face[2], face[0]);
            CreateFaceMesh(face[0], face[1], face[2]);
        }

        public void RemoveHullFace(List<int> face)
        {
            int a = face[0];
            int b = face[1];
            int c = face[2];

            var faceKey = (a, b, c);

            if(_hullMeshMap.TryGetValue(faceKey, out GameObject meshObj))
            {
                Destroy(meshObj);
                _hullMeshMap.Remove(faceKey);
            }

            for(int i = 0; i < _hullFaces.Count; i++)
            {
                var hullface = _hullFaces[i];
                if(hullface[0] == a && hullface[1] == b && hullface[2] == c)
                {
                    _hullFaces.RemoveAt(i);
                    break;
                }
            }

            var remainingEdges = new HashSet<(int, int)>();
            foreach(var hullface in _hullFaces)
            {
                remainingEdges.Add(EdgeKey(hullface[0], hullface[1]));
                remainingEdges.Add(EdgeKey(hullface[1], hullface[2]));
                remainingEdges.Add(EdgeKey(hullface[2], hullface[0]));
            }

            foreach(var (x, y) in new[] { (a, b), (b, c), (c, a) })
            {
                var edgeKey = EdgeKey(x, y);
                if(!remainingEdges.Contains(edgeKey) && _hullEdgeMap.TryGetValue(edgeKey, out GameObject lineObj))
                {
                    Destroy(lineObj);
                    _hullEdgeMap.Remove(edgeKey);
                }
            }
        }

        public void ClearHullVisualization()
        {
            _hullFaces.Clear();

            foreach(var edge in _hullEdgeMap) 
            {
                Destroy(edge.Value);
            }

            _hullEdgeMap.Clear();

            foreach(var mesh in _hullMeshMap)
            {
                Destroy(mesh.Value);
            }

            _hullMeshMap.Clear();
            
            ClearSearchLine();
            ClearHighlightLines();
        }

        //-------------------------------------------------------------------------

        public void ToggleFaces(bool value)
        {
            _showFaces = value;
            foreach(var mesh in _hullMeshMap)
            {
                if(mesh.Value != null) 
                {
                    mesh.Value.SetActive(_showFaces);
                }
            }
        }

        public void HighlightHullFaces(List<List<int>> faceVertices, Color color)
        {
            foreach(var vertex in faceVertices)
            {
                var key = (vertex[0], vertex[1], vertex[2]);

                if(_hullMeshMap.TryGetValue(key, out GameObject meshObj) && meshObj != null)
                {
                    meshObj.GetComponent<MeshRenderer>().material.color = color;
                }
            }
        }

        public void ResetHullFaceColors()
        {
            foreach(var mesh in _hullMeshMap)
            {
                if(mesh.Value != null)
                {
                    mesh.Value.GetComponent<MeshRenderer>().material.color = faceColor;
                }
            }
        }

        public Color GetFaceColor(int a, int b, int c)
        {
            var key = (a, b, c);
            if(_hullMeshMap.TryGetValue(key, out GameObject meshObj) && meshObj != null)
            {
                return meshObj.GetComponent<MeshRenderer>().material.color;
            }
            return faceColor;
        }

        public void SetFaceColor(int a, int b, int c, Color color)
        {
            var key = (a, b, c);
            if(_hullMeshMap.TryGetValue(key, out GameObject meshObj) && meshObj != null)
            {
                meshObj.GetComponent<MeshRenderer>().material.color = color;
            }
        }

        //-------------------------------------------------------------------------

        public void UpdateSearchLine(Vector3 from, Vector3 to, Color color)
        {
            if(_currentSearchLine == null)
            {
                _currentSearchLine = CreateLineRenderer("SearchLine", from, to, color, lineWidth * 0.5f).gameObject;
            }

            LineRenderer renderer = _currentSearchLine.GetComponent<LineRenderer>();
            renderer.SetPosition(0, from);
            renderer.SetPosition(1, to);
            renderer.material.color = color;
        }

        public void ClearSearchLine()
        {
            if(_currentSearchLine != null)
            {
                Destroy(_currentSearchLine);
            }
            _currentSearchLine = null;
        }

        public void AddHighlightLine(Vector3 from, Vector3 to, Color color)
        {
            LineRenderer lr = CreateLineRenderer("HighlightLine", from, to, color, lineWidth * 2f);
            _tempHighlightLines.Add(lr.gameObject);
        }

        public void ClearHighlightLines()
        {
            foreach(var line in _tempHighlightLines)
            {
                if(line != null)
                {
                    Destroy(line);
                }
            }
            _tempHighlightLines.Clear();
        }

        //-------------------------------------------------------------------------

        public (Vector3 from, Vector3 to, Color color)[] GetHighlightLinesData()
        {
            var output = new (Vector3, Vector3, Color)[_tempHighlightLines.Count];

            for(int i = 0; i < _tempHighlightLines.Count; i++)
            {
                var lr = _tempHighlightLines[i].GetComponent<LineRenderer>();
                output[i] = (lr.GetPosition(0), lr.GetPosition(1), lr.material.color);
            }

            return output;
        }

        public (Vector3 from, Vector3 to, Color color)? GetSearchLineData()
        {
            if(_currentSearchLine == null) return null;

            var lr = _currentSearchLine.GetComponent<LineRenderer>();
            return (lr.GetPosition(0), lr.GetPosition(1), lr.material.color);
        }

        //-------------------------------------------------------------------------


        private void CreateFaceMesh(int a, int b, int c)
        {
            var key = (a, b, c);
            
            if(_hullMeshMap.ContainsKey(key)) return;

            Vector3 va = _pointManager.Points[a];
            Vector3 vb = _pointManager.Points[b];
            Vector3 vc = _pointManager.Points[c];
            
            GameObject faceObj = new GameObject("HullFace");
            faceObj.transform.SetParent(transform, false);

            MeshFilter   mf = faceObj.AddComponent<MeshFilter>();
            MeshRenderer mr = faceObj.AddComponent<MeshRenderer>();

            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] { va, vb, vc, va, vb, vc };
            mesh.triangles = new int[]{ 0, 1, 2, 5, 4, 3 };
            
            mesh.RecalculateNormals();
            mf.mesh = mesh;
            
            mr.material= Instantiate(faceMaterial);
            mr.material.color = faceColor;
            faceObj.SetActive(_showFaces);

            _hullMeshMap[key] = faceObj;
        }

        private void CreateHullEdge(int a, int b)
        {
            var key = EdgeKey(a, b);

            if(_hullEdgeMap.ContainsKey(key)) return;

            LineRenderer lr = CreateLineRenderer("HullEdge", _pointManager.Points[a], _pointManager.Points[b], lineColor, lineWidth);
            _hullEdgeMap[key] = lr.gameObject;
        }


        private LineRenderer CreateLineRenderer(string name, Vector3 from, Vector3 to, Color color, float width)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(transform, false);

            LineRenderer lr = obj.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startWidth = width;
            lr.endWidth = width;
            lr.positionCount = 2;
            lr.SetPosition(0, from);
            lr.SetPosition(1, to);
            lr.material.color = color;

            return lr;
        }

    }
}

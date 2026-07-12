using System.Collections.Generic;
using UnityEngine;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public class HullPointManager : MonoBehaviour
    {
        [Header("Point Generation")]
        [SerializeField] private int numberOfPoints = 15;
        [SerializeField] private float pointScale = 0.2f;

        [Header("Point Colors")]
        [SerializeField] private Color normalPointColor;
        [SerializeField] private Color hullPointColor;
        [SerializeField] private Color currentPointColor;

        [Header("Point Material")]
        [SerializeField] private Material pointMaterial;

        private const float SpawnRadius = 3f;

        private List<Vector3> _points = new List<Vector3>();
        private List<Renderer> _renderers  = new List<Renderer>();

        //----------------------------------------------------------------------------------

        public List<Vector3> Points         => _points;
        public Color NormalPointColor       => normalPointColor;
        public Color HullPointColor         => hullPointColor;
        public Color CurrentPointColor      => currentPointColor;
        
        public int NumberOfPoints
        {   
            get => numberOfPoints; 
            set => numberOfPoints = value; 
        }

        public void GenerateRandomPoints()
        {
            float minDistance = SpawnRadius * 0.3f;
            int maxAttempts = 100;

            for(int i = 0; i < numberOfPoints; i++)
            {
                Vector3 point = Vector3.zero;

                for(int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    point = Random.insideUnitSphere * SpawnRadius;
                    bool valid = true;

                    foreach(var existing in _points)
                    {
                        if(Vector3.Distance(point, existing) < minDistance)
                        {
                            valid = false;
                            break;
                        }
                    }

                    if(valid)
                    {
                        break;
                    }
                }

                _points.Add(point);
                CreatePointObject(point, i);
            }
        }

        public void GenerateFromPoints(List<Vector3> sourcePoints)
        {
            for(int i = 0; i < sourcePoints.Count; i++)
            {
                _points.Add(sourcePoints[i]);
                CreatePointObject(sourcePoints[i], i);
            }
        }

        public void SetPointColor(int index, Color color)
        {
            if(index >= 0 && index < _renderers.Count)
            {
                _renderers[index].material.color = color;
            }
        }

        public void ResetAllPointColors()
        {
            foreach(var point in _renderers)
            {
                point.material.color = normalPointColor;
            }
        }

        public Color[] GetPointColors()
        {
            var colors = new Color[_renderers.Count];

            for(int i = 0; i < _renderers.Count; i++)
            {
                colors[i] = _renderers[i].material.color;
            }
            
            return colors;
        }

        public void SetPointColors(Color[] colors)
        {
            for(int i = 0; i < _renderers.Count; i++)
            {
                _renderers[i].material.color = colors[i];
            }
        }

        public void ClearPoints()
        {
            foreach(var point in _renderers)
            {
                Destroy(point.gameObject);
            }

            _renderers.Clear();
            _points.Clear();
        }

        private void CreatePointObject(Vector3 position, int index)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Point_" + index;
            sphere.transform.SetParent(transform, false);
            sphere.transform.localPosition = position;
            sphere.transform.localScale = Vector3.one * pointScale;

            Renderer r = sphere.GetComponent<Renderer>();
            r.material = Instantiate(pointMaterial);
            r.material.color = normalPointColor;

            _renderers.Add(r);
        }
    }
}
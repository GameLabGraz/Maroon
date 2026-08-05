using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Maroon.Parameter.ObjectsInUse;

namespace Maroon.Physics.ThreeDimensionalMotion
{
    public class CoordSystem : MonoBehaviour
    {
        private static CoordSystem _instance;

        private GameObject _trajectory;
        private GameObject _particle;
        [SerializeField] private List<GameObject> _particleObjects = new List<GameObject>();
        [SerializeField] private GameObject _planet;
        [SerializeField] private Material _lineRendererMaterial;

        [SerializeField] private TMP_Text _xLabel;
        [SerializeField] private TMP_Text _yLabel;
        [SerializeField] private TMP_Text _zLabel;
        [SerializeField] private TMP_Text _xLabel2;
        [SerializeField] private TMP_Text _yLabel2;
        [SerializeField] private TMP_Text _zLabel2;

        private string _textX = "";
        private string _textY = "";
        private string _textZ = "";
        private string _textX2 = "";
        private string _textY2 = "";
        private string _textZ2 = "";

        private Vector3 _xMax;
        private Vector3 _yMax;
        private Vector3 _zMax;

        private bool _borderValuesSet = false;
        private Color _color = Color.black;

        private List<GameObject> _origin = new List<GameObject>();

        [SerializeField] private Vector3 _scaleXYZ = new Vector3(4.5f, 3.5f, 3.5f);

        private Vector3 _realXYZMin;
        private Vector3 _realXYZMax;

        private bool drawOrigin = true;

        /// <summary>
        /// Inits the scale of objects, trajectory
        /// Inits the coord system (visibility range)
        /// </summary>
        private void Awake()
        {
            _particle = _particleObjects[0];

            Vector3 end;

            end = new Vector3(transform.position.x + _scaleXYZ.x, transform.position.y, transform.position.z);
            _xMax = end;
            end = new Vector3(transform.position.x, transform.position.y + _scaleXYZ.y, transform.position.z);
            _yMax = end;
            end = new Vector3(transform.position.x, transform.position.y, transform.position.z + _scaleXYZ.z);
            _zMax = end;

            _trajectory = new GameObject();
            _trajectory.name = "Trajectory";
            _trajectory.AddComponent<LineRenderer>();
            LineRenderer line = _trajectory.GetComponent<LineRenderer>();
            line.widthCurve = AnimationCurve.Constant(0, 1, 0.01f);
            line.material = _lineRendererMaterial;
            line.material.color = Color.red;
        }

        /// <summary>
        /// Draws the coordinate origin lines
        /// </summary>
        /// <param name="start">Start points of the lines</param>
        /// <param name="end">End points of the lines</param>
        /// <param name="color">Colour of the lines</param>
        void DrawOrigin(Vector3 start, Vector3 end, Color color)
        {
            GameObject myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = _lineRendererMaterial;
            lr.material.color = color;
            lr.widthCurve = AnimationCurve.Constant(0, 1, 0.02f);
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            _origin.Add(myLine);
            lr.numCapVertices = 5;
        }

        /// <summary>
        /// Sets the min/max values of the coordinate system to the calculated values
        /// and rounds the values for the visualization 
        /// </summary>
        /// <param name="min">Calculated min values</param>
        /// <param name="max">Calculated max values</param>
        public void SetRealCoordBorders(Vector3 min, Vector3 max)
        {
            _realXYZMin = min;
            _realXYZMax = max;

            _textX = "X: " + RoundDisplayedValues(_realXYZMax.x) + "m";
            _textY = "Y: " + RoundDisplayedValues(_realXYZMax.z) + "m";
            _textZ = "Z: " + RoundDisplayedValues(_realXYZMax.y) + "m";

            _textX2 = "X: " + RoundDisplayedValues(_realXYZMin.x) + "m";
            _textY2 = "Y: " + RoundDisplayedValues(_realXYZMin.z) + "m";
            _textZ2 = "Z: " + RoundDisplayedValues(_realXYZMin.y) + "m";

            _borderValuesSet = true;
            DrawOriginGrid();
        }

        /// <summary>
        /// Rounds calculated values into the form x.xx for visualization
        /// </summary>
        /// <param name="value">Original value</param>
        /// <returns>Rounded value</returns>
        private string RoundDisplayedValues(float value)
        {
            return System.Math.Round(value, 2) == 0.00f ? value.ToString() : System.Math.Round(value, 2).ToString();
        }

        /// <summary>
        /// Maps the calculated coord values to unity coord values
        /// </summary>
        /// <param name="point">Points to map</param>
        /// <returns>Mapped points</returns>
        public Vector3 MapValues(Vector3 point)
        {
            Vector3 tmp = point;

            tmp.x = ScaleCoords(point.x, _realXYZMin.x, _realXYZMax.x, transform.position.x, _xMax.x);
            tmp.y = ScaleCoords(point.y, _realXYZMin.y, _realXYZMax.y, transform.position.y, _yMax.y);
            tmp.z = ScaleCoords(point.z, _realXYZMin.z, _realXYZMax.z, transform.position.z, _zMax.z);

            return tmp;
        }

        /// <summary>
        /// Maps the unity coord values back to the calculated coord values
        /// </summary>
        /// <param name="point">Points to map back</param>
        /// <returns>Back-Mapped points</returns>
        public Vector3 MapValuesBack(Vector3 point)
        {
            Vector3 tmp = point;

            tmp.x = ScaleCoordsBack(point.x, _realXYZMin.x, _realXYZMax.x, transform.position.x, _xMax.x);
            tmp.y = ScaleCoordsBack(point.y, _realXYZMin.y, _realXYZMax.y, transform.position.y, _yMax.y);
            tmp.z = ScaleCoordsBack(point.z, _realXYZMin.z, _realXYZMax.z, transform.position.z, _zMax.z);

            return tmp;
        }

        /// <summary>
        /// Method for scaling points into a given range
        /// </summary>
        /// <param name="value">Value to scale</param>
        /// <param name="min">Calculated min value</param>
        /// <param name="max">Calculated max value</param>
        /// <param name="unityCoordA">Unity min value</param>
        /// <param name="unityCoordB">Unity max value</param>
        /// <returns>Scaled point</returns>
        private float ScaleCoords(double value, double min, double max, double unityCoordA, double unityCoordB)
        {
            return (float)((((unityCoordB - unityCoordA) * (value - min)) / (max - min)) + unityCoordA);
        }

        /// <summary>
        /// Method for scaling calculated points back into the original range
        /// </summary>
        /// <param name="value">Value to scale</param>
        /// <param name="unityCoordA">Unity min value</param>
        /// <param name="unityCoordB">Unity max value</param>
        /// <param name="min">Calculated min value</param>
        /// <param name="max">Calculated max value</param>
        /// <returns>Scaled point</returns>
        private float ScaleCoordsBack(double value, double unityCoordA, double unityCoordB, double min, double max)
        {
            return (float)((((unityCoordB - unityCoordA) * (value - min)) / (max - min)) + unityCoordA);
        }

        /// <summary>
        /// Draws the point at the given coordinates
        /// If drawTrajectory is true -> Trajectory is drawn too and added to a list for deleting at reset
        /// </summary>
        /// <param name="point">Point coordinates to draw</param>
        /// <param name="drawTrajectory">Bool if trajectory should be drawn</param>
        /// <param name="particleInUse">Current object in use (e.g. Particle, Ball, Satellite)</param>
        public void DrawPoint(Vector3 point, bool drawTrajectory)
        {
            _particle.transform.position = MapValues(point);

            if (drawTrajectory)
            {
                LineRenderer line = _trajectory.GetComponent<LineRenderer>();
                line.SetPosition(line.positionCount++, MapValues(point));
            }

        }

        public void DrawPoint(Vector3 point, Vector3 look_at, bool drawTrajectory)
        {
            DrawPoint(point, drawTrajectory);
            _particle.transform.LookAt(MapValues(look_at));
        }


        public static CoordSystem Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<CoordSystem>();
                return _instance;
            }
        }

        /// <summary>
        /// Resets the object
        /// </summary>
        public void ResetObject()
        {
            // deleting origin lines
            foreach (GameObject line in _origin)
                Destroy(line);

            _origin.Clear();

            _particle.SetActive(false);
            _particle = _particleObjects[0];
            _planet.SetActive(false);

            _textX = "";
            _textY = "";
            _textZ = "";
            _textX2 = "";
            _textY2 = "";
            _textZ2 = "";

            _borderValuesSet = false;
            _color = Color.black;

            _trajectory.GetComponent<LineRenderer>().positionCount = 0;
        }

        /// <summary>
        /// Sets the particle which is used active
        /// </summary>
        public void SetParticleActive(ParticleObject particleInUse)
        {
            switch (particleInUse)
            {
                case ParticleObject.Default:
                    _particle = _particleObjects[0];
                    break;
                case ParticleObject.Ball:
                    _particle = _particleObjects[1];
                    break;
                case ParticleObject.Satellite:
                    _particle = _particleObjects[2];
                    _planet.SetActive(true);
                    break;
                case ParticleObject.Rocket:
                    _particle = _particleObjects[3];
                    break;
                default:
                    _particle = _particleObjects[0];
                    break;
            }

            _particle.SetActive(true);
        }

        /// <summary>
        /// UI checkbox for show/hide labels
        /// </summary>
        /// <param name="show">Bool if checkbox is active</param>
        public void ShowLabels(bool show)
        {
            if (show)
            {
                _xLabel.text = _textX;
                _yLabel.text = _textY;
                _zLabel.text = _textZ;
                _xLabel2.text = _textX2;
                _yLabel2.text = _textY2;
                _zLabel2.text = _textZ2;
            }
            else
            {
                _xLabel.text = "";
                _yLabel.text = "";
                _zLabel.text = "";
                _xLabel2.text = "";
                _yLabel2.text = "";
                _zLabel2.text = "";
            }
        }

        /// <summary>
        /// UI checkbox for show/hide origin grid
        /// Method calculates the origin points and map them for drawing
        /// </summary>
        /// <param name="draw">Bool if grid should be drawn or deleted</param>
        public void DrawOriginGrid()
        {
            if (drawOrigin && _borderValuesSet)
            {
                Vector3 start;
                Vector3 end;

                // x line
                start = new Vector3(_realXYZMin.x, 0, 0);
                end = new Vector3(_realXYZMax.x, 0, 0);
                DrawOrigin(MapValues(start), MapValues(end), _color);
                _xLabel.transform.position = MapValues(end);
                _xLabel2.transform.position = MapValues(start);

                // y line
                start = new Vector3(0, 0, _realXYZMin.z);
                end = new Vector3(0, 0, _realXYZMax.z);
                DrawOrigin(MapValues(start), MapValues(end), _color);
                _yLabel.transform.position = MapValues(end);
                _yLabel2.transform.position = MapValues(start);

                // z line
                start = new Vector3(0, _realXYZMin.y, 0);
                end = new Vector3(0, _realXYZMax.y, 0);
                DrawOrigin(MapValues(start), MapValues(end), _color);
                _zLabel.transform.position = MapValues(end);
                _zLabel2.transform.position = MapValues(start);

                _planet.transform.position = new Vector3(-0.2f, 1.75f, 6.4f);
            }
            else
            {
                // deleting origin lines
                foreach (GameObject line in _origin)
                    Destroy(line);
                _origin.Clear();
            }
        }

        public void DrawOriginGrid(bool draw)
        {
            drawOrigin = draw;
            DrawOriginGrid();
        }

        /// <summary>
        /// Sets the member color
        /// </summary>
        /// <param name="col">Color to use</param>    
        public void SetColor(Color col)
        {
            _color = col;
        }

    }
}
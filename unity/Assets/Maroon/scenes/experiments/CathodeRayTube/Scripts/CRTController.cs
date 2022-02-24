using System;
using System.Collections.Generic;
using Maroon.UI.Charts;
using UnityEngine;
using XCharts;

public enum DistanceEnum
{
    Both,
    Vertical,
    Horizontal
}

public enum OrderEnum
{
    VerticalHorizontal,
    HorizontalVertical,
    Vertical,
    Horizontal
}

public enum XAxisEnum
{
    X,
    Time
}

public enum YAxisEnum
{
    X,
    Vx,
    Fx,
    Y,
    Vy,
    Fy,
    Z,
    Vz,
    Fz
}

namespace Maroon.Physics.CathodeRayTube
{
    public class CRTController : MonoBehaviour
    {
        private ElectronLineController _electronLineController;
        [SerializeField] private GameObject screen;
        [SerializeField] private GameObject cathode;
        [SerializeField] private GameObject anode;
        [SerializeField] private GameObject verticalDeflectionPlate;
        [SerializeField] private GameObject horizontalDeflectionPlate;
        [SerializeField] private QuantityInt vX;
        [SerializeField] private QuantityInt vY;
        [SerializeField] private QuantityInt vZ;
        [SerializeField] private QuantityFloat d;
        [SerializeField] private SimpleLineChart plot;

        [SerializeField] private QuantityString fXInfo;
        [SerializeField] private QuantityString fYInfo;
        [SerializeField] private QuantityString fZInfo;
        [SerializeField] private QuantityString eXInfo;
        [SerializeField] private QuantityString eYInfo;
        [SerializeField] private QuantityString eZInfo;

        private int _order;
        public int Order
        {
            get => _order;
            set
            {
                _order = value;
                UpdateOrder();
            }
        }

        public int Distance { get; set; }
        public int XAxis { get; set; }
        public int YAxis { get; set; }

        private const float ElectronCharge = -1.6022e-19f;
        private const float ElectronMass = 9.11e-31f;
        private float _electronGunLength;
        public int lineResolution = 500;

        private GameObject _verticalDeflectionPlateTop;
        private GameObject _verticalDeflectionPlateBottom;
        private GameObject _horizontalDeflectionPlateLeft;
        private GameObject _horizontalDeflectionPlateRight;

        private Vector3 _verticalDeflectionPlateStartPos;
        private Vector3 _horizontalDeflectionPlateStartPos;

        private float _vertDeflPlateDistance;
        private float _horizDeflPlateDistance;

        private List<Vector3> _pointData = new List<Vector3>();
        private List<Vector3> _velocityData = new List<Vector3>();
        private List<Vector3> _forceData = new List<Vector3>();

        private void Start()
        {
            _electronLineController = gameObject.GetComponentInChildren<ElectronLineController>();
            _electronGunLength = anode.transform.position.x - GetCRTStart().x;
            _vertDeflPlateDistance = d;
            _horizDeflPlateDistance = d;

            _verticalDeflectionPlateTop = verticalDeflectionPlate.transform.GetChild(0).gameObject;
            _verticalDeflectionPlateBottom = verticalDeflectionPlate.transform.GetChild(1).gameObject;
            _horizontalDeflectionPlateLeft = horizontalDeflectionPlate.transform.GetChild(0).gameObject;
            _horizontalDeflectionPlateRight = horizontalDeflectionPlate.transform.GetChild(1).gameObject;

            _verticalDeflectionPlateStartPos = verticalDeflectionPlate.transform.position;
            _horizontalDeflectionPlateStartPos = horizontalDeflectionPlate.transform.position;

            for (int i = 0; i < lineResolution; i++)
            {
                _pointData.Add(Vector3.zero);
                _velocityData.Add(Vector3.zero);
                _forceData.Add(Vector3.zero);
            }

            XAxis = (int)XAxisEnum.X;
            YAxis = (int)YAxisEnum.Y;
            
            UpdateDistance();
            UpdateOrder();
            UpdateInformation();
        }

        private void FixedUpdate()
        {
            UpdateInformation();
        }

        public void UpdateDistance()
        {
            if (!SimulationController.Instance.SimulationRunning)
                return;
            
            switch ((DistanceEnum)Distance)
            {
                case DistanceEnum.Both:
                    _vertDeflPlateDistance = d;
                    _horizDeflPlateDistance = d;
                    break;
                case DistanceEnum.Vertical:
                    _vertDeflPlateDistance = d;
                    break;
                case DistanceEnum.Horizontal:
                    _horizDeflPlateDistance = d;
                    break;
                default:
                    _vertDeflPlateDistance = d;
                    _horizDeflPlateDistance = d;
                    break;
            }
            
            var position = verticalDeflectionPlate.transform.position;
            _verticalDeflectionPlateTop.transform.position = position + new Vector3(0, _vertDeflPlateDistance / 2, 0);
            _verticalDeflectionPlateBottom.transform.position = position - new Vector3(0, _vertDeflPlateDistance / 2, 0);
            position = horizontalDeflectionPlate.transform.position;
            _horizontalDeflectionPlateRight.transform.position = position + new Vector3(0, 0, _horizDeflPlateDistance / 2);
            _horizontalDeflectionPlateLeft.transform.position = position - new Vector3(0, 0, _horizDeflPlateDistance / 2);
                        
            _electronLineController.UpdateElectronLine();
        }

        public void UpdateOrder()
        {
            if (!SimulationController.Instance.SimulationRunning)
                return;
            
            _horizontalDeflectionPlateLeft.SetActive(true);
            _horizontalDeflectionPlateRight.SetActive(true);
            _verticalDeflectionPlateTop.SetActive(true);
            _verticalDeflectionPlateBottom.SetActive(true);

            Vector3 position;
            switch ((OrderEnum)Order)
            {
                case OrderEnum.HorizontalVertical:
                    verticalDeflectionPlate.transform.position = _horizontalDeflectionPlateStartPos;
                    horizontalDeflectionPlate.transform.position = _verticalDeflectionPlateStartPos;
                    break;
                
                case OrderEnum.VerticalHorizontal:
                    verticalDeflectionPlate.transform.position = _verticalDeflectionPlateStartPos;
                    horizontalDeflectionPlate.transform.position = _horizontalDeflectionPlateStartPos;
                    break;

                case OrderEnum.Horizontal:
                    position = _verticalDeflectionPlateStartPos;
                    position.x += (_horizontalDeflectionPlateStartPos.x - _verticalDeflectionPlateStartPos.x) / 2;
                    horizontalDeflectionPlate.transform.position = position;
                    verticalDeflectionPlate.transform.position = Vector3.zero;
                    _verticalDeflectionPlateTop.SetActive(false);
                    _verticalDeflectionPlateBottom.SetActive(false);
                    break;
                
                case OrderEnum.Vertical:
                    position = _verticalDeflectionPlateStartPos;
                    position.x += (_horizontalDeflectionPlateStartPos.x - _verticalDeflectionPlateStartPos.x) / 2;
                    verticalDeflectionPlate.transform.position = position;
                    horizontalDeflectionPlate.transform.position = Vector3.zero;
                    _horizontalDeflectionPlateLeft.SetActive(false);
                    _horizontalDeflectionPlateRight.SetActive(false);
                    break;

                default:
                    verticalDeflectionPlate.transform.position = _verticalDeflectionPlateStartPos;
                    horizontalDeflectionPlate.transform.position = _horizontalDeflectionPlateStartPos;
                    break;
            }

            var newPosition = verticalDeflectionPlate.transform.position;
            _verticalDeflectionPlateTop.transform.position = newPosition + new Vector3(0, _vertDeflPlateDistance / 2, 0);
            _verticalDeflectionPlateBottom.transform.position = newPosition - new Vector3(0, _vertDeflPlateDistance / 2, 0);
            newPosition = horizontalDeflectionPlate.transform.position;
            _horizontalDeflectionPlateRight.transform.position = newPosition + new Vector3(0, 0, _horizDeflPlateDistance / 2);
            _horizontalDeflectionPlateLeft.transform.position = newPosition - new Vector3(0, 0, _horizDeflPlateDistance / 2);
            
            _electronLineController.UpdateElectronLine();
        }

        private void UpdateInformation()
        {
            int informationResolution = 1000;
            float length;
            float size;

            int eXValue = (int)(vX / (Math.Truncate(informationResolution * _electronGunLength) / informationResolution));
            int eYValue = (int)(vY / (Math.Truncate(informationResolution * _vertDeflPlateDistance) / informationResolution));
            int eZValue = (int)(vZ / (Math.Truncate(informationResolution * _horizDeflPlateDistance) / informationResolution));

            eXInfo.Value = vX.Value + " / " +
                           (Math.Truncate(informationResolution * _electronGunLength) / informationResolution)
                           + " \n= " + eXValue + " V/m";
            eYInfo.Value = vY.Value + " / " +
                           (Math.Truncate(informationResolution * _vertDeflPlateDistance) / informationResolution)
                           + " \n= " + eYValue + " V/m";
            eZInfo.Value = vZ.Value + " / " +
                           (Math.Truncate(informationResolution * _horizDeflPlateDistance) / informationResolution)
                           + " \n= " + eZValue + " V/m";

            float fXValue = -ElectronCharge *
                       (vX / (float)(Math.Truncate(informationResolution * _electronGunLength) /
                                     informationResolution)) * (float)Math.Pow(10, 15);
            float fYValue = -ElectronCharge *
                            (vY / (float)(Math.Truncate(informationResolution * _vertDeflPlateDistance) /
                                          informationResolution)) * (float)Math.Pow(10, 15);
            float fZValue = -ElectronCharge *
                            (vZ / (float)(Math.Truncate(informationResolution * _horizDeflPlateDistance) /
                                          informationResolution)) * (float)Math.Pow(10, 15);

            fXInfo.Value = "1.6022e-19 * " + eXValue + " * H(" +
                           (Math.Truncate(informationResolution * _electronGunLength) / informationResolution) +
                           " - x)" + " \n= " + fXValue + " * (10 ^ -15) N"  ;

            size = _verticalDeflectionPlateTop.GetComponent<Renderer>().bounds.size.z / 2;
            length = Math.Abs(GetCRTStart().x - verticalDeflectionPlate.transform.position.x);
            fYInfo.Value = "1.6022e-19 * " + eYValue + " * \nH(" +
                           (Math.Truncate(informationResolution * size) / informationResolution)
                           + " - abs(x - " + (Math.Truncate(informationResolution * length) / informationResolution) +
                           "))" + " * \nH(" + _verticalDeflectionPlateTop.GetComponent<Renderer>().bounds.size.z / 2 
                           + " - abs(z - " + GetCRTStart().z + "))"
                           + " \n= " + fYValue + " * (10 ^ -15) N";

            size = _horizontalDeflectionPlateLeft.GetComponent<Renderer>().bounds.size.y / 2;
            length = Math.Abs(GetCRTStart().x - horizontalDeflectionPlate.transform.position.x);
            fZInfo.Value = "1.6022e-19 * " + eZValue + " * \nH(" +
                           (Math.Truncate(informationResolution * size) / informationResolution)
                           + " - abs(x - " + (Math.Truncate(informationResolution * length) / informationResolution) +
                           "))" + " * \nH(" + _horizontalDeflectionPlateLeft.GetComponent<Renderer>().bounds.size.y / 2 
                           + " - abs(y - " + GetCRTStart().y + "))"
                           + " \n= " + fZValue + " * (10 ^ -15) N";
        }

        public void UpdateData(List<Vector3> points, List<Vector3> velocities, List<Vector3> forces)
        {
            _pointData = points;
            _velocityData = velocities;
            _forceData = forces;
        }

        public void UpdatePlot()
        {
            plot.ResetObject();
            float timeStep = GetTimeStep();
            LineChart lineChart = plot.GetComponent<LineChart>();
            lineChart.series.GetSerie(0).maxShow = lineResolution;
            List<float> xAxisData = new List<float>();
            List<float> yAxisData = new List<float>();

            lineChart.xAxis0.minMaxType = Axis.AxisMinMaxType.Default;
            lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Default;

            switch ((XAxisEnum)XAxis)
            {
                case XAxisEnum.X:
                    lineChart.xAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.xAxis0.min = GetCRTStart().x;
                    lineChart.xAxis0.max = screen.transform.position.x;
                    for (int i = 0; i < lineResolution; i++)
                        xAxisData.Add(_pointData[i].x);
                    break;
                case XAxisEnum.Time:
                    lineChart.xAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.xAxis0.min = 0;
                    lineChart.xAxis0.max = lineResolution * timeStep;
                    for (int i = 0; i < lineResolution; i++)
                        xAxisData.Add(i * timeStep);
                    break;
            }

            switch ((YAxisEnum)YAxis)
            {
                case YAxisEnum.X:
                    lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.yAxis0.min = GetCRTStart().x;
                    lineChart.yAxis0.max = screen.transform.position.x;
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(_pointData[i].x);
                    break;
                case YAxisEnum.Vx:
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(_velocityData[i].x);
                    break;
                case YAxisEnum.Fx:
                    lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.yAxis0.min = vX / 100 * -(float)Math.Pow(10, -15);
                    lineChart.yAxis0.max = vX / 100 * (float)Math.Pow(10, -15);
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(_forceData[i].x);
                    break;
                case YAxisEnum.Y:
                    lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.yAxis0.min =
                        screen.transform.position.y - screen.GetComponent<Renderer>().bounds.size.y / 2;
                    lineChart.yAxis0.max =
                        screen.transform.position.y + screen.GetComponent<Renderer>().bounds.size.y / 2;
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(_pointData[i].y);
                    break;
                case YAxisEnum.Vy:
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(_velocityData[i].y);
                    break;
                case YAxisEnum.Fy:
                    lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.yAxis0.min = -(float)Math.Pow(10, -15);
                    lineChart.yAxis0.max = (float)Math.Pow(10, -15);
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(_forceData[i].y);
                    break;
                case YAxisEnum.Z:
                    lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.yAxis0.min =
                        screen.transform.position.z - screen.GetComponent<Renderer>().bounds.size.z / 2;
                    lineChart.yAxis0.max =
                        screen.transform.position.z + screen.GetComponent<Renderer>().bounds.size.z / 2;
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(_pointData[i].z);
                    break;
                case YAxisEnum.Vz:
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(_velocityData[i].z);
                    break;
                case YAxisEnum.Fz:
                    lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.yAxis0.min = -(float)Math.Pow(10, -15);
                    lineChart.yAxis0.max = (float)Math.Pow(10, -15);
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(_forceData[i].z);
                    break;
            }

            for (int i = 0; i < lineResolution; i++)
                plot.AddData(xAxisData[i], yAxisData[i]);
        }

        private float H(float x)
        {
            return x == 0 ? 0.5f : (1 + x / Math.Abs(x)) / 2;
        }

        public Vector3 ApplyForce(Vector3 currentPoint)
        {
            float x;
            float y;
            float z;
            float length;
            float size;
            float dist;

            Vector3 point = Vector3.zero;

            point.x = -ElectronCharge * (vX / _electronGunLength) *
                      H((GetCRTStart().x + _electronGunLength) - currentPoint.x);

            float scale = _verticalDeflectionPlateTop.GetComponent<Renderer>().bounds.size.x;
            if (verticalDeflectionPlate.transform.position != Vector3.zero)
            {
                length = Math.Abs(GetCRTStart().x - verticalDeflectionPlate.transform.position.x);
                x = (scale / 2) - Math.Abs(currentPoint.x - (GetCRTStart().x + length));

                dist = _vertDeflPlateDistance / 2;
                y = dist - Math.Abs(currentPoint.y - GetCRTStart().y);

                size = _verticalDeflectionPlateTop.GetComponent<Renderer>().bounds.size.z / 2;
                z = size - Math.Abs(currentPoint.z - GetCRTStart().z);

                point.y = -ElectronCharge * (vY / _vertDeflPlateDistance) * H(x) * H(y) * H(z);
            }

            scale = _horizontalDeflectionPlateLeft.GetComponent<Renderer>().bounds.size.x;
            if (horizontalDeflectionPlate.transform.position != Vector3.zero)
            {
                length = Math.Abs(GetCRTStart().x - horizontalDeflectionPlate.transform.position.x);
                x = (scale / 2) - Math.Abs(currentPoint.x - (GetCRTStart().x + length));

                size = _horizontalDeflectionPlateLeft.GetComponent<Renderer>().bounds.size.y / 2;
                y = size - Math.Abs(currentPoint.y - GetCRTStart().y);

                dist = _horizDeflPlateDistance / 2;
                z = dist - Math.Abs(currentPoint.z - GetCRTStart().z);

                point.z = -ElectronCharge * (vZ / _horizDeflPlateDistance) * H(x) * H(y) * H(z);
            }

            return point;
        }

        public Vector3 RK4(Vector3 currentPoint)
        {
            float timeStep = GetTimeStep();
            Vector3 k1 = timeStep * ApplyForce(currentPoint) / ElectronMass;
            Vector3 k23 = timeStep * ApplyForce(currentPoint + new Vector3(timeStep / 2, 0, 0)) / ElectronMass;
            Vector3 k4 = timeStep * ApplyForce(currentPoint + new Vector3(timeStep, 0, 0)) / ElectronMass;
            return (k1 + 2 * k23 + 2 * k23 + k4) / 6;
        }

        public float GetTimeStep()
        {
            float v = (float)Math.Sqrt(-2 * ElectronCharge * vX / ElectronMass);
            float t = (float)Math.Sqrt(2 * _electronGunLength * ElectronMass /
                                       (ElectronCharge * (-vX / _electronGunLength)));
            t += GetCRTDist() / v;
            return t / lineResolution;
        }

        public float GetCRTDist()
        {
            return screen.transform.position.x - GetCRTStart().x;
        }

        public Vector3 GetCRTStart()
        {
            Vector3 point = cathode.transform.position;
            point.x += cathode.GetComponent<Renderer>().bounds.size.x / 2;
            return point;
        }
    }
}
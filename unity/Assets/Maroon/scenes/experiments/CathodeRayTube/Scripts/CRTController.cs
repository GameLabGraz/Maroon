using System;
using System.Collections.Generic;
using Maroon.UI.Charts;
using UnityEngine;
using XCharts;

public enum DistanceEnum
{
    Both,
    Horizontal,
    Vertical
}

public enum OrderEnum
{
    HorizontalVertical,
    VerticalHorizontal,
    Horizontal,
    Vertical
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
        [SerializeField] private GameObject screen;
        [SerializeField] private GameObject cathode;
        [SerializeField] private GameObject anode;
        [SerializeField] private GameObject horizontalCapacitor;
        [SerializeField] private GameObject verticalCapacitor;
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

        public int Order { get; set; }
        public int Distance { get; set; }
        public int XAxis { get; set; }
        public int YAxis { get; set; }

        private const float ElectronCharge = -1.6022e-19f;
        private const float ElectronMass = 9.11e-31f;
        private float _electronGunLength;
        public int lineResolution = 500;

        private GameObject _horizontalCapacitorTop;
        private GameObject _horizontalCapacitorBottom;
        private GameObject _verticalCapacitorLeft;
        private GameObject _verticalCapacitorRight;

        private Vector3 _horizCapStartPos;
        private Vector3 _vertCapStartPos;

        private float _horizontalDistance;
        private float _verticalDistance;

        private List<Vector3> _pointData = new List<Vector3>();
        private List<Vector3> _velocityData = new List<Vector3>();
        private List<Vector3> _forceData = new List<Vector3>();

        private void Start()
        {
            _electronGunLength = anode.transform.position.x - GetCRTStart().x;
            _horizontalDistance = d;
            _verticalDistance = d;

            _horizontalCapacitorTop = horizontalCapacitor.transform.GetChild(0).gameObject;
            _horizontalCapacitorBottom = horizontalCapacitor.transform.GetChild(1).gameObject;
            _verticalCapacitorLeft = verticalCapacitor.transform.GetChild(0).gameObject;
            _verticalCapacitorRight = verticalCapacitor.transform.GetChild(1).gameObject;

            _horizCapStartPos = horizontalCapacitor.transform.position;
            _vertCapStartPos = verticalCapacitor.transform.position;

            for (int i = 0; i < lineResolution; i++)
            {
                _pointData.Add(Vector3.zero);
                _velocityData.Add(Vector3.zero);
                _forceData.Add(Vector3.zero);
            }

            XAxis = (int)XAxisEnum.X;
            YAxis = (int)YAxisEnum.Y;
        }

        private void FixedUpdate()
        {
            UpdateDistance();
            UpdateOrder();
            UpdateInformation();
        }

        private void UpdateDistance()
        {
            switch ((DistanceEnum)Distance)
            {
                case DistanceEnum.Both:
                    _horizontalDistance = d;
                    _verticalDistance = d;
                    break;
                case DistanceEnum.Horizontal:
                    _horizontalDistance = d;
                    break;
                case DistanceEnum.Vertical:
                    _verticalDistance = d;
                    break;
                default:
                    _horizontalDistance = d;
                    _verticalDistance = d;
                    break;
            }
        }

        private void UpdateOrder()
        {
            _verticalCapacitorLeft.SetActive(true);
            _verticalCapacitorRight.SetActive(true);
            _horizontalCapacitorTop.SetActive(true);
            _horizontalCapacitorBottom.SetActive(true);

            Vector3 position;
            switch ((OrderEnum)Order)
            {
                case OrderEnum.HorizontalVertical:
                    horizontalCapacitor.transform.position = _horizCapStartPos;
                    verticalCapacitor.transform.position = _vertCapStartPos;
                    break;

                case OrderEnum.VerticalHorizontal:
                    horizontalCapacitor.transform.position = _vertCapStartPos;
                    verticalCapacitor.transform.position = _horizCapStartPos;
                    break;

                case OrderEnum.Horizontal:
                    position = _horizCapStartPos;
                    position.x += (_vertCapStartPos.x - _horizCapStartPos.x) / 2;
                    horizontalCapacitor.transform.position = position;
                    verticalCapacitor.transform.position = Vector3.zero;
                    _verticalCapacitorLeft.SetActive(false);
                    _verticalCapacitorRight.SetActive(false);
                    break;

                case OrderEnum.Vertical:
                    position = _horizCapStartPos;
                    position.x += (_vertCapStartPos.x - _horizCapStartPos.x) / 2;
                    verticalCapacitor.transform.position = position;
                    horizontalCapacitor.transform.position = Vector3.zero;
                    _horizontalCapacitorTop.SetActive(false);
                    _horizontalCapacitorBottom.SetActive(false);
                    break;

                default:
                    horizontalCapacitor.transform.position = _horizCapStartPos;
                    verticalCapacitor.transform.position = _vertCapStartPos;
                    break;
            }

            var newPosition = horizontalCapacitor.transform.position;
            _horizontalCapacitorTop.transform.position = newPosition + new Vector3(0, _horizontalDistance / 2, 0);
            _horizontalCapacitorBottom.transform.position = newPosition - new Vector3(0, _horizontalDistance / 2, 0);
            newPosition = verticalCapacitor.transform.position;
            _verticalCapacitorRight.transform.position = newPosition + new Vector3(0, 0, _verticalDistance / 2);
            _verticalCapacitorLeft.transform.position = newPosition - new Vector3(0, 0, _verticalDistance / 2);
        }

        private void UpdateInformation()
        {
            int informationResolution = 1000;
            float length;
            float size;

            int eXValue = (int)(vX / (Math.Truncate(informationResolution * _electronGunLength) / informationResolution));
            int eYValue = (int)(vY / (Math.Truncate(informationResolution * _horizontalDistance) / informationResolution));
            int eZValue = (int)(vZ / (Math.Truncate(informationResolution * _verticalDistance) / informationResolution));

            eXInfo.Value = vX.Value + " / " +
                           (Math.Truncate(informationResolution * _electronGunLength) / informationResolution)
                           + " \n= " + eXValue + " V/m";
            eYInfo.Value = vY.Value + " / " +
                           (Math.Truncate(informationResolution * _horizontalDistance) / informationResolution)
                           + " \n= " + eYValue + " V/m";
            eZInfo.Value = vZ.Value + " / " +
                           (Math.Truncate(informationResolution * _verticalDistance) / informationResolution)
                           + " \n= " + eZValue + " V/m";

            float fXValue = -ElectronCharge *
                       (vX / (float)(Math.Truncate(informationResolution * _electronGunLength) /
                                     informationResolution)) * (float)Math.Pow(10, 15);
            float fYValue = -ElectronCharge *
                            (vY / (float)(Math.Truncate(informationResolution * _horizontalDistance) /
                                          informationResolution)) * (float)Math.Pow(10, 15);
            float fZValue = -ElectronCharge *
                            (vZ / (float)(Math.Truncate(informationResolution * _verticalDistance) /
                                          informationResolution)) * (float)Math.Pow(10, 15);

            fXInfo.Value = "1.6022e-19 * " + eXValue + " * H(" +
                           (Math.Truncate(informationResolution * _electronGunLength) / informationResolution) +
                           " - x)" + " \n= " + fXValue + " * (10 ^ -15) N"  ;

            size = _horizontalCapacitorTop.GetComponent<Renderer>().bounds.size.z / 2;
            length = Math.Abs(GetCRTStart().x - horizontalCapacitor.transform.position.x);
            fYInfo.Value = "1.6022e-19 * " + eYValue + " * \nH(" +
                           (Math.Truncate(informationResolution * size) / informationResolution)
                           + " - abs(x - " + (Math.Truncate(informationResolution * length) / informationResolution) +
                           "))" + " * \nH(" + _horizontalCapacitorTop.GetComponent<Renderer>().bounds.size.z / 2 
                           + " - abs(z - " + GetCRTStart().z + "))"
                           + " \n= " + fYValue + " * (10 ^ -15) N";

            size = _verticalCapacitorLeft.GetComponent<Renderer>().bounds.size.y / 2;
            length = Math.Abs(GetCRTStart().x - verticalCapacitor.transform.position.x);
            fZInfo.Value = "1.6022e-19 * " + eZValue + " * \nH(" +
                           (Math.Truncate(informationResolution * size) / informationResolution)
                           + " - abs(x - " + (Math.Truncate(informationResolution * length) / informationResolution) +
                           "))" + " * \nH(" + _verticalCapacitorLeft.GetComponent<Renderer>().bounds.size.y / 2 
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

            float scale = _horizontalCapacitorTop.GetComponent<Renderer>().bounds.size.x;
            if (horizontalCapacitor.transform.position != Vector3.zero)
            {
                length = Math.Abs(GetCRTStart().x - horizontalCapacitor.transform.position.x);
                x = (scale / 2) - Math.Abs(currentPoint.x - (GetCRTStart().x + length));

                dist = _horizontalDistance / 2;
                y = dist - Math.Abs(currentPoint.y - GetCRTStart().y);

                size = _horizontalCapacitorTop.GetComponent<Renderer>().bounds.size.z / 2;
                z = size - Math.Abs(currentPoint.z - GetCRTStart().z);

                point.y = -ElectronCharge * (vY / _horizontalDistance) * H(x) * H(y) * H(z);
            }

            scale = _verticalCapacitorLeft.GetComponent<Renderer>().bounds.size.x;
            if (verticalCapacitor.transform.position != Vector3.zero)
            {
                length = Math.Abs(GetCRTStart().x - verticalCapacitor.transform.position.x);
                x = (scale / 2) - Math.Abs(currentPoint.x - (GetCRTStart().x + length));

                size = _verticalCapacitorLeft.GetComponent<Renderer>().bounds.size.y / 2;
                y = size - Math.Abs(currentPoint.y - GetCRTStart().y);

                dist = _verticalDistance / 2;
                z = dist - Math.Abs(currentPoint.z - GetCRTStart().z);

                point.z = -ElectronCharge * (vZ / _verticalDistance) * H(x) * H(y) * H(z);
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
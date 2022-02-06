using System;
using System.Collections.Generic;
using System.Linq;
using Maroon.UI.Charts;
using UnityEngine;
using XCharts;

namespace Maroon.Physics.CathodeRayTube
{
    public class CRTController : MonoBehaviour
    {
        [SerializeField] private GameObject Screen;
        [SerializeField] private GameObject Cathode;
        [SerializeField] private GameObject Anode;
        [SerializeField] private GameObject HorizontalCapacitor;
        [SerializeField] private GameObject VerticalCapacitor;
        [SerializeField] private QuantityInt V_x;
        [SerializeField] private QuantityInt V_y;
        [SerializeField] private QuantityInt V_z;
        [SerializeField] private QuantityFloat d;
        [SerializeField] private SimpleLineChart plot;
        
        [SerializeField] private QuantityInt Ex;
        [SerializeField] private QuantityInt Ey;
        [SerializeField] private QuantityInt Ez;
        [SerializeField] private QuantityString ExCalc;
        [SerializeField] private QuantityString EyCalc;
        [SerializeField] private QuantityString EzCalc;
        [SerializeField] private QuantityFloat Fx;
        [SerializeField] private QuantityFloat Fy;
        [SerializeField] private QuantityFloat Fz;
        [SerializeField] private QuantityString FxCalc;
        [SerializeField] private QuantityString FyCalc;
        [SerializeField] private QuantityString FzCalc;
        public int Order { get; set; }
        public int Distance { get; set; }
        public int xAxis { get; set; }
        public int yAxis { get; set; }
        
        private const float _electronCharge = -1.6022e-19f;
        private const float _electronMass = 9.11e-31f;
        private float _electronGunLength;
        public int lineResolution = 500;

        private GameObject HorizontalCapacitorTop;
        private GameObject HorizontalCapacitorBottom;
        private GameObject VerticalCapacitorLeft;
        private GameObject VerticalCapacitorRight;

        private Vector3 HorizCapStartPos;
        private Vector3 VertCapStartPos;

        private float horizontalDistance;
        private float verticalDistance;

        private List<Vector3> pointData = new List<Vector3>();
        private List<Vector3> velocityData = new List<Vector3>();
        private List<Vector3> forceData = new List<Vector3>();

        private void Start()
        {
            _electronGunLength = Anode.transform.position.x - GetCRTStart().x;
            horizontalDistance = d;
            verticalDistance = d;
            
            HorizontalCapacitorTop = HorizontalCapacitor.transform.GetChild(0).gameObject;
            HorizontalCapacitorBottom = HorizontalCapacitor.transform.GetChild(1).gameObject;
            VerticalCapacitorLeft = VerticalCapacitor.transform.GetChild(0).gameObject;
            VerticalCapacitorRight = VerticalCapacitor.transform.GetChild(1).gameObject;
            
            HorizCapStartPos = HorizontalCapacitor.transform.position;
            VertCapStartPos = VerticalCapacitor.transform.position;

            for (int i = 0; i < lineResolution; i++)
            {
                pointData.Add(new Vector3(0, 0, 0));
                velocityData.Add(new Vector3(0, 0, 0));
                forceData.Add(new Vector3(0, 0, 0));
            }

            xAxis = 0;
            yAxis = 3;
        }

        private void FixedUpdate()
        {
            updateDistance();
            updateOrder();
            updateInformation();
        }

        void updateDistance()
        {
            switch (Distance)
            {
                case 0:
                    horizontalDistance = d;
                    verticalDistance = d;
                    break;
                case 1:
                    horizontalDistance = d;
                    break;
                case 2:
                    verticalDistance = d;
                    break;
                default:
                    horizontalDistance = d;
                    verticalDistance = d;
                    break;
            }
        }
        
        void updateOrder()
        {
            Vector3 position;
            switch (Order)
            {
                case 0:
                    HorizontalCapacitor.transform.position = HorizCapStartPos;
                    VerticalCapacitor.transform.position = VertCapStartPos;
                    break;
                
                case 1:
                    HorizontalCapacitor.transform.position = VertCapStartPos;
                    VerticalCapacitor.transform.position = HorizCapStartPos;
                    break;

                case 2:
                    position = HorizCapStartPos;
                    position.x += (VertCapStartPos.x - HorizCapStartPos.x) / 2;
                    HorizontalCapacitor.transform.position = position;
                    VerticalCapacitor.transform.position = new Vector3(0, 0, 0);
                    break;
                
                case 3:
                    position = HorizCapStartPos;
                    position.x += (VertCapStartPos.x - HorizCapStartPos.x) / 2;
                    VerticalCapacitor.transform.position = position;
                    HorizontalCapacitor.transform.position = new Vector3(0, 0, 0);
                    break;
                
                default:
                    HorizontalCapacitor.transform.position = HorizCapStartPos;
                    VerticalCapacitor.transform.position = VertCapStartPos;
                    break;
            }

            var newPosition = HorizontalCapacitor.transform.position;
            HorizontalCapacitorTop.transform.position = newPosition + new Vector3(0, horizontalDistance / 2, 0);
            HorizontalCapacitorBottom.transform.position = newPosition - new Vector3(0, horizontalDistance / 2, 0);
            newPosition = VerticalCapacitor.transform.position;
            VerticalCapacitorRight.transform.position = newPosition + new Vector3(0, 0,verticalDistance / 2);
            VerticalCapacitorLeft.transform.position = newPosition - new Vector3(0, 0, verticalDistance / 2);
        }
        
        void updateInformation()
        {
            int informationResolution = 1000;
            float length;
            float size;
            
            Ex.Value = (int)(V_x / (Math.Truncate(informationResolution * _electronGunLength) / informationResolution));
            Ey.Value = (int)(V_y / (Math.Truncate(informationResolution * horizontalDistance) / informationResolution));
            Ez.Value = (int)(V_z / (Math.Truncate(informationResolution * verticalDistance) / informationResolution));

            ExCalc.Value = V_x.Value + " / " + (Math.Truncate(informationResolution * _electronGunLength) / informationResolution);
            EyCalc.Value = V_y.Value + " / " + (Math.Truncate(informationResolution * horizontalDistance) / informationResolution);
            EzCalc.Value = V_z.Value + " / " + (Math.Truncate(informationResolution * verticalDistance) / informationResolution);
            
            Fx.Value = -_electronCharge * (V_x / (float)(Math.Truncate(informationResolution * _electronGunLength) / informationResolution)) * (float)Math.Pow(10, 15);
            Fy.Value = -_electronCharge * (V_y / (float)(Math.Truncate(informationResolution * horizontalDistance) / informationResolution)) * (float)Math.Pow(10, 15);
            Fz.Value = -_electronCharge * (V_z / (float)(Math.Truncate(informationResolution * verticalDistance) / informationResolution)) * (float)Math.Pow(10, 15);

            FxCalc.Value = "1.6022e-19 * " + Ex.Value + " * H(" + (Math.Truncate(informationResolution * _electronGunLength) / informationResolution) + " - x)";

            size = HorizontalCapacitorTop.GetComponent<Renderer>().bounds.size.z / 2;
            length = Math.Abs(GetCRTStart().x - HorizontalCapacitor.transform.position.x);
            FyCalc.Value = "1.6022e-19 * " + Ey.Value + " * H(" + (Math.Truncate(informationResolution * size) / informationResolution) 
                           + " - abs(x - " + (Math.Truncate(informationResolution * length) / informationResolution) + "))";

            size = VerticalCapacitorLeft.GetComponent<Renderer>().bounds.size.y / 2;
            length = Math.Abs(GetCRTStart().x - VerticalCapacitor.transform.position.x);
            FzCalc.Value = "1.6022e-19 * " + Ez.Value + " * H(" + (Math.Truncate(informationResolution * size) / informationResolution) 
                           + " - abs(x - " + (Math.Truncate(informationResolution * length) / informationResolution) + "))";
        }

        public void updateData(List<Vector3> points, List<Vector3> velocities, List<Vector3> forces)
        {
            pointData = points;
            velocityData = velocities;
            forceData = forces;
        }

        public void updatePlot()
        {
            plot.ResetObject();
            float timeStep = getTimeStep();
            LineChart lineChart = plot.GetComponent<LineChart>();
            lineChart.series.GetSerie(0).maxShow = lineResolution;
            List<float> xAxisData = new List<float>();
            List<float> yAxisData = new List<float>();
            
            lineChart.xAxis0.minMaxType = Axis.AxisMinMaxType.Default;
            lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Default;

            switch (xAxis)
            {
                case 0:
                    lineChart.xAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.xAxis0.min = GetCRTStart().x;
                    lineChart.xAxis0.max = Screen.transform.position.x;
                    for (int i = 0; i < lineResolution; i++)
                        xAxisData.Add(pointData[i].x);
                    break;
                case 1:
                    lineChart.xAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.xAxis0.min = 0;
                    lineChart.xAxis0.max = lineResolution * timeStep;
                    for (int i = 0; i < lineResolution; i++)
                        xAxisData.Add(i * timeStep);
                    break;
            }

            switch (yAxis)
            {
                case 0:
                    lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.yAxis0.min = GetCRTStart().x;
                    lineChart.yAxis0.max = Screen.transform.position.x;
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(pointData[i].x);
                    break;
                case 1:
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(velocityData[i].x);
                    break;
                case 2:
                    lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.yAxis0.min = -(float)Math.Pow(10, -15);
                    lineChart.yAxis0.max = (float)Math.Pow(10, -15);
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(forceData[i].x);
                    break;
                case 3:
                    lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.yAxis0.min = Screen.transform.position.y - Screen.GetComponent<Renderer>().bounds.size.y / 2;
                    lineChart.yAxis0.max = Screen.transform.position.y + Screen.GetComponent<Renderer>().bounds.size.y / 2;
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(pointData[i].y);
                    break;
                case 4:
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(velocityData[i].y);
                    break;
                case 5:
                    lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.yAxis0.min = -(float)Math.Pow(10, -15);
                    lineChart.yAxis0.max = (float)Math.Pow(10, -15);
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(forceData[i].y);
                    break;
                case 6:
                    lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.yAxis0.min = Screen.transform.position.z - Screen.GetComponent<Renderer>().bounds.size.z / 2;
                    lineChart.yAxis0.max = Screen.transform.position.z + Screen.GetComponent<Renderer>().bounds.size.z / 2;
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(pointData[i].z);
                    break;
                case 7:
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(velocityData[i].z);
                    break;
                case 8:
                    lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
                    lineChart.yAxis0.min = -(float)Math.Pow(10, -15);
                    lineChart.yAxis0.max = (float)Math.Pow(10, -15);
                    for (int i = 0; i < lineResolution; i++)
                        yAxisData.Add(forceData[i].z);
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
            float x = 0;
            float y = 0;
            float z = 0;
            float length;
            float size;
            float dist;

            Vector3 point = new Vector3(0, 0, 0);

            point.x = -_electronCharge * (V_x / _electronGunLength) * H((GetCRTStart().x + _electronGunLength) - currentPoint.x);
            
            float scale = HorizontalCapacitorTop.GetComponent<Renderer>().bounds.size.x;
            if (HorizontalCapacitor.transform.position != new Vector3(0, 0, 0))
            {
                length = Math.Abs(GetCRTStart().x - HorizontalCapacitor.transform.position.x);
                x = (scale / 2) - Math.Abs(currentPoint.x - (GetCRTStart().x + length));
            
                dist = horizontalDistance / 2;
                y = dist - Math.Abs(currentPoint.y - GetCRTStart().y);

                size = HorizontalCapacitorTop.GetComponent<Renderer>().bounds.size.z / 2;
                z = size - Math.Abs(currentPoint.z - GetCRTStart().z);
                
                point.y = -_electronCharge * (V_y / horizontalDistance) * H(x) * H(y) * H(z);
            }

            scale = VerticalCapacitorLeft.GetComponent<Renderer>().bounds.size.x;
            if (VerticalCapacitor.transform.position != new Vector3(0, 0, 0))
            {
                length = Math.Abs(GetCRTStart().x - VerticalCapacitor.transform.position.x);
                x = (scale / 2) - Math.Abs(currentPoint.x - (GetCRTStart().x + length));
            
                size = VerticalCapacitorLeft.GetComponent<Renderer>().bounds.size.y / 2;
                y = size - Math.Abs(currentPoint.y - GetCRTStart().y);
            
                dist = verticalDistance / 2 ;
                z = dist - Math.Abs(currentPoint.z - GetCRTStart().z);
                
                point.z = -_electronCharge * (V_z / verticalDistance) * H(x) * H(y) * H(z);
            }

            return point;
        }

        public Vector3 RK4(Vector3 currentPoint)
        {
            float timeStep = getTimeStep();
            Vector3 k1 = timeStep * ApplyForce(currentPoint) / _electronMass;
            Vector3 k2_3 = timeStep * ApplyForce(currentPoint + new Vector3(timeStep / 2, 0, 0)) / _electronMass;
            Vector3 k4 = timeStep * ApplyForce(currentPoint + new Vector3(timeStep, 0, 0)) / _electronMass;
            return (k1 + 2 * k2_3 + 2 * k2_3 + k4) / 6;
        }

        public float getTimeStep()
        {
            float v = (float)Math.Sqrt(-2 * _electronCharge * V_x / _electronMass);
            float t = (float)Math.Sqrt(2 * _electronGunLength * _electronMass / (_electronCharge * (-V_x / _electronGunLength))); 
            t += GetCRTDist() / v;
            return t / lineResolution;
        }
        
        public float GetCRTDist()
        {
            return Screen.transform.position.x - GetCRTStart().x;
        }

        public Vector3 GetCRTStart()
        {
            Vector3 point = Cathode.transform.position;
            point.x += Cathode.GetComponent<Renderer>().bounds.size.x / 2;
            return point;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using GEAR.Gadgets.ReferenceValue;
using Maroon.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Maroon.Physics.CathodeRayTube
{
    public class CRTController : MonoBehaviour
    {
        [SerializeField] private GameObject Screen;
        [SerializeField] private GameObject ElectronGun;
        [SerializeField] private GameObject HorizontalCapacitor;
        [SerializeField] private GameObject VerticalCapacitor;
        [SerializeField] private QuantityInt V_x;
        [SerializeField] private QuantityInt V_y;
        [SerializeField] private QuantityInt V_z;
        [SerializeField] private QuantityFloat d; 
        public int Order { get; set; }

        private GameObject HorizontalCapacitorTop;
        private GameObject HorizontalCapacitorBottom;
        private GameObject VerticalCapacitorLeft;
        private GameObject VerticalCapacitorRight;

        private Vector3 HorizCapStartPos;
        private Vector3 VertCapStartPos;
        private Vector3 HorizCapStartScale;
        private Vector3 VertCapStartScale;
        
        private const float _electronCharge = -1.6022e-19f;
        private const float _electronMass = 9.11e-31f;
        private float _electronGunLength;
        public int lineResolution = 1000;

        private void Start()
        {
            _electronGunLength = ElectronGun.GetComponent<Renderer>().bounds.size.x / 2;
            
            HorizontalCapacitorTop = HorizontalCapacitor.transform.GetChild(0).gameObject;
            HorizontalCapacitorBottom = HorizontalCapacitor.transform.GetChild(1).gameObject;
            VerticalCapacitorLeft = VerticalCapacitor.transform.GetChild(0).gameObject;
            VerticalCapacitorRight = VerticalCapacitor.transform.GetChild(1).gameObject;
            
            HorizCapStartPos = HorizontalCapacitor.transform.position;
            VertCapStartPos = VerticalCapacitor.transform.position;
            HorizCapStartScale = HorizontalCapacitor.transform.localScale;
            VertCapStartScale = VerticalCapacitor.transform.localScale;
        }

        private void FixedUpdate()
        {
            switch (Order)
            {
                case 0:
                    HorizontalCapacitor.transform.position = HorizCapStartPos;
                    VerticalCapacitor.transform.position = VertCapStartPos;
                    HorizontalCapacitor.transform.localScale = HorizCapStartScale;
                    VerticalCapacitor.transform.localScale = VertCapStartScale;
                    break;
                
                case 1:
                    HorizontalCapacitor.transform.position = VertCapStartPos;
                    VerticalCapacitor.transform.position = HorizCapStartPos;
                    HorizontalCapacitor.transform.localScale = HorizCapStartScale;
                    VerticalCapacitor.transform.localScale = VertCapStartScale;
                    break;
                
                case 2:
                    Vector3 position = HorizCapStartPos;
                    position.x += (VertCapStartPos.x - HorizCapStartPos.x) / 2;
                    HorizontalCapacitor.transform.position = position;
                    VerticalCapacitor.transform.position = position;

                    Vector3 scale = HorizCapStartScale;
                    scale.z = d;
                    HorizontalCapacitor.transform.localScale = scale;
                    scale = VertCapStartScale;
                    scale.y = d;
                    VerticalCapacitor.transform.localScale = scale;
                    break;
                
                default:
                    HorizontalCapacitor.transform.position = HorizCapStartPos;
                    VerticalCapacitor.transform.position = VertCapStartPos;
                    HorizontalCapacitor.transform.localScale = HorizCapStartScale;
                    VerticalCapacitor.transform.localScale = VertCapStartScale;
                    break;
            }

            var newPosition = HorizontalCapacitor.transform.position;
            HorizontalCapacitorTop.transform.position = newPosition + new Vector3(0, d/2, 0);
            HorizontalCapacitorBottom.transform.position = newPosition - new Vector3(0, d/2, 0);
            newPosition = VerticalCapacitor.transform.position;
            VerticalCapacitorRight.transform.position = newPosition + new Vector3(0, 0,d/2);
            VerticalCapacitorLeft.transform.position = newPosition - new Vector3(0, 0, d/2);

        }

        public float GetVerticalDeflection()
        {
            float dist = Screen.transform.position.x - HorizontalCapacitor.transform.position.x;
            float scale = HorizontalCapacitorTop.GetComponent<Renderer>().bounds.size.x;
            return (dist * scale * V_y) / (2 * d * V_x);
        }
        
        public float GetHorizontalDeflection()
        {
            float dist = Screen.transform.position.x - VerticalCapacitor.transform.position.x;
            float scale = VerticalCapacitorLeft.GetComponent<Renderer>().bounds.size.x;
            return (dist * scale * V_z) / (2 * d * V_x);
        }
        
        public Vector3 GetContactPoint()
        {
            Vector3 screenPos = Screen.transform.position;
            Vector3 contactPoint = new Vector3
            {
                x = screenPos.x,
                y = screenPos.y + GetVerticalDeflection(),
                z = screenPos.z + GetHorizontalDeflection()
            };
            return contactPoint;
        }
        
        private float H(float x)
        {
            return x == 0 ? 0.5f : (1 + x / Math.Abs(x)) / 2;
        }

        private float ApplyForce(int cap, float currentX)
        {
            float x = 0;
            int voltage = 0;
            float plateDistance = 0;
            float scale;
            float length;

            switch (cap)
            {
                case 0:
                    voltage = V_x;
                    plateDistance = _electronGunLength;
                    x = (ElectronGun.transform.position.x + _electronGunLength) - currentX;
                    break;
                case 1:
                    voltage = V_y;
                    plateDistance = d;
                    scale = HorizontalCapacitorTop.GetComponent<Renderer>().bounds.size.x;
                    length = Math.Abs(ElectronGun.transform.position.x - HorizontalCapacitor.transform.position.x);
                    x = (scale / 2) - Math.Abs(currentX - (ElectronGun.transform.position.x + length));
                    break;
                case 2:
                    voltage = V_z;
                    plateDistance = d;
                    scale = VerticalCapacitorLeft.GetComponent<Renderer>().bounds.size.x;
                    length = Math.Abs(ElectronGun.transform.position.x - VerticalCapacitor.transform.position.x);
                    x = (scale / 2) - Math.Abs(currentX - (ElectronGun.transform.position.x + length));
                    break;
            }
            
            return (-_electronCharge) * (voltage / plateDistance) * H(x);
        }

        public float RK4(int cap, float currentX)
        {
            float timeStep = getTimeStep();
            float k1 = timeStep * ApplyForce(cap, currentX) / _electronMass;
            float k2 = timeStep * ApplyForce(cap, currentX + timeStep / 2) / _electronMass;
            float k3 = timeStep * ApplyForce(cap, currentX + timeStep / 2) / _electronMass;
            float k4 = timeStep * ApplyForce(cap, currentX + timeStep) / _electronMass;
            return (k1 + 2 * k2 + 2 * k3 + k4) / 6;
        }

        public float getTimeStep()
        {
            float v = (float)Math.Sqrt(-2 * _electronCharge * V_x / _electronMass);
            
            float t = (float)Math.Sqrt(2 * _electronGunLength * _electronMass / (_electronCharge * (-V_x / _electronGunLength))); 
            t += (GetCRTDist() - _electronGunLength) / v;
            return t / lineResolution;
        }

        public float GetCRTDist()
        {
            return Screen.transform.position.x - ElectronGun.transform.position.x;
        }

        public Vector3 GetCRTStart()
        {
            return ElectronGun.transform.position;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class PointCharge : MonoBehaviour
    {
        // Constants
        public const float RADIUS = 0.065f; // In unity units
        public const float MAX_ABSOLUTE_CHARGE = 5e-6f; // In Coulomb, current max is 1 mikro coulomb

        // Members
        private float _charge = 0.0f; // In Coulomb
        private MeshRenderer _baseMeshRenderer;
        private GameObject _fixingRing;

        private void Awake()
        {
            _baseMeshRenderer = transform.Find("Base").GetComponent<MeshRenderer>();
            _fixingRing       = transform.Find("FixingRing").gameObject;

            GetComponent<SelectableObject>().boundingRadius = RADIUS;
            GetComponent<DraggableObject>().OnDraggedOutOfBounds.AddListener((DraggableObject _unused) =>
            {
                GameObject.Destroy(this.gameObject);
            });

            // Note(MartinR): The Prefab Mesh has a radius of 1, e.g. bounds in the range [-1, 1]
            transform.localScale = new Vector3(RADIUS, RADIUS, RADIUS);

            ElectricField.Instance.pointCharges.Add(this);

            SetCharge(_charge);
        }

        private void OnDestroy() 
        { 
            ElectricField.Instance.pointCharges.Remove(this); 
        }

        public static Color ChargeValueToColor(float charge)
        {
            return Color.Lerp(Color.gray, charge < 0 ? Color.blue : Color.red, Mathf.Pow(Mathf.Abs(charge) / MAX_ABSOLUTE_CHARGE, 2));
        }

        public float GetCharge() { return _charge; }

        public void SetCharge(float newCharge)
        {
            _charge = Mathf.Clamp(newCharge, -MAX_ABSOLUTE_CHARGE, MAX_ABSOLUTE_CHARGE);
            Color color = ChargeValueToColor(_charge);

            // Change second material (Upper and lower part of the + symbol) to show + or - depending on charge
            List<Material> materials = new List<Material>();
            _baseMeshRenderer.GetMaterials(materials);
            materials[0].color = color;
            materials[2] = _baseMeshRenderer.materials[_charge < 0 ? 0 : 1];
            _baseMeshRenderer.SetMaterials(materials);
        }
    }
}

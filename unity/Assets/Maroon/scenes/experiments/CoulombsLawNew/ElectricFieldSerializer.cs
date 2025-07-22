using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public struct ChargedPointData
    {
        public ChargedPointData(
            Vector3 pos, Vector3 initialVelocity, float charge, bool createFieldLines, 
            float mass, bool hasCollision, bool lockPosition, bool contributeToEField, bool isConductive)
        {
            this.position = pos;
            this.initialVelocity = initialVelocity;
            this.charge = charge;
            this.createFieldLines = createFieldLines;
            this.mass = mass;
            this.hasCollision = hasCollision;
            this.lockPosition = lockPosition;
            this.contributeToEField = contributeToEField;
            this.isConductive = isConductive;
        }

        public Vector3 position;
        public float charge; // Coulombs
        public bool createFieldLines;

        public Vector3 initialVelocity;
        public float mass;
        public bool hasCollision;
        public bool lockPosition;
        public bool contributeToEField;
        public bool isConductive;
    }

    public struct ChargedRodData
    {
        public ChargedRodData(Vector3 pos, Vector3 direction, float chargeDensity, bool createFieldLines, bool isConductive)
        {
            this.position = pos;
            this.direction = direction;
            this.chargeDensity = chargeDensity;
            this.createFieldLines = createFieldLines;
            this.isConductive = isConductive;
        }

        public Vector3 position;
        public Vector3 direction; // normalized
        public float chargeDensity; // Coulombs/meter
        public bool createFieldLines;
        public bool isConductive;
    }

    public struct ChargedPlaneData
    {
        public ChargedPlaneData(Vector3 pos, Vector3 normal, float chargeDensity, bool createFieldLines, bool isConductive)
        {
            this.position = pos;
            this.normal = normal;
            this.chargeDensity = chargeDensity;
            this.createFieldLines = createFieldLines;
            this.isConductive = isConductive;
        }

        public Vector3 position;
        public Vector3 normal; // normalized
        public float chargeDensity; // Coulombs/meter^2
        public bool createFieldLines;
        public bool isConductive;
    }

    public struct Configuration
    {
        public ChargedPointData[] chargedPoints;
        public ChargedRodData[] chargedRods;
        public ChargedPlaneData[] chargedPlanes;
    }

    public class ElectricFieldSerializer : MonoBehaviour
    {
        public static Configuration CreateConfigurationForCurrentSetup()
        {
            Configuration result = new Configuration();

            var efield = ElectricField.Instance;
            result.chargedPoints = new ChargedPointData[efield.chargedPoints.Count];
            result.chargedRods   = new ChargedRodData[efield.chargedRods.Count];
            result.chargedPlanes = new ChargedPlaneData[efield.chargedPlanes.Count];

            Bounds box = SimulationBox.Instance.Bounds;
            for (int i = 0; i < efield.chargedPoints.Count; i++)
            {
                var pointCharge = efield.chargedPoints[i];
                result.chargedPoints[i] = new ChargedPointData(
                    pointCharge.transform.position - box.center, pointCharge.initialVelocity, pointCharge.GetCharge(), pointCharge.generateFieldLines,
                    pointCharge.GetMass(), pointCharge.GetHasCollision(), pointCharge.lockPosition, 
                    pointCharge.contributeToEField, pointCharge.isConductive);
            }
            for (int i = 0; i < efield.chargedRods.Count; i++)
            {
                var chargedRod = efield.chargedRods[i];
                result.chargedRods[i] = new ChargedRodData(
                    chargedRod.transform.position - box.center, chargedRod.GetDirection(), chargedRod.GetChargeDensity(), 
                    chargedRod.generateFieldLines, chargedRod.isConductive);
            }
            for (int i = 0; i < efield.chargedPlanes.Count; i++)
            {
                var chargedPlane = efield.chargedPlanes[i];
                result.chargedPlanes[i] = new ChargedPlaneData(
                    chargedPlane.transform.position - box.center, chargedPlane.GetNormal(), chargedPlane.GetChargeDensity(), 
                    chargedPlane.generateFieldLines, chargedPlane.isConductive);
            }
            
            return result;
        }

        // Note(MartinR): I'm passing the prefabs as parameters, as we cannot set references to the prefabs as static members in the unity inspector
        public static void RestoreConfiguration(
            Configuration configuration, Transform parentForNewObjects, 
            ChargedPoint pointChargePrefab, ChargedRod chargedRodPrefab, ChargedPlane chargedPlanePrefab)
        {
            var efield = ElectricField.Instance;

            foreach (var point in efield.chargedPoints)  { GameObject.Destroy(point.gameObject); }
            foreach (var rod in efield.chargedRods)     { GameObject.Destroy(rod.gameObject); }
            foreach (var plane in efield.chargedPlanes) { GameObject.Destroy(plane.gameObject); }
            efield.chargedPoints.Clear();
            efield.chargedRods.Clear();
            efield.chargedPlanes.Clear();

            Bounds box = SimulationBox.Instance.Bounds;
            foreach (var pointData in configuration.chargedPoints)
            {
                var newParticle = GameObject.Instantiate(pointChargePrefab, pointData.position + box.center, Quaternion.identity, parentForNewObjects);
                newParticle.SetCharge(pointData.charge);
                newParticle.generateFieldLines = pointData.createFieldLines;
                newParticle.initialVelocity = pointData.initialVelocity;
                newParticle.SetHasCollision(pointData.hasCollision);
                newParticle.SetMass(pointData.mass);
                newParticle.contributeToEField = pointData.contributeToEField;
                newParticle.lockPosition = pointData.lockPosition;
                newParticle.isConductive = pointData.isConductive;
            }
            foreach (var rodData in configuration.chargedRods)
            {
                var newRod = GameObject.Instantiate(chargedRodPrefab, rodData.position + box.center, Quaternion.identity, parentForNewObjects);
                newRod.SetRodParameters(rodData.position + box.center, rodData.direction);
                newRod.SetChargeDensity(rodData.chargeDensity);
                newRod.generateFieldLines = rodData.createFieldLines;
                newRod.isConductive = rodData.isConductive;
            }
            foreach (var planeData in configuration.chargedPlanes)
            {
                var newPlane = GameObject.Instantiate(chargedPlanePrefab, planeData.position + box.center, Quaternion.identity, parentForNewObjects);
                newPlane.SetPlaneParameters(planeData.position + box.center, planeData.normal);
                newPlane.SetChargeDensity(planeData.chargeDensity);
                newPlane.generateFieldLines = planeData.createFieldLines;
                newPlane.isConductive = planeData.isConductive;
            }
        }
    }
}

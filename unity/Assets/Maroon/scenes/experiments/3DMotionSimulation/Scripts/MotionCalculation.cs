﻿using System.Collections.Generic;
using UnityEngine;
using NCalc;
using ObjectsInUse;
using System;
using System.Linq;
using GEAR.Localization;

namespace Maroon.Physics.ThreeDimensionalMotion
{
    public class MotionCalculation : PausableObject, IResetObject
    {
        private ParticleObject _particleInUse;
        private const float Threshold = 0.001f;

        /// new members
        private Motion.Simulation simulation = null;
        private Motion.SimulatedEntity entity = null;

        private int frame = 0;

        /// <summary>
        /// Update is called every frame
        /// </summary>
        protected override void HandleUpdate()
        {

        }

        /// <summary>
        /// Handles the Play/Pause/Step functionality
        /// Play button triggers the start of the calculation and displaying results
        /// On error -> shows error message and resets the experiment
        /// </summary>
        protected override void HandleFixedUpdate()
        {
            // Create new simulation from UI Paramters
            if (simulation == null && entity == null)
            {
                Vector3 initialPosition = ParameterUI.Instance.GetXYZ();
                Vector3 initialVelocity = ParameterUI.Instance.GetVxVyVz();

                entity = new Motion.SimulatedEntity();
                entity.SetInitialState(initialPosition, initialVelocity);
                entity.AddExpression("fx", ParameterUI.Instance.GetFunctionFx());
                entity.AddExpression("fy", ParameterUI.Instance.GetFunctionFy());
                entity.AddExpression("fz", ParameterUI.Instance.GetFunctionFz());
                entity.AddExpression("m", ParameterUI.Instance.GetMass());

                simulation = new Motion.Simulation();
                simulation.t0 = ParameterUI.Instance.GetTimes().x;
                simulation.dt = ParameterUI.Instance.GetTimes().y;
                simulation.steps = (int) ParameterUI.Instance.GetTimes().z;

                simulation.AddEntity(entity);
                simulation.Run();

                Vector3 min_hack = TransformZUp(simulation.bounds.min);
                Vector3 max_hack = TransformZUp(simulation.bounds.max);

                // oh god oh god why do i have to abuse this thing like this??? 
                // i would really like to clean up this experiment but i don't know
                // if this would be out of scope....

                // this is from the previous implementation
                min_hack.x -= GetMinMaxScaleFactor(min_hack.x);
                min_hack.y -= GetMinMaxScaleFactor(min_hack.y);
                min_hack.z -= GetMinMaxScaleFactor(min_hack.z);

                if (ScalingNeeded(min_hack.x))
                    max_hack.x += GetMinMaxScaleFactor(max_hack.x);
                if (ScalingNeeded(min_hack.y))
                    max_hack.y += GetMinMaxScaleFactor(max_hack.y);
                if (ScalingNeeded(min_hack.z))
                    max_hack.z += GetMinMaxScaleFactor(max_hack.z);


                CoordSystem.Instance.SetRealCoordBorders(min_hack, max_hack);

                _particleInUse = ParameterUI.Instance.GetObjectInUse();
                CoordSystem.Instance.SetParticleActive(_particleInUse);
            }

            var step = frame % simulation.steps;
            var pos = TransformZUp(entity.Position[step]);
            var mapped_pos = CoordSystem.Instance.MapValues(pos);
            CoordSystem.Instance.DrawPoint(mapped_pos, frame < simulation.steps);

            frame++;
        }

        static Vector3 TransformZUp(Vector3 v)
        {
            return new Vector3(v.x, v.z, v.y);
        }

        /// <summary>
        /// This method checks if scaling is needed
        /// If a certain value is smaller than the threshold -> scaling leads to 
        /// incorrect visualization
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>Bool if scaling is needed</returns>
        private bool ScalingNeeded(float value)
        {
            return !(Math.Abs(value) < Threshold);
        }

        /// <summary>
        /// Method to scale min/max values for visualization
        /// </summary>
        /// <param name="value">Value to scale</param>
        /// <returns>Adapted value</returns>
        private float GetMinMaxScaleFactor(float value)
        {
            value = Math.Abs(value);
            return value == 0 ? 0.5f : 0f;
        }

        // Getter for all values
        // --------------------------------------------------------------
        public List<Vector2> GetDataX() { return entity.State.Select(s => new Vector2((float)s.Time, s.Position.x)).ToList(); }
        public List<Vector2> GetDataY() { return entity.State.Select(s => new Vector2((float)s.Time, s.Position.y)).ToList(); }
        public List<Vector2> GetDataZ() { return entity.State.Select(s => new Vector2((float)s.Time, s.Position.z)).ToList(); }

        public List<Vector2> GetDataVX() { return entity.State.Select(s => new Vector2((float)s.Time, s.Velocity.x)).ToList(); }
        public List<Vector2> GetDataVY() { return entity.State.Select(s => new Vector2((float)s.Time, s.Velocity.y)).ToList(); }
        public List<Vector2> GetDataVZ() { return entity.State.Select(s => new Vector2((float)s.Time, s.Velocity.z)).ToList(); }

        public List<Vector2> GetDataFX() { return entity.State.Select(s => new Vector2((float)s.Time, s.Force.x)).ToList(); }
        public List<Vector2> GetDataFY() { return entity.State.Select(s => new Vector2((float)s.Time, s.Force.y)).ToList(); }
        public List<Vector2> GetDataFZ() { return entity.State.Select(s => new Vector2((float)s.Time, s.Force.z)).ToList(); }

        public List<Vector2> GetDataP() { return entity.State.Select(s => new Vector2((float)s.Time, (float)s.Power)).ToList(); }
        public List<Vector2> GetDataEkin() { return entity.State.Select(s => new Vector2((float)s.Time, (float)s.KineticEnergy)).ToList(); }
        public List<Vector2> GetDataW() { return entity.State.Select(s => new Vector2((float)s.Time, (float)s.Work)).ToList(); }
        // --------------------------------------------------------------

        /// <summary>
        /// Resets the object
        /// </summary>
        public void ResetObject()
        {
            entity = null;
            simulation = null;
            frame = 0;
        }

        /// <summary>
        /// Method for showing error messages
        /// </summary>
        private void ShowError()
        {
            string message = LanguageManager.Instance.GetString("CalculationError");
            ParameterUI.Instance.DisplayMessage(message);
        }

    }
}
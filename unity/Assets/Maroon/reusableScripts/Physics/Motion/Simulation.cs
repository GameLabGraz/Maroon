﻿using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maroon.Physics.Motion
{
    public class Simulation
    {
        private double _dt;
        private double _t0;
        private double _t;
        private int _steps;
        private int current_step = 0;
        private IIntegrator integrator;
        private Bounds _bounds = new Bounds();

        private bool isInitialized = false;

        private List<SimulatedEntity> entities = new List<SimulatedEntity>();

        public double dt { get => _dt; set { _dt = value; } }
        public double t { get => _t; }
        public double t0 { get => _t0;  set { _t0 = value; } }
        public int steps { get => _steps; set { _steps = value; } }
        public Bounds bounds { get => _bounds; }

        public Simulation()
        {
            integrator = new RungeKutta4();
        }

        public Simulation(IIntegrator integrator)
        {
            this.integrator = integrator;
        }

        public Simulation(IIntegrator integrator, double dt, double t0, int steps) : this(integrator)
        {
            _dt = dt;
            _t0 = t0;
            _steps = steps;
        }

        public Simulation(IIntegrator integrator, double dt, double t0, double duration) : this(integrator)
        {
            _dt = dt;
            _t0 = t0;
            this.steps = (int) (duration / dt);
        }

        public void AddEntity(SimulatedEntity entity)
        {
            entities.Add(entity);
        }

        public void Initialize()
        {
            _t = t0;

            foreach (SimulatedEntity entity in entities)
            {
                entity.Initialize(t0, dt);
            }

            this.isInitialized = true;
        }

        public void Solve(int steps)
        {
            if (!isInitialized)
                throw new Exception("Simulation must be initialized");

            int start = current_step;

            while(current_step < (start + steps)) {
                foreach (SimulatedEntity entity in entities)
                {
                    entity.current_state = integrator.Integrate(entity.current_state, t, dt);
                    entity.SaveState();
                }

                current_step++;
                _t = current_step * dt;
            }

            foreach (SimulatedEntity entity in entities) {
                _bounds.Encapsulate(entity.Bounds);
            }

            this.steps = Math.Max(this.steps, current_step);
        }

        public void Run()
        {
            Initialize();
            Solve(this.steps);
        }
    }
}
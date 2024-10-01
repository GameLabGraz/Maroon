using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Maroon.Physics.Viscosimeter
{
    [RequireComponent(typeof(WeighableObject))]
    public class Ball : PausableObject, IResetObject
    {
        //ball variables
        private const decimal G = 9.81m;
        private decimal _velocity = 0;

        public decimal Velocity
        {
            get { return _velocity; }
            private set { _velocity = value; }
        }
        private decimal _volume = 0;

        private Vector3 start_position_;
        public decimal start_diameter_in_mm = 30.00m; //millimeter
        private bool dropped_ = true;
        private bool touching_liquid = false;


        private decimal ball_density_ = 8243.5949328270666898586m;
        private WeighableObject _weighableObject;


        private decimal radius_in_m;
        //diameter in meter
        public QuantityDecimal diameter_;
        public decimal Diameter
        {
            get => diameter_.Value;
            set
            {
                diameter_.Value = value;
                radius_in_m = diameter_.Value/2;
            }
        }

        //Weight in kg
        [FormerlySerializedAs("weight_")] public QuantityDecimal _mass;

        public decimal Mass
        {
            get => _mass.Value;
            set
            {
                _mass.Value = value;
            }
        }


        private void Awake()
        {
            //randomize start diameter a bit here
            start_diameter_in_mm = ViscosimeterManager.AddInaccuracy(start_diameter_in_mm);
      
            Mass = ViscosimeterManager.AddInaccuracy(Mass);
            _weighableObject = GetComponent<WeighableObject>();
            radius_in_m = start_diameter_in_mm / 2000m;
            _volume = CalculateVolume();
            Mass = ball_density_ * _volume;
            _weighableObject.starting_weight = Mass;
            start_position_ = transform.position;
            transform.localScale = new Vector3((float)start_diameter_in_mm/1000, (float)start_diameter_in_mm/1000, (float)start_diameter_in_mm/1000);
            _weighableObject.SetWeight(_mass);
        }

        protected override void Start()
        {
            base.Start();
      
        }

        protected override void HandleUpdate()
        {
      
        }

        protected override void HandleFixedUpdate()
        {
            ApplyFallPhysics();
        }



        private decimal CalculateVolume()
        {
            return (4.0m / 3.0m) * (decimal)Mathf.PI * radius_in_m * radius_in_m * radius_in_m;
        }



        public void ResetObject()
        {
            dropped_ = false;
            touching_liquid = false;
            Velocity = 0;

            transform.position = start_position_;
        }

        public void DropBall()
        {
            dropped_ = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == 4) //Water
            {
                touching_liquid = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            touching_liquid = false;
        }

        private void ApplyFallPhysics()
        {
            //calculate gravity force:
            decimal f_grav = Mass * G;

            if (!touching_liquid)
            {
                ApplyForce(f_grav);
                return;
            }

            decimal fluid_density = ViscosimeterManager.Instance.fluid_density_;
            float temperature = ViscosimeterManager.Instance.fluid_temperature_;
            //calculate buoyancy force:
            decimal f_buoy = 0;
            if (_velocity != 0)
            {
                f_buoy = fluid_density * _volume * G;
            }
      
            //calculate dynamic viscosity
            decimal dynamic_viscosity = ViscosimeterManager.Instance.CalculateDynamicViscosity((decimal)temperature);
      
            Debug.Log("dynamic visc " + dynamic_viscosity);
       
            //calculate viscosity friction force (stoke's law)
            decimal f_fric = 6.0m * (decimal)Math.PI * dynamic_viscosity * radius_in_m * Velocity;
      
            //calculate net force applied to ball
            decimal f_net = f_grav - (f_fric + f_buoy);
      
            ApplyForce(f_net);
        }

        private void ApplyForce(decimal force)
        {
            decimal acceleration = force / Mass;
            Vector3 current_position = transform.position;
            Velocity += acceleration * (decimal)Time.deltaTime;
            bool noCollision = CheckCollision(current_position, Velocity);

            if (noCollision)
            {
                transform.position = new Vector3(current_position.x,
                    current_position.y - (float)(Velocity * (decimal)Time.deltaTime),
                    current_position.z);
            }
        }

        private bool CheckCollision(Vector3 position, decimal velocity)
        {
            float range = (float)(velocity * (decimal)Time.deltaTime);
            Ray ray = new Ray(position, Vector3.down * range);
            Debug.DrawRay(position, Vector3.down * range);

            if (UnityEngine.Physics.Raycast(ray, out RaycastHit hit, range))
            {
                if (hit.collider.CompareTag("Floor"))
                {
                    float possibleMovement = GetPossibleMovement(ray, range);
                    transform.position = new Vector3(position.x, position.y - possibleMovement + (float)radius_in_m, position.z);
                    Velocity = 0;
                    return false;
                }
            }

            return true;
        }

        private float GetPossibleMovement(Ray ray, float range)
        {
            float new_range = range;

            while (UnityEngine.Physics.Raycast(ray, out RaycastHit hit, new_range))
            {
                new_range -= 0.001f;
            }

            return new_range;
        }


    }
}
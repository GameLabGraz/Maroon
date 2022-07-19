using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Physics
{
    public class Ball : PausableObject, IResetObject
    {
        private float start_weight_;
        private float start_radius_;
        private bool dropped_ = true;

        public QuantityFloat radius_ = 0.5f;
        public QuantityFloat weight_ = 1f;

        private Vector3 start_position_;
        //private Rigidbody rigidbody_ = this.gameObject.GetComponent<Rigidbody>();

        public float Weight
        {
            get => weight_;
            set 
            {
                weight_.Value = value;
                updateBall();
            }
        }

        public float Radius
        {
            get => radius_;
            set 
            {
                radius_.Value = value;
                updateBall();
            }
        }

        
        
        protected override void Start()
        {
            start_weight_ = weight_;
            start_radius_ = radius_;
            start_position_ = transform.position;
        }

        protected override void HandleUpdate()
        {
            if(!dropped_)
            {
                return;
            }
            Debug.Log("HandleUpdate");
            gameObject.GetComponent<Rigidbody>().AddForce(0, 0, 1);
        }

        protected override void HandleFixedUpdate()
        {

        }

        void updateBall()
        {
            float diameter = radius_ * 2;
            transform.localScale.Set(diameter, diameter, diameter);
        }

        public void ResetObject()
        {
            dropped_ = false;
            weight_ = start_weight_;
            radius_ = start_radius_;

            transform.position = start_position_;
            //rigidbody_.velocity = Vector3.zero;
        }

        public void dropBall()
        {
            dropped_ = true;
        }
    }


}

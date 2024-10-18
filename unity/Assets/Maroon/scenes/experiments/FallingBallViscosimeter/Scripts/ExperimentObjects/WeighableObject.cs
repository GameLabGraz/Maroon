using UnityEngine;


namespace Maroon.Physics.Viscosimeter
{
    public class WeighableObject : MonoBehaviour, IResetObject
    {

        public decimal starting_weight;
        private decimal weight;
        public decimal Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        private void Awake()
        {
            ResetObject();
        }

        public void ResetWeight()
        {
            weight = starting_weight;
        }

        public void ResetObject()
        {
            ResetWeight();
        }

    }
}
using UnityEngine;


namespace Maroon.Physics.Viscosimeter
{
    public class WeighableObject : MonoBehaviour, IResetObject
    {

        public decimal starting_weight;
        private decimal weight;

        private void Awake()
        {
            ResetObject();
        }

        public decimal GetWeight()
        {
            return weight;
        }

        public void SetWeight(decimal new_weight)
        {
            weight = new_weight;
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
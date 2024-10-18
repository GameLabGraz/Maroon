using UnityEngine;




namespace Maroon.Physics.Viscosimeter
{
    public class Faucet : MonoBehaviour
    {

        public SnapPoint snapPoint;
        private Pycnometer _pycnometer;

        private void OnMouseDown()
        {
            if (snapPoint != null)
            {
                _pycnometer = snapPoint.currentObject.gameObject.GetComponent<Pycnometer>();
                if (_pycnometer != null)
                {
                    if (_pycnometer.filled)
                    {
                        _pycnometer.EmptyPycnometer();
                    }
                    else
                    {
                        _pycnometer.FillPycnometer();
                    }
                }
            }
        }
    }
}
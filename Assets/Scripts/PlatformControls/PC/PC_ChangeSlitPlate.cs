using System.Collections.Generic;
using UnityEngine;

namespace PlatformControls.PC
{
    [RequireComponent(typeof(SnapSlipPlate))]
    public class PC_ChangeSlitPlate : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _slitPlates = new List<GameObject>();

        private SnapSlipPlate _snapSlipPlate;

        private void Start()
        {
            _snapSlipPlate = GetComponent<SnapSlipPlate>();
        }

        public void ChangeSlitPlate(int plateIndex)
        {
            var currentPlate = _snapSlipPlate.SnappedPlateObject;
            var platePosition = currentPlate.transform.position;
            var plateScale = currentPlate.transform.localScale;
            var plateRotation = currentPlate.transform.localRotation;

            currentPlate.SetActive(false);
            _snapSlipPlate.UnplugPlate();         

            _slitPlates[plateIndex].transform.position = platePosition;
            _slitPlates[plateIndex].transform.localScale = plateScale;
            _slitPlates[plateIndex].transform.localRotation = plateRotation;
            _slitPlates[plateIndex].SetActive(true);
        }
    }
}

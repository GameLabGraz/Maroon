using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class InsidePort : MonoBehaviour {
        [SerializeField]
        private Transform packetStartEndPosition;
        public Vector3 Position => packetStartEndPosition.position;
    }
}

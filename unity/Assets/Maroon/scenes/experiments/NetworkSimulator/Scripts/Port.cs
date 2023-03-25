using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class Port : MonoBehaviour {
        public NetworkDevice Device { get; private set; }
        public Cable Cable { get; private set; }
        public Vector3 Position => transform.position;

        public bool IsFree { get => Cable == null; }

        public void Start () {
            Device = GetComponentInParent<NetworkDevice>();
        }
    }
}

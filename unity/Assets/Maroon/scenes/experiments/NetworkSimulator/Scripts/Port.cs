using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class Port : MonoBehaviour {
        public NetworkDevice Device { get; private set; }
        public Cable Cable { get; set; }
        public Vector3 Position => transform.position;
        public Vector3 BezierPoint => transform.position + transform.forward * 0.2f;

        public bool IsFree { get => Cable == null; }

        public void Start () {
            Device = GetComponentInParent<NetworkDevice>();
        }
    }
}

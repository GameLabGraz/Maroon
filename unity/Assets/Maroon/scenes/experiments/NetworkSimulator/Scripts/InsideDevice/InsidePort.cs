using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class InsidePort : MonoBehaviour {
        [SerializeField]
        private GameObject statusRing;
        [SerializeField]
        private Transform packetStartEndPosition;
        public Vector3 Position => packetStartEndPosition.position;

        private bool connected;
        private MeshRenderer statusRingMeshRenderer;
        [SerializeField]
        private Color unconnectedColor;
        [SerializeField]
        private Color connectedColor;
        [SerializeField]
        private Color selectionColor;

        private void Start() {
            statusRingMeshRenderer = statusRing.GetComponent<MeshRenderer>();
            connected = false;
        }
        public void SetConnected(bool connected) {
            this.connected = connected;
            ResetStatus();
        }
        public void ShowSelection() {
            statusRingMeshRenderer.material.color = selectionColor;
        }
        public void ResetStatus() {
            statusRingMeshRenderer.material.color = connected ? connectedColor : unconnectedColor;
        }
    }
}

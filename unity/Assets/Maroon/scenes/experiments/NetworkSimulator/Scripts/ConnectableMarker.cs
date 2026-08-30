using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class ConnectableMarker : MonoBehaviour {
        [SerializeField]
        private Color green;
        [SerializeField]
        private Color red;
        [SerializeField]
        private MeshRenderer[] renderers;

        private void Set(Color color) {
            foreach(var renderer in renderers) {
                renderer.material.color = color;
            }
        }
        public void SetGreen() {
            Set(green);
        }
        public void SetRed() {
            Set(red);
        }
    }
}

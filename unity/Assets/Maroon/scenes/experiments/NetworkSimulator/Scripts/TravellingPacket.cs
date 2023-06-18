using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class TravellingPacket : MonoBehaviour {

        public Packet packet { get; private set; }
        public Port sender { get; private set; }
        public Port receiver { get; private set; }
        public float progress { get; set; }

        private void Start() {
            progress = 0f;
        }
        public void Initialize(Packet packet, Port sender, Port receiver) {
            this.packet = packet;
            this.sender = sender;
            this.receiver = receiver;
            transform.position = sender.Position;
            GetComponentInChildren<MeshRenderer>().material.color = packet.Color;
        }

    }
}

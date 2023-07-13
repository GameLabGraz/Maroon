using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class InsidePacket : MonoBehaviour {

        public Packet Packet { get; private set; }

        public Vector3 Position => transform.position;

        public void Initialize(Packet packet, InsidePort receiver) {
            Packet = packet;
            transform.position = receiver.Position;
            GetComponentInChildren<MeshRenderer>().material.color = packet.Color;
        }

        public void MoveTowards(Vector3 target, float maxDistanceDelta) {
            transform.position = Vector3.MoveTowards(transform.position, target, maxDistanceDelta);
        }

    }
}

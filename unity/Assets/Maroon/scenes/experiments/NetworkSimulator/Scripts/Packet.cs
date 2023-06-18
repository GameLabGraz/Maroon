using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class Packet {
        public int HopCount = 8;
        public readonly Color Color;
        public Packet() {
            Color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
        public Packet(Packet packet) {
            HopCount = packet.HopCount;
            Color = packet.Color;
        }
    }
}
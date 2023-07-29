using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class Packet {
        public int HopCount = 8;
        public readonly Color Color;
        public string SourceIPAddress;
        public string DestinationIPAddress;
        public string SourceMACAddress;
        public string DestinationMACAddress;
        public Packet() {
            Color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
        public Packet(Packet packet) {
            HopCount = packet.HopCount;
            Color = packet.Color;
        }
    }
}
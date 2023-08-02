using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class Packet {
        public int HopCount = 8;
        public readonly Color Color;
        public IPAddress SourceIPAddress;
        public IPAddress DestinationIPAddress;
        public MACAddress SourceMACAddress;
        public MACAddress DestinationMACAddress;
        public Packet(IPAddress sourceIPAddress, IPAddress destinationIPAddress, MACAddress sourceMACAddress, MACAddress destinationMACAddress) {
            SourceIPAddress = sourceIPAddress;
            DestinationIPAddress = destinationIPAddress;
            SourceMACAddress = sourceMACAddress;
            DestinationMACAddress = destinationMACAddress;
            Color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
        public Packet(Packet packet) {
            HopCount = packet.HopCount;
            Color = packet.Color;
            SourceIPAddress = packet.SourceIPAddress;
            DestinationIPAddress = packet.DestinationIPAddress;
            SourceMACAddress = packet.SourceMACAddress;
            DestinationMACAddress = packet.DestinationMACAddress;
        }
    }
}
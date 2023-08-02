using System;

namespace Maroon.NetworkSimulator {
    public readonly struct MACAddress : IEquatable<MACAddress> {
        private readonly string value;

        public MACAddress(string address) {
            value = address ?? throw new ArgumentNullException(nameof(address));
        }

        public bool Equals(MACAddress other) => value == other.value;
        public static bool operator==(MACAddress a, MACAddress b) => a.Equals(b);
        public static bool operator!=(MACAddress a, MACAddress b) => !a.Equals(b);
        public override bool Equals(object obj) => value.Equals(obj);
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value;
    }
}

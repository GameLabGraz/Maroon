using System;

namespace Maroon.NetworkSimulator {
    public readonly struct IPAddress : IEquatable<IPAddress> {
        private readonly string value;

        public IPAddress(string address) {
            value = address;
        }

        public bool Equals(IPAddress other) => value.Equals(other.value);
        public static bool operator ==(IPAddress a, IPAddress b) => a.Equals(b);
        public static bool operator !=(IPAddress a, IPAddress b) => !a.Equals(b);
        public override bool Equals(object obj) => value.Equals(obj);
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value;
    }
}

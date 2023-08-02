namespace Maroon.NetworkSimulator {
    public readonly struct AddressTableEntry<T> {
        public readonly T Value;
        public readonly int Distance;
        public AddressTableEntry(T value, int distance) {
            Value = value;
            Distance = distance;
        }
    }
}

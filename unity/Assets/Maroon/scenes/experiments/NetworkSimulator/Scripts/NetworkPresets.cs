using UnityEngine;

namespace Maroon.NetworkSimulator {
    public static class NetworkPresets {
        public static Preset[] Presets = new[] {
            new Preset(
                new[] {
                    new DevicePreset(NetworkDevice.DeviceType.Computer, -0.67f, 0),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 0.48f, 0.3f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 1.02f, -0.2f),
                    new DevicePreset(NetworkDevice.DeviceType.Hub, -0.13f, -0.38f)
                },
                new[] {
                    (0, 3),
                    (1, 3),
                    (2, 3)
                }
            ),
            new Preset(
                new[] {
                    new DevicePreset(NetworkDevice.DeviceType.Computer, -0.67f, 0),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 0.48f, 0.3f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 1.02f, -0.2f),
                    new DevicePreset(NetworkDevice.DeviceType.Switch, -0.13f, -0.38f)
                },
                new[] {
                    (0, 3),
                    (1, 3),
                    (2, 3)
                }
            ),
            new Preset(
                new[] {
                    new DevicePreset(NetworkDevice.DeviceType.Computer, -0.67f, 0),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 0.48f, 0.3f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 1.02f, -0.2f),
                    new DevicePreset(NetworkDevice.DeviceType.Router, -0.13f, -0.38f)
                },
                new[] {
                    (0, 3),
                    (1, 3),
                    (2, 3)
                }
            ),
            new Preset(
                new[] {
                    new DevicePreset(NetworkDevice.DeviceType.Computer, -0.72f, -0.15f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, -0.43f, 0.4f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 0.75f, 0.35f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 1.01f, -0.22f),
                    new DevicePreset(NetworkDevice.DeviceType.Hub, -0.29f, -0.25f),
                    new DevicePreset(NetworkDevice.DeviceType.Hub, 0.22f, -0.23f),
                    new DevicePreset(NetworkDevice.DeviceType.Switch, 0, -0.5f)
                },
                new[] {
                    (0, 4),
                    (1, 4),
                    (4, 6),
                    (5, 6),
                    (2, 5),
                    (3, 5)
                }
            ),
            new Preset(
                new[] {
                    new DevicePreset(NetworkDevice.DeviceType.Computer, -0.72f, -0.15f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, -0.43f, 0.4f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 0.75f, 0.35f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 1.01f, -0.22f),
                    new DevicePreset(NetworkDevice.DeviceType.Switch, -0.29f, -0.25f),
                    new DevicePreset(NetworkDevice.DeviceType.Switch, 0.22f, -0.23f),
                    new DevicePreset(NetworkDevice.DeviceType.Router, 0, -0.5f)
                },
                new[] {
                    (0, 4),
                    (1, 4),
                    (4, 6),
                    (5, 6),
                    (2, 5),
                    (3, 5)
                }
            ),
            new Preset(
                new[] {
                    new DevicePreset(NetworkDevice.DeviceType.Computer, -1f, 0.09f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, -0.7f, 0.58f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 0.6f, 0.6f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 1.02f, 0.14f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, -0.37f, -0.63f),
                    new DevicePreset(NetworkDevice.DeviceType.Computer, 0.37f, -0.63f),
                    new DevicePreset(NetworkDevice.DeviceType.Switch, -0.55f, 0.15f),
                    new DevicePreset(NetworkDevice.DeviceType.Switch, 0.26f, 0.13f),
                    new DevicePreset(NetworkDevice.DeviceType.Switch, -0.17f, -0.17f)
                },
                new[] {
                    (0, 6),
                    (1, 6),
                    (4, 8),
                    (6, 8),
                    (7, 8),
                    (6, 7),
                    (2, 7),
                    (3, 7),
                    (5, 8)
                }
            ),
        };
        public readonly struct Preset {
            public readonly DevicePreset[] Devices;
            public readonly (int, int)[] Cables;

            public Preset(DevicePreset[] devices, (int, int)[] cables) {
                Devices = devices;
                Cables = cables;
            }
        }
        public readonly struct DevicePreset {
            public readonly NetworkDevice.DeviceType Type;
            public readonly Vector3 Position;

            public DevicePreset(NetworkDevice.DeviceType type, float x, float z) {
                Type = type;
                Position = new Vector3(x, -0.085f, z);
            }
        }
    }
}

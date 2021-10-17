using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts
{
    public enum MoleculeType
    {
        CO,
        CO2,
        O2
    }

    public class Molecule : MonoBehaviour
    {
        [SerializeField] MoleculeType type;
        
        public MoleculeType Type { get { return type; } }
    }
}
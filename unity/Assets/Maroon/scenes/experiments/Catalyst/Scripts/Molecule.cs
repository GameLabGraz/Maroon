using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts
{
    public enum MoleculeType
    {
        CO,
        CO2,
        O,
        O2,
        Pt
    }

    public class Molecule : MonoBehaviour
    {
        [SerializeField] MoleculeType type;
        [SerializeField] List<MoleculeType> canConnectToList = new List<MoleculeType>();

        [Header("Molecule Movement")]
        [SerializeField] bool isFixedMolecule;
        [SerializeField] float movementSpeed;
        
        
        public MoleculeType Type { get => type; } }
}
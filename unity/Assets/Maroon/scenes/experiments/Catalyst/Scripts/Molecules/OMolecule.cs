using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public class OMolecule : Molecule
    {
        private SphereCollider _collider;
        private float _timeToIncreaseCollider = 2.0f;
        private float _currentTime = 0.0f;

        protected override void Start()
        {
            base.Start();
            _collider = GetComponent<SphereCollider>();
        }

        protected override void HandleFixedUpdate()
        {
            if (State == MoleculeState.DrawnByCO)
            {
                // handle drawing to CO molecule
                base.HandleFixedUpdate();
            }
            else
            {
                // gradually increase drawing collider
                _currentTime += Time.deltaTime;
                if (_currentTime >= _timeToIncreaseCollider && _collider.radius <= 4)
                {
                    _collider.radius *= 2;
                    _currentTime = 0.0f;
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (State == MoleculeState.DrawnByPlat || State == MoleculeState.DrawnByCO) return;
            if (ConnectedMolecule == null) // draw O atoms to nearby CO molecules
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && otherMolecule.Type == MoleculeType.CO &&
                    otherMolecule.State == MoleculeState.Fixed &&
                    otherMolecule.ConnectedMolecule != null && otherMolecule.ConnectedMolecule.Type == MoleculeType.Pt)
                {
                    SetMoleculeDrawn(otherMolecule, MoleculeState.DrawnByCO);
                    otherMolecule.ConnectedMolecule = this;
                    ActivateDrawingCollider(false);
                    otherMolecule.ActivateDrawingCollider(false);
                }
            }
        }
    }
}
namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public class CobaltMolecule : Molecule
    {
        protected override void ReactionStart_Impl()
        {
            if (IsTopLayerSurfaceMolecule)
                ActivateDrawingCollider(true);
        }
    }
}
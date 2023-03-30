using UnityEditor;
using GEAR.Gadgets.ReferenceValue.Editor;

namespace Maroon.Physics.Editor
{
    [CustomPropertyDrawer(typeof(QuantityReferenceValue))]
    public class QuantityReferenceValuePropertyDrawer : ReferenceValuePropertyDrawer<IQuantity>
    {
    }
}
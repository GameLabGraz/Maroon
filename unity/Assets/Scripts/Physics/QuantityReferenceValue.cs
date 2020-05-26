using System;
using GEAR.Gadgets.ReferenceValue;
using GEAR.Gadgets.ReferenceValue.Editor;

namespace Maroon.Physics
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomPropertyDrawer(typeof(QuantityReferenceValue))]
    public class QuantityReferenceValuePropertyDrawer : ReferenceValuePropertyDrawer<IQuantity> { }
#endif

    [Serializable] public class QuantityReferenceValue : ReferenceValue<IQuantity> { }
}
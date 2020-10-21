using System;
using GEAR.Gadgets.ReferenceValue;

namespace Maroon.Physics
{
#if UNITY_EDITOR
    using UnityEditor;
    using GEAR.Gadgets.ReferenceValue.Editor;


    [CustomPropertyDrawer(typeof(QuantityReferenceValue))]
    public class QuantityReferenceValuePropertyDrawer : ReferenceValuePropertyDrawer<IQuantity> { }
#endif

    [Serializable] public class QuantityReferenceValue : ReferenceValue<IQuantity> { }
}
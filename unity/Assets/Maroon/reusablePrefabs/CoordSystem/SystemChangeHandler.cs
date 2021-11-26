using System;
using UnityEngine;

namespace Maroon.Physics.CoordinateSystem
{
/*
 * Currently doing nothing
 */

    public class SystemChangeHandler : MonoBehaviour
    {
        private static SystemChangeHandler _instance;

        public static SystemChangeHandler Instance
        {
            get
            {
                if (_instance is null)
                    _instance = FindObjectOfType<SystemChangeHandler>();
                return _instance;

            }
        }

        public event Action<float> OnUniformWorldLength;
        public event Action<float> OnFontSizeChange;

        public void FontSizeChanged(float value)
        {
            OnFontSizeChange?.Invoke(value);
        }

        public void UniformWorldLengthChanged(float value)
        {
            OnUniformWorldLength?.Invoke(value);
        }

        /*public event Action<Axis, int> onAxisSubdivisionChange;
        public event Action<Axis, float> onWorldSpaceAxisScaleChange;
        public event Action<Axis, float> onLocalSpaceAxisLengthChange;
        public event Action onEnableThirdDimensionChange;
        public event Action<Axis> onEnableNegativeDirectionChange;
    
        public void AxisSubdivisionChange(Axis id, int value)
        {
            onAxisSubdivisionChange?.Invoke(id, value);
        }
    
        public void WorldSpaceAxisScaleChange(Axis id, float value)
        {
            onWorldSpaceAxisScaleChange?.Invoke(id, value);
        }
    
        public void EnableThirdDimensionChange()
        {
            onEnableThirdDimensionChange?.Invoke();
        }
    
        public void LocalSpaceAxisLengthChange(Axis id, float value)
        {
            onLocalSpaceAxisLengthChange?.Invoke(id, value);
        }
    
        public void EnableNegativeDirectionChange(Axis id)
        {
            onEnableNegativeDirectionChange?.Invoke(id);
        }*/
    }
}

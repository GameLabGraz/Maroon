using System.Globalization;
using Maroon.UI.ValueDisplay;
using UnityEngine;

public class QuantityRad2DegFormat : QuantityDisplayFormat
{
    protected override string FormatInt(int rad)
    {
        var deg = rad * Mathf.Rad2Deg;
        return deg.ToString("F2", CultureInfo.InvariantCulture);
    }
    protected override string FormatFloat(float rad)
    {
        var deg = rad * Mathf.Rad2Deg;
        return deg.ToString("F2", CultureInfo.InvariantCulture);
    }
}

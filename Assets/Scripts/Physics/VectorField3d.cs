//-----------------------------------------------------------------------------
// VectorField.cs
//
// Class for the vector field to represent the field
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class for the vector field to represent the field
/// </summary>
public class VectorField3d : VectorField
{
    /// <summary>
    /// The vector field depth
    /// </summary>
    private float depth;
    
    /// <summary>
    /// Initialization
    /// </summary>
    protected void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

        height = GetComponent<MeshFilter>().mesh.bounds.size.z;
        depth = GetComponent<MeshFilter>().mesh.bounds.size.y;
        width = GetComponent<MeshFilter>().mesh.bounds.size.x;
        
        createVectorFieldArrows();
    }

    private void OnMouseDown()
    {
        Debug.Log("Vector Field Mouse Down");
    }

    /// <summary>
    /// Creates the vector field arrows and move them to the right position depending on the resolution.
    /// </summary>
    protected override void createVectorFieldArrows()
    {
        if (simController == null)
        {
            Start();
            return;
        }
        
        float arrow_scale;
        if (resolution > 20)
            arrow_scale = 0.1f - (resolution - 20) * 0.05f;
        else
            arrow_scale = 0.1f + (20 - resolution) * 0.05f;
        arrow_scale *= 0.5f;

        var widthHalf = -width / 2;
        var widthFactor = width / resolution;
        var heightHalf = -height / 2;
        var heightFactor = height / resolution;
        var depthHalf = -depth / 2;        
        var depthFactor = depth / resolution;
        
        for (var i = 0; i < resolution; i++)
        {
            for (var j = 0; j < resolution; j++)
            {
                for (var k = 0; k < resolution; k++)
                {
                    var x = widthHalf + widthFactor * (0.5f + i);
                    var y = heightHalf + heightFactor * (0.5f + j);
                    var z = depthHalf + depthFactor * (0.5f + k);

                    var arrow = Instantiate(arrowPrefab, transform, true) as GameObject;
                    arrow.GetComponent<ArrowController>().fieldStrengthFactor = this.fieldStrengthFactor;
                    arrow.GetComponent<ArrowController>().Allow3dRotation = true;
                    
                    arrow.transform.localScale = Vector3.Scale(new Vector3(arrow_scale, arrow_scale, arrow_scale),
                                                     transform.localScale) / 3;
                    arrow.transform.localPosition = new Vector3(x, z, y);
                    
                    simController.AddNewResetObject(arrow.GetComponent<IResetObject>());
                    vectorFieldArrows.Add(arrow);
                }
            }
        }
    }
}

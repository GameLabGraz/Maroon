using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class WallManager : MonoBehaviour
    {
        private Plane[] _walls = new Plane[6];  // 6 walls to define a finite volume for the laboratory

        void Start()
        {
            _walls[0] = new Plane(Vector3.down,   new Vector3(2.0f, 2.0f, 1.0f)); // Bottom wall
            _walls[1] = new Plane(Vector3.up,     new Vector3(2.0f, 0.0f, 1.0f)); // Top wall
            
            _walls[2] = new Plane(Vector3.right,   new Vector3(-0.5f, 1.0f, 1.0f));// Left wall
            _walls[3] = new Plane(Vector3.left,    new Vector3(4.5f, 1.0f, 1.0f)); // Right wall
            
            _walls[4] = new Plane(Vector3.forward, new Vector3(2.0f, 1.0f, 2.5f)); // Back wall
            _walls[5] = new Plane(Vector3.back,    new Vector3(2.0f, 1.0f, -0.5f));// Front wall

            foreach (var wall in _walls)
            {
                wall.Translate(Constants.TableBaseOffset);  // Set origin to table left-lower corner
            }

        }
        
    }
}

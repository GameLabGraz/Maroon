using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class WallManager : MonoBehaviour
    {
        public static WallManager Instance;
        private Wall[] _walls = new Wall[6];  // 6 walls to define a finite volume for the laboratory

        public Wall[] Walls => _walls;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed WallManager");
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            _walls[0] = new Wall(new Vector3(2.0f, 2.0f, 1.0f) + Constants.TableBaseOffset, Vector3.down);    // Bottom wall
            _walls[1] = new Wall(new Vector3(2.0f, 0.0f, 1.0f) + Constants.TableBaseOffset, Vector3.up);      // Top wall
            
            _walls[2] = new Wall(new Vector3(-0.5f, 1.0f, 1.0f) + Constants.TableBaseOffset, Vector3.right);  // Left wall
            _walls[3] = new Wall(new Vector3(4.5f, 1.0f, 1.0f) + Constants.TableBaseOffset, Vector3.left);    // Right wall
            
            _walls[4] = new Wall(new Vector3(2.0f, 1.0f, 2.5f) + Constants.TableBaseOffset, Vector3.forward); // Back wall
            _walls[5] = new Wall(new Vector3(2.0f, 1.0f, -0.5f) + Constants.TableBaseOffset, Vector3.back);   // Front wall
        }

        public struct Wall
        {
            public Vector3 p0;
            public Vector3 n;

            public Wall(Vector3 p0, Vector3 n)
            {
                this.p0 = p0;
                this.n = n;
            }
        }
        
    }
}

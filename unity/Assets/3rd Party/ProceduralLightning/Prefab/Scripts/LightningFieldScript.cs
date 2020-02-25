//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;

namespace DigitalRuby.ThunderAndLightning
{
    public class LightningFieldScript : LightningBoltPrefabScriptBase
    {
        [Tooltip("The bounds to put the field in.")]
        public Bounds FieldBounds;

        [Tooltip("Optional light for the lightning field to emit")]
        public Light Light;

        private Vector3 RandomPointInBounds()
        {
            float x = UnityEngine.Random.Range(FieldBounds.min.x, FieldBounds.max.x);
            float y = UnityEngine.Random.Range(FieldBounds.min.y, FieldBounds.max.y);
            float z = UnityEngine.Random.Range(FieldBounds.min.z, FieldBounds.max.z);

            return new Vector3(x, y, z);
        }

        protected override void Start()
        {
            base.Start();

            if (Light != null)
            {
                Light.enabled = false;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Light != null)
            {
                Light.transform.position = FieldBounds.center;
                Light.intensity = UnityEngine.Random.Range(2.8f, 3.2f);
            }
        }

        public override void CreateLightningBolt(LightningBoltParameters parameters)
        {
            // get two random points in the bounds
            parameters.Start = RandomPointInBounds();
            parameters.End = RandomPointInBounds();

            if (Light != null)
            {
                Light.enabled = true;
            }

            base.CreateLightningBolt(parameters);
        }
    }
}

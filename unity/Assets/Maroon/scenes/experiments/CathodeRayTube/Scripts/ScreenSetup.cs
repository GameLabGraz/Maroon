using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.CathodeRayTube
{
    public class ScreenSetup : PausableObject, IResetObject
    {
        [SerializeField] private GameObject pixel;
        private readonly List<GameObject> _pixelList = new List<GameObject>();

        private new void Start()
        {
            var screenTransform = transform;
            Vector3 screenPosition = screenTransform.position;
            Vector3 screenScale = screenTransform.localScale;


            var localScale = pixel.transform.localScale;
            float pixelHeight = localScale.y;
            float pixelWidth = localScale.z;

            for (float y = screenPosition.y - screenScale.y / 2 + pixelHeight;
                 y <= screenPosition.y + screenScale.y / 2 - pixelHeight;
                 y += pixelHeight)
            {
                for (float z = screenPosition.z - screenScale.z / 2 + pixelWidth;
                     z <= screenPosition.z + screenScale.z / 2;
                     z += pixelWidth)
                {
                    GameObject tmpPixel = Instantiate(this.pixel, new Vector3(screenPosition.x, y, z),
                        this.pixel.transform.rotation);
                    tmpPixel.transform.name = "pixel_" + y + "_" + z;
                    tmpPixel.GetComponent<MeshRenderer>().enabled = false;
                    _pixelList.Add(tmpPixel);
                }
            }
        }

        protected override void HandleUpdate()
        {
        }

        protected override void HandleFixedUpdate()
        {
        }

        public void ActivatePixel(Vector3 contactPoint)
        {
            if (!GetComponent<Collider>().bounds.Contains(contactPoint))
                return;

            GameObject closestPixel = _pixelList[0];
            float closestDistance = float.MaxValue;

            foreach (var tmpPixel in _pixelList)
            {
                float tempDistance = Vector3.Distance(contactPoint, tmpPixel.transform.position);
                if (!(tempDistance < closestDistance))
                    continue;
                closestDistance = tempDistance;
                closestPixel = tmpPixel;
            }

            closestPixel.GetComponent<MeshRenderer>().enabled = true;
        }

        public void ResetObject()
        {
            foreach (var tmpPixel in _pixelList)
            {
                tmpPixel.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
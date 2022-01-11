﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using VRTK;

namespace Maroon.Physics.CathodeRayTube
{
    public class ScreenSetup : PausableObject, IResetObject
    {
        private CRTController _crtController;
        
        [SerializeField] private GameObject Pixel;
        private List<GameObject> pixelList = new List<GameObject>();
        
        void Start()
        {
            _crtController = transform.parent.gameObject.GetComponent<CRTController>();
            
            var screenTransform = transform;
            Vector3 screenPosition = screenTransform.position;
            Vector3 screenScale = screenTransform.localScale;


            var localScale = Pixel.transform.localScale;
            float pixelHeight = localScale.y;
            float pixelWidth = localScale.z;

            for (float y = screenPosition.y - screenScale.y / 2 + pixelHeight;
                y <= screenPosition.y + screenScale.y / 2;
                y += pixelHeight)
            {
                for (float z = screenPosition.z - screenScale.z / 2 + pixelWidth;
                    z <= screenPosition.z + screenScale.z / 2;
                    z += pixelWidth)
                {
                    GameObject pixel = Instantiate(Pixel, new Vector3(screenPosition.x, y, z),
                        Pixel.transform.rotation);
                    pixel.transform.name = "pixel_" + y + "_" + z;
                    pixel.GetComponent<MeshRenderer>().enabled = false;
                    pixelList.Add(pixel);
                }
            }
        }

        protected override void HandleUpdate()
        {

        }

        protected override void HandleFixedUpdate()
        {
            ActivatePixel(_crtController.GetContactPoint());
        }

        private void ActivatePixel(Vector3 contactPoint)
        {
            if (!GetComponent<Collider>().bounds.Contains(contactPoint))
                return;
            
            GameObject closestPixel = pixelList[0];
            float closestDistance = float.MaxValue;

            foreach (var pixel in pixelList)
            {
                float tempDistance = Vector3.Distance(contactPoint, pixel.transform.position);
                if (!(tempDistance < closestDistance)) continue;
                closestDistance = tempDistance;
                closestPixel = pixel;
            }

            closestPixel.GetComponent<MeshRenderer>().enabled = true;
        }
        
        public void ResetObject()
        {
            foreach (var pixel in pixelList)
            {
                pixel.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
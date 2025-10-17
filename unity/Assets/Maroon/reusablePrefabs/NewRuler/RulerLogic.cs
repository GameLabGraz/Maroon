using System;
using Maroon.Physics;
using Maroon.Physics.CoordinateSystem;
using UnityEngine;
using Maroon.GlobalEntities;
using Maroon.Utility;

namespace Maroon.Tools.Ruler
{
    public class RulerLogic : MonoBehaviour
    {
        [Header("UI-Element References")]
        [SerializeField] private TMPro.TMP_Text distanceText;
        [SerializeField] private UIPositionDisplay positionDisplayStartPin;
        [SerializeField] private UIPositionDisplay positionDisplayEndPin;
        [SerializeField] private UIItemDragHandlerSimple uiStartPinDragHandler;
        [SerializeField] private UIItemDragHandlerSimple uiEndPinDragHandler;

        [Header("Game-Object References")]
        [SerializeField] private Transform startPin;
        [SerializeField] private Transform endPin;
        [SerializeField] private PC_DragHandler startPinDragHandler;
        [SerializeField] private PC_DragHandler endPinDragHandler;
        public LineRenderer lineRenderer;
       
        private void Awake()
        {
            // Note(MartinR): Pins have a weird starting position (Y=51.7m), I guess because they are children of some UI-components,
            //      which may have weird transforms for 3D objects,
            //      so I set the position manually to zero here (Pins are disabled at startup anyways, this only affects the UI)
            startPin.transform.position = Vector3.zero;
            endPin.transform.position = Vector3.zero;

            // Set UI-Callbacks
            positionDisplayStartPin.affectedObject = startPin;
            positionDisplayEndPin.affectedObject = endPin;

            uiStartPinDragHandler.OnDragFinished.AddListener((Vector3 pos) =>
            {
                if (startPin == null) return;
                startPin.gameObject.SetActive(true);
                startPin.position = pos;
            });
            uiEndPinDragHandler.OnDragFinished.AddListener((Vector3 pos) =>
            {
                if (endPin == null) return;
                endPin.gameObject.SetActive(true);
                endPin.position = pos;
            });

            startPinDragHandler.onEndMovingOutsideBoundaries.AddListener(() => startPin.gameObject.SetActive(false));
            endPinDragHandler.onEndMovingOutsideBoundaries.AddListener(() => endPin.gameObject.SetActive(false));
        }

        public void LateUpdate()
        {
            // Early exit if pins are not set
            if (!startPin.gameObject.activeSelf || !endPin.gameObject.activeSelf)
            {
                distanceText.text = "---";
                lineRenderer.enabled = false;
                return;
            }

            // Calculate and update distance text
            float distance = 
                (CoordSystemHandler.Instance.GetSystemPosition(startPin.position, Unit.m) - 
                CoordSystemHandler.Instance.GetSystemPosition(endPin.position, Unit.m)).magnitude;
            SI_Prefix distancePrefix = SI_Prefix_Helpers.GetClosestPrefix(distance);
            distanceText.text = string.Format("{0:0.000}{1}m", distance / distancePrefix.GetFactor(), distancePrefix.GetAbbreviation());

            // Update line renderer
            lineRenderer.SetPosition(0, startPin.position);
            lineRenderer.SetPosition(1, endPin.position);
            lineRenderer.enabled = true;
        }
    }
}

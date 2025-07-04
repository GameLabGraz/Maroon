using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class GuiIconTo3DObjectDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public UnityEngine.Events.UnityEvent<Vector3> OnDragFinished;

        private GameObject _placeholderObject; // The 2D object used as placeholder until the drag has finished
        private Canvas _parentCanvas = null;

        private void Awake()
        {
            _parentCanvas = GetComponentInParent<Canvas>();
            Debug.Assert(_parentCanvas != null, "GuiIconTo3DObjectDrag should only be used on UI-objects");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _placeholderObject = Instantiate(gameObject, _parentCanvas.transform);
            var recTransformOrig = gameObject.GetComponent<RectTransform>();
            var recTransform = _placeholderObject.GetComponent<RectTransform>();
            recTransform.sizeDelta = recTransformOrig.sizeDelta;
            recTransform.localScale = recTransformOrig.localScale;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var screenPoint = Input.mousePosition;
            var finish = _parentCanvas.worldCamera.ScreenToWorldPoint(screenPoint);
            finish.z = 0f;
            _placeholderObject.transform.position = finish;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Destroy(_placeholderObject);

            var simBox = SimulationBox.Instance.Bounds;
            Vector3 dragEndPoint = CameraController.GetMousePointOnPlaneParallelToCamera(simBox.center);

            // Early exit if we placed something out of bounds
            if (!simBox.Contains(dragEndPoint)) return;

            // Invoke callback (Which usually places objects in 3D space)
            OnDragFinished.Invoke(dragEndPoint);
        }
    }
}

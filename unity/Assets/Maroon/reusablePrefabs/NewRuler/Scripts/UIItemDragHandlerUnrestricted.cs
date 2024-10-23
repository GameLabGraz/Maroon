using System;
using System.Linq;
using UnityEngine.Events;

namespace Maroon.Tools.Ruler
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UIItemDragHandlerUnrestricted : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Canvas parentCanvas;
        [SerializeField] private GameObject generatedObject;

        [Header("Boundary Settings")]
        [SerializeField] private bool isUsingBoundaries;
        [SerializeField] private Transform minBoundary;
        [SerializeField] private Transform maxBoundary;
        [SerializeField] private bool is2D = false;

        private GameObject _item;
        private bool _objectInsideBoundaries;
       
        public void OnBeginDrag(PointerEventData eventData)
        {
            _item = Instantiate(gameObject, parentCanvas.transform);
         
            var recTransformOrig = gameObject.GetComponent<RectTransform>();
            var recTransform = _item.GetComponent<RectTransform>();

            recTransform.sizeDelta = recTransformOrig.sizeDelta;
            recTransform.localScale = recTransformOrig.localScale;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var screenPoint = Input.mousePosition;
            var finish = screenPoint;
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (is2D)
                finish.z = 0f;

            _objectInsideBoundaries = CheckIfObjectIsInBounds();
            _item.transform.position = finish;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 100f);

            if (_objectInsideBoundaries)
            {
                if(hits.Length != 0) 
                    ShowObject(hits.First().point, hits.First().transform.parent);
            }
            
            Destroy(_item);
        }
        
        protected virtual void ShowObject(Vector3 position, Transform parent)
        {
            if (generatedObject == null) return;

            generatedObject.transform.position = position;
            generatedObject.SetActive(true);
        }

        private bool CheckIfObjectIsInBounds()
        {

            if (maxBoundary != null && minBoundary != null)
            {
                var maxBoundPosition = maxBoundary.transform.position;
                var minBoundPosition = minBoundary.transform.position;
                var ray = Camera.main.ScreenPointToRay(_item.transform.position);
                var pt = ray.GetPoint(20);

                if (pt.x >= Mathf.Max(minBoundPosition.x, maxBoundPosition.x) || pt.x <= Mathf.Min(minBoundPosition.x, maxBoundPosition.x))
                    return false;
                if (pt.y >= Mathf.Max(minBoundPosition.y, maxBoundPosition.y) || pt.y <= Mathf.Min(minBoundPosition.y, maxBoundPosition.y))
                    return false;
                if (!is2D && _item.transform.position.z >= Mathf.Max(minBoundPosition.z, maxBoundPosition.z))
                    return false;
            }

            return true;
        }
    }
}

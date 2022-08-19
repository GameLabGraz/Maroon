
namespace Maroon.Tools.Ruler
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UIItemDragHandlerUnrestricted : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Canvas parentCanvas;
        public GameObject generatedObject;
        
        private GameObject _item;
        [SerializeField] bool is2D = false;

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
            var finish = parentCanvas.worldCamera.ScreenToWorldPoint(screenPoint);
            
            if (is2D)
                finish.z = 0f;

            _item.transform.position = finish;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 1000f);

            foreach (var hit in hits)
            {
                ShowObject(hit.point, hit.transform.parent);
            }

            Destroy(_item);
        }

        protected virtual void ShowObject(Vector3 position, Transform parent)
        {
            if (generatedObject == null) return;

            generatedObject.transform.position = position;
            generatedObject.SetActive(true);
        }
    }
}

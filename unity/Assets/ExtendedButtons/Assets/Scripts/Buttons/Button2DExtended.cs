using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExtendedButtons
{
    [RequireComponent(typeof(ICanvasElement))]
    public class Button2DExtended : Button2D, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public UnityEvent onBeginDrag;
        public UnityEvent onDrag;
        public UnityEvent onEndDrag;


        public override void Init()
        {
            base.Init();

            if (onBeginDrag == null)
                onBeginDrag = new UnityEvent();
            if (onDrag == null)
                onDrag = new UnityEvent();
            if (onEndDrag == null)
                onEndDrag = new UnityEvent();
        }
        
        protected override void Awake()
        {
            Init();
        }

        protected Button2DExtended() { }
        

        //public override void OnPointerDown(PointerEventData eventData)
        //{
        //    if (!interactable) return;
        //    base.OnPointerDown(eventData);
        //    onDown?.Invoke();
        //}

        //public override void OnPointerUp(PointerEventData eventData)
        //{
        //    if (!interactable) return;
        //    base.OnPointerUp(eventData);
        //    onUp?.Invoke();
        //}

        //public override void OnPointerEnter(PointerEventData eventData)
        //{
        //    if (!interactable) return;
        //    base.OnPointerEnter(eventData);
        //    onEnter?.Invoke();
        //}

        //public override void OnPointerExit(PointerEventData eventData)
        //{
        //    if (!interactable) return;
        //    base.OnPointerExit(eventData);
        //    onExit?.Invoke();
        //}

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!interactable) return;
            onBeginDrag?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!interactable) return;
            onDrag?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!interactable) return;
            onEndDrag?.Invoke();
        }
    }
}

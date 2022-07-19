using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExtendedButtons
{
    [RequireComponent(typeof(ICanvasElement))]
    public class Button2D : Button
    {
        public UnityEvent onEnter;
        public UnityEvent onDown;
        public UnityEvent onUp;
        public UnityEvent onExit;

        protected bool isInit = false;

        public virtual void Init()
        {
            if (isInit) return;
            isInit = true;

            if (onEnter == null)
                onEnter = new UnityEvent();
            if (onDown == null)
                onDown = new UnityEvent();
            if (onUp == null)
                onUp = new UnityEvent();
            if (onExit == null)
                onExit = new UnityEvent();
        }

        protected override void Awake()
        {
            Init();
        }

        protected Button2D() { }


        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable) return;
            base.OnPointerDown(eventData);
            onDown?.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!interactable) return;
            base.OnPointerUp(eventData);
            onUp?.Invoke();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!interactable) return;
            base.OnPointerEnter(eventData);
            onEnter?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!interactable) return;
            base.OnPointerExit(eventData);
            onExit?.Invoke();
        }
    }
}

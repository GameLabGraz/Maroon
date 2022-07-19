using UnityEngine;
using UnityEngine.Events;

namespace ExtendedButtons
{
    [RequireComponent(typeof(Collider))]
    public class Button3D : MonoBehaviour
    {
        public bool Interactable { get; set; } = true;

        public UnityEvent onEnter;
        public UnityEvent onDown;
        public UnityEvent onUp;
        public UnityEvent onClick;
        public UnityEvent onExit;

        public UnityEvent onBeginDrag;
        public UnityEvent onDrag;
        public UnityEvent onEndDrag;

        protected bool isInit = false;

        public void Init()
        {
            if (isInit) return;
            isInit = true;

            if (onClick == null)
                onClick = new UnityEvent();
            if (onDown == null)
                onDown = new UnityEvent();
            if (onUp == null)
                onUp = new UnityEvent();
            if (onEnter == null)
                onEnter = new UnityEvent();
            if (onExit == null)
                onExit = new UnityEvent();

            if (onBeginDrag == null)
                onBeginDrag = new UnityEvent();
            if (onDrag == null)
                onDrag = new UnityEvent();
            if (onEndDrag == null)
                onEndDrag = new UnityEvent();
        }

        protected void Awake()
        {
            Init();
        }

        //[System.Serializable]
        //public class ButtonEvent : UnityEvent
        //{
        //    public ButtonEvent() { }
        //}
    }
}

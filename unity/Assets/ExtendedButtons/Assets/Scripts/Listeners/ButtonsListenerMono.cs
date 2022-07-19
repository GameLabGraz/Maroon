using UnityEngine;

namespace ExtendedButtons
{
    public abstract class ButtonsListenerMono : MonoBehaviour, IButtonsListener
    {
        public abstract void Listener();

        public virtual void Update()
        {
            Listener();
        }
    }
}

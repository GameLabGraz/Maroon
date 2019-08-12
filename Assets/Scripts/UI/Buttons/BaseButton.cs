using UnityEngine;
using UnityEngine.UI;

namespace Maroon.UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public abstract class BaseButton : MonoBehaviour
    {
        protected SimulationController SimController;

        protected CanvasRenderer Renderer;

        protected Button Button;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            SimController = FindObjectOfType<SimulationController>();
            Renderer = GetComponent<CanvasRenderer>();
            Button = GetComponent<Button>();
        }

        protected void Enable()
        {
            Renderer.SetAlpha(1.0f);
            Button.interactable = true;
        }

        protected void Disable()
        {
            Renderer.SetAlpha(0.0f);
            Button.interactable = false;
        }
    }

}


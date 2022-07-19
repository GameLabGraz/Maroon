using UnityEngine;
using UnityEngine.UI;

namespace ExtendedButtons.Example
{
    public class Button2DExtendedExampleUsage : MonoBehaviour
    {
        [SerializeField] private Text debugText;

        private void Start()
        {
            var button = GetComponent<Button2DExtended>();
            button?.onEnter.AddListener(() =>
            {
                Debug.Log("OnButton Enter");
                debugText.text = "OnButton Enter";
            });
            button?.onDown.AddListener(() =>
            {
                Debug.Log("OnButton Down");
                debugText.text = "OnButton Down";
            });
            button?.onUp.AddListener(() =>
            {
                Debug.Log("OnButton Up");
                debugText.text = "OnButton Up";
            });
            button?.onClick.AddListener(() =>
            {
                Debug.Log("OnButton Click");
                debugText.text = "OnButton Click";
            });
            button?.onExit.AddListener(() =>
            {
                Debug.Log("OnButton Exit");
                debugText.text = "OnButton Exit";
            });
            button?.onBeginDrag.AddListener(() =>
            {
                Debug.Log("OnButton BeginDrag");
                debugText.text = "OnButton BeginDrag";
            });
            button?.onDrag.AddListener(() =>
            {
                Debug.Log("OnButton Drag");
                debugText.text = "OnButton Drag";
            });
            button?.onEndDrag.AddListener(() =>
            {
                Debug.Log("OnButton EndDrag");
                debugText.text = "OnButton EndDrag";
            });
        }
    }
}
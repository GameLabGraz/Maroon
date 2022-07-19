using UnityEngine;
using UnityEngine.UI;

namespace ExtendedButtons.Example
{
    public class Button2DExampleUsage : MonoBehaviour
    {
        [SerializeField] private Text debugText;

        private void Start()
        {
            var button = GetComponent<Button2D>();
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
        }
    }
}
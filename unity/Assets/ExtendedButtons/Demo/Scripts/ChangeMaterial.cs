using UnityEngine;

namespace ExtendedButtons.Example
{
    public class ChangeMaterial : MonoBehaviour
    {
        [SerializeField] private Material red;
        [SerializeField] private Material white;

        private void Start()
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            var button = GetComponent<Button3D>();
            button?.onEnter.AddListener(() =>
            {
                Debug.Log("OnButton Enter");
                meshRenderer.material = red;
            });
            button?.onDown.AddListener(() =>
            {
                Debug.Log("OnButton Down");
            });
            button?.onClick.AddListener(() =>
            {
                Debug.Log("OnButton Click");
            });
            button?.onUp.AddListener(() =>
            {
                Debug.Log("OnButton Up");
            });
            button?.onExit.AddListener(() =>
            {
                Debug.Log("OnButton Exit");
                meshRenderer.material = white;
            });
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.UI
{
    public class Tab : MonoBehaviour
    {
        [SerializeField] private Color activeColor = new Color(1f, 1f, 1f, 0.4f);
        [SerializeField] private Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 0.4f);

        [SerializeField] private GameObject tabContent;

        private Image _tabImage;

        private bool _interactable = true;

        private void Start()
        {
            _tabImage = GetComponent<Image>();
        }

        public void SetActive()
        {
            if (!_interactable) return;

            gameObject.SetActive(true);
            tabContent.SetActive(true);
            _tabImage.color = activeColor;
        }

        public void SetInactive()
        {
            tabContent.SetActive(false);
            _tabImage.color = inactiveColor;
        }
    }
}

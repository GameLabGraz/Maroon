using TMPro;
using UnityEngine;

namespace Maroon.Experiments.PlanetarySystem
{
    public class WhiteboardDisplay : MonoBehaviour
    {
        public TextMeshProUGUI whiteboardText;

        public void UpdateWhiteboardText(string info)
        {
            whiteboardText.text = info;
        }
    }
}
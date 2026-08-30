using TMPro;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class AddressTableRow : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI text1;
        [SerializeField]
        private TextMeshProUGUI text2;
        public string Text1 => text1.text;
        public string Text2 => text2.text;

        public void SetText(string text1, string text2) {
            this.text1.SetText(text1);
            this.text2.SetText(text2);
        }

        public void SetFontStyleBold() {
            text1.fontStyle = FontStyles.Bold;
            text2.fontStyle = FontStyles.Bold;
        }

        public void SetFontStyleNormal() {
            text1.fontStyle = FontStyles.Normal;
            text2.fontStyle = FontStyles.Normal;
        }
    }
}

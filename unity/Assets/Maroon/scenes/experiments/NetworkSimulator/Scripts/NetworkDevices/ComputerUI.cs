using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class ComputerUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI ipAddress;
        [SerializeField]
        private TextMeshProUGUI macAddress;

        [SerializeField]
        private Button[] tabButtons;
        private Image[] tabButtonImages;
        [SerializeField]
        private GameObject[] tabPanels;
        [SerializeField]
        private Color inactiveTabButtonColor;
        private Color activeTabButtonColor;

        [SerializeField]
        private Transform messageList;
        [SerializeField]
        private Button messageButtonTemplate;
        [SerializeField]
        private TextMeshProUGUI sender;
        [SerializeField]
        private TMP_InputField receivedMessage;

        [SerializeField]
        private Dropdown recipients;
        [SerializeField]
        private TMP_InputField message;

        [SerializeField]
        private GameObject overlay;

        private Computer computer;

        public void Initialize(Computer computer) {
            this.computer = computer;
            ipAddress.SetText(computer.IPAddress.ToString());
            macAddress.SetText(computer.MACAddress.ToString());
            tabButtonImages = new Image[tabButtons.Length];
            for(int i = 0; i < tabButtons.Length; i++) {
                tabButtonImages[i] = tabButtons[i].GetComponent<Image>();
            }
            activeTabButtonColor = tabButtonImages[0].color;
            ShowTab(0);
        }

        public void ShowTab(int index) {
            for(int i = 0; i < tabPanels.Length; i++) {
                tabButtonImages[i].color = inactiveTabButtonColor;
                tabPanels[i].SetActive(false);
            }
            tabButtonImages[index].color = activeTabButtonColor;
            tabPanels[index].SetActive(true);
        }

        public void Activate() {
            recipients.options.Clear();
            foreach(var recipient in computer.GetRecipients().OrderBy(r => r)) {
                recipients.options.Add(new Dropdown.OptionData(recipient));
            }
            recipients.interactable = recipients.options.Any();
            overlay.SetActive(false);
        }
        public void Deactivate() {
            overlay.SetActive(true);
        }

        public void SendMessagePacket() {
            if(!recipients.options.Any() || string.IsNullOrEmpty(message.text)) {
                return;
            }
            var recipient = recipients.options[recipients.value].text;
            computer.SendPacket(new IPAddress(recipient), message.text);
            message.text = string.Empty;
        }
        public void ReceiveMessage(IPAddress sender, string message) {
            var button = Instantiate(messageButtonTemplate, messageList);
            button.GetComponentInChildren<TextMeshProUGUI>().SetText($"{DateTime.Now:HH:mm} - {sender}");
            button.onClick.AddListener(() => {
                DisplayMessage(sender.ToString(), message);
            });
            button.gameObject.SetActive(true);
        }
        public void DisplayMessage(string sender, string message) {
            this.sender.SetText(sender);
            receivedMessage.text = message;
        }
    }
}

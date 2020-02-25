using UnityEngine;

namespace HelpCharacter
{
    [RequireComponent(typeof(HelpMessage))]
    public class HelpMessageClick : MonoBehaviour
    {
        private HelpMessage[] _helpMessages;

        private void Start()
        {
            _helpMessages = GetComponents<HelpMessage>();
        }

        private void OnMouseDown()
        {
            foreach (var helpMessage in _helpMessages)
                helpMessage.ShowMessage();
        }
    }
}

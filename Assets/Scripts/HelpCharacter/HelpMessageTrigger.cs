using UnityEngine;
using Util;

namespace HelpCharacter
{
    [RequireComponent(typeof(HelpMessage))]
    public class HelpMessageTrigger : MonoBehaviour
    {
        private HelpMessage[] _helpMessages;

        private void Start ()
        {
            _helpMessages = GetComponents<HelpMessage>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!PlayerUtil.IsPlayer(other.gameObject))
                return;

            foreach (var helpMessage in _helpMessages)
                helpMessage.ShowMessage();
        }
    }
}

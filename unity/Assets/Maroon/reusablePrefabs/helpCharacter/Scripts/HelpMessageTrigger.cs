using UnityEngine;
using Util;

namespace HelpCharacter
{
    [RequireComponent(typeof(HelpMessage))]
    public class HelpMessageTrigger : MonoBehaviour
    {
        private HelpMessage[] _helpMessages;
        private bool _trigger = true;
        private int _count;

        private void Start ()
        {
            _helpMessages = GetComponents<HelpMessage>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!PlayerUtil.IsPlayer(other.gameObject))
                return;

            if (_trigger)
            {
                foreach (var helpMessage in _helpMessages)
                    helpMessage.ShowMessage();

                _trigger = false;
            }

            if (_count++ > 2000) _trigger = true;
        }
    }
}

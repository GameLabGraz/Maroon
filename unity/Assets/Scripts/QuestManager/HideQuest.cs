using UnityEngine;

namespace QuestManager
{
    public class HideQuest : MonoBehaviour
    {
        private void OnTriggerEnter(Collider trigger)
        {
            if(trigger.CompareTag("QuestBody"))
                HideObjects(trigger.gameObject);
        }

        private void OnTriggerExit(Collider trigger)
        {
            if (trigger.CompareTag("QuestBody"))
                RevealObjects(trigger.gameObject);
        }

        private static void HideObjects(GameObject obj)
        {
            obj.GetComponentInParent<IQuest>().IsHidden = true;
            foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
                renderer.enabled = false;
        }

        private static void RevealObjects(GameObject obj)
        {
            obj.GetComponentInParent<IQuest>().IsHidden = false;
            foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
                renderer.enabled = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointChargeExperiment
{
    [ExecuteAlways]
    public class UISynchronizeLabelSizes : MonoBehaviour
    {
        public float maxLabelSize = 400;

        private void findChildrenWithTagRecursive(UnityEngine.GameObject parent, string tag, List<UnityEngine.GameObject> labels)
        {
            // Add to list if it has given tag
            TMPro.TMP_Text textComponent = parent.GetComponent<TMPro.TMP_Text>();
            UnityEngine.UI.LayoutElement layoutComponent = parent.GetComponent<UnityEngine.UI.LayoutElement>();
            if (parent.CompareTag(tag) && textComponent != null && layoutComponent != null)
            {
                labels.Add(parent);
            }

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                var child = parent.transform.GetChild(i).gameObject;
                findChildrenWithTagRecursive(child, tag, labels);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            List<UnityEngine.GameObject> labels = new List<UnityEngine.GameObject>();
            findChildrenWithTagRecursive(gameObject, "uiLabel", labels);

            // Figure out max width of all labels
            float maxWidth = 0.0f;
            foreach (var label in labels)
            {
                var textComponent = label.GetComponent<TMPro.TMP_Text>();
                if (textComponent == null) continue;
                float width = textComponent.GetPreferredValues().x;
                maxWidth = Mathf.Max(maxWidth, width);
            }

            maxWidth = Mathf.Min(maxWidth, maxLabelSize);

            // Set all label widths to max-width
            foreach (var label in labels)
            {
                var layoutElement = label.GetComponent<UnityEngine.UI.LayoutElement>();
                if (layoutElement == null) continue;
                layoutElement.minWidth = maxWidth;
            }
        }
    }
}

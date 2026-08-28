using System.Text;
using TMPro;
using UnityEngine;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public class PseudocodeUI : MonoBehaviour
    {
        [Header("ConvexHull")]
        [SerializeField] private ConvexHull3D hull;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI pseudocodeText;

        // -------------------------------------------------------------------------
        
        private void Start()
        {
            hull.OnStep             += HandleStep;
            hull.OnSimulationReset  += UpdatePseudoCodePanel;

            UpdatePseudoCodePanel();
        }

        private void OnDestroy()
        {
            hull.OnStep             -= HandleStep;
            hull.OnSimulationReset  -= UpdatePseudoCodePanel;
        }

        // -------------------------------------------------------------------------

        private void HandleStep(int _) => UpdatePseudoCodePanel();

        private void UpdatePseudoCodePanel()
        {

            var pseudoCodeLines = hull.CurrentAlgorithm.PseudocodeLines;
            int currentLine = hull.CurrentPseudoLine;

            var currentHighlightLine = new StringBuilder();

            for(int i = 0; i < pseudoCodeLines.Length; i++)
            {
                if(i == currentLine)
                {
                    currentHighlightLine.AppendLine($">> {pseudoCodeLines[i]}");
                }
                else
                {
                    currentHighlightLine.AppendLine($"    {pseudoCodeLines[i]}");
                }
            }

            pseudocodeText.text = currentHighlightLine.ToString();
        }
    }
}
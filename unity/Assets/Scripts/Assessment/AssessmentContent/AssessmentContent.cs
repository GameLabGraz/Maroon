using Antares.Evaluation.LearningContent;
using UnityEngine;

namespace Maroon.Assessment.Content
{
    public abstract class AssessmentContent : MonoBehaviour
    {
        public static string ResourceFolder = "CustomUI/";
        public static string PanelPrefab = ResourceFolder + "Panel";
        public static string TablePrefab = ResourceFolder + "Table";
        public static string TableRowPrefab = ResourceFolder + "TableRow";
        public static string TextPrefab = ResourceFolder + "Text";
        public static string DropDownPrefab = ResourceFolder + "DropDown";
        public static string InputPrefab = ResourceFolder + "Input";
        public static string ButtonPrefab = ResourceFolder + "Button";
        public static string ImagePrefab = ResourceFolder + "Image";
        public string ObjectId { get; set; } = null;

        public abstract void LoadContent(Node content);
    }
}


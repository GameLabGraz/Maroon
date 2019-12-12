using System.Threading.Tasks;
using Antares.Evaluation.LearningContent;
using Maroon.Util;
using UnityEngine;

namespace Maroon.Assessment.Content
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class AssessmentImage : AssessmentContent
    {
        public override void LoadContent(Node content)
        {
            if (!(content is Antares.Evaluation.LearningContent.Image image))
                return;

            var imageComponent = GetComponent<UnityEngine.UI.Image>();
            _ = LoadImage(imageComponent);
        }

        private static async Task LoadImage(UnityEngine.UI.Image imageContent)
        {
            // Test Image
            imageContent.sprite =
                await RemoteTexture.GetSprite(
                    "https://github.com/GamesResearchTUG/Maroon/blob/master/Assets/Images/logo.png?raw=true");
        }
    }
}

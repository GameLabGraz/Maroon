using System.Threading.Tasks;
using UnityEngine;
using Antares.Evaluation.LearningContent;
using GEAR.Gadgets.RemoteTexture;

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
            imageComponent.preserveAspect = true;

            _ = LoadImage(imageComponent, image.Uri);
        }

        private static async Task LoadImage(UnityEngine.UI.Image imageComponent, string url)
        {
            imageComponent.sprite = await RemoteTexture.GetSprite(url);
        }
    }
}

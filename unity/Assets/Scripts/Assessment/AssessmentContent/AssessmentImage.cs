using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Antares.Evaluation.LearningContent;

namespace Maroon.Assessment.Content
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class AssessmentImage : AssessmentContent
    {
        private UnityEngine.UI.Image _imageComponent;

        private void Start()
        {
            _imageComponent = GetComponent<UnityEngine.UI.Image>();
        }

        public override void LoadContent(Node content)
        {
            if (!(content is Antares.Evaluation.LearningContent.Image image))
                return;

            var imageComponent = GetComponent<UnityEngine.UI.Image>();
            imageComponent.preserveAspect = true;

            StartCoroutine(LoadImage(image.Uri));
        }

        private IEnumerator LoadImage(string url)
        {
            var www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var texture2d = ((DownloadHandlerTexture)www.downloadHandler).texture;
                _imageComponent.sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0f, 0f));
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lecture  : MonoBehaviour
{
    [SerializeField]
    private List<Texture> contents;

    private bool isDone = true;

    public List<Texture> Contents { get { return contents; } }

    public bool IsDone { get { return isDone; } }

    public void AddContentTexture(Texture content)
    {
        contents.Add(content);
    }

    public void LoadWebContents(List<string> urls)
    {
        isDone = false;
        StartCoroutine(LoadWebContentTextures(urls));
    }

    private IEnumerator LoadWebContentTextures(List<string> urls)
    {
        foreach (string url in urls)
        {
            WWW webContent = new WWW(url);
            yield return webContent;
            contents.Add(webContent.texture);
        }
        isDone = true;
    }
}
